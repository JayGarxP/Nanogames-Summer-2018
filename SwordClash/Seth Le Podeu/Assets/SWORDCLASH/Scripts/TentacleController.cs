using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordClash
{
    public class TentacleController : MonoBehaviour
    {
        public GameObject TentacleTip;
        public float UPswipeSpeedConstant; //constant speed of tentacle
        public float UPswipeSpeedModifier; //Added to constant speed of tentacle
        public float maxTentacleLength;
        public float bRollRotateDegs; //Degrees to rotate tentacle tip per physics update, ~20 looks good.
        public float BROLLEndSpinRotationDegrees; //snowboard style degrees of rotation until barrel roll ends, two rotations = 720.
        public short TimesCanBarrelRoll; //2 times means each up-swipe launch player gets two barrel rolls, reset once coiled
        public Text RotationValue_UI_Text;
        public float TT_jukePosRight_Amount;
        public float TT_jukePosLeft_Amount;
        public float TT_timesAllowedToJuke; //3 means 3 taps can happen in one 'strike'



        public GameObject JellyfishEnemy;
        public GameObject MovingJellyfishEnemy;
        public Sprite tentacleTipStung_Sprite;

        //Tried to implement enterprise OO gang of four State pattern, was denied by Unity Component architecture...
        public TentacleState CurrentTentacleState { get; set; }

        private JellyfishController JelFishController;

        private Vector2 movePositionVelocity_TT_Active;
        public Vector2 movePositionVelocity_TT_Requested;
        public float moveRotationAngle_TT_Requested; //for training mode 'ghosts'
        private Rigidbody2D TentacleTip_RB2D;
        private float startTentacleLength;
        private Vector2 tentacleReadyPosition;
        private float startTentacleRotation;
        private string UI_RotationValue;
        private SpriteRenderer m_SpriteRenderer;
        private Sprite m_SceneSprite; //sprite object starts with
       

        public Vector2 MovePositionVelocity_TT_Active
        {
           get
            {
                return movePositionVelocity_TT_Active;
            }

            set
            {
                movePositionVelocity_TT_Active = value;
            }
        }

        public float MoveRotationAngle_TT_Active { get; set; }

        // Setup the component you are on right now (the "this" object); before all Start()s
        void Awake()
        {
            MovePositionVelocity_TT_Active = Vector2.zero;
            startTentacleLength = 0;
        }

        // Use this for initialization; Here you setup things that depend on other components.
        void Start()
        {
            TentacleTip_RB2D = TentacleTip.GetComponent<Rigidbody2D>();
            tentacleReadyPosition = TentacleTip_RB2D.position;
            startTentacleLength = tentacleReadyPosition.magnitude;
            maxTentacleLength = startTentacleLength * 2; //TODO: fix maxtentacleLength solution
            startTentacleRotation = TentacleTip_RB2D.rotation;
            //Collide with jellyfish event subscription
            JelFishController = JellyfishEnemy.GetComponent<JellyfishController>();
            JelFishController.JellyfishHitByTentacleTip_Event += Handle_JellyfishHitByTentacleTip_Event;
            //TODO: child jellyfish in editor is NULL reference here... how avoid needing each controller instance??? Base class? Way for GetComponent to get ALL???
            var bigJelFishController = MovingJellyfishEnemy.GetComponent<JellyfishController>();
            bigJelFishController.JellyfishHitByTentacleTip_Event += Handle_JellyfishHitByTentacleTip_Event;

            //Set sprite renderer reference so tentacle can change color
            m_SpriteRenderer = this.GetComponent<SpriteRenderer>();
            m_SceneSprite = m_SpriteRenderer.sprite;

            //Redundant cast seems to help avoid null reference in update loop
            CurrentTentacleState = new CoiledState(((TentacleController)this));
        }

        // Update is called once per frame
        void Update()
        {
            RotationValue_UI_Text.text = UI_RotationValue;
        }

        //TODO: understand the differences between each update loop, and best practices.
        void FixedUpdate()
        {
            if (CurrentTentacleState != null)
            {
                CurrentTentacleState.ProcessState();
            }

        }

        public void TT_RecoilTentacle()
        {
            ReelBack();
        }

        //TODO: make into seperate methods and or states to move tentacle
        public void TT_MoveTentacleTip()
        {

            //be careful! if physics update is not finished and you MovePosition() in same update frame, unexpected behavior will occur!
            //Position = current position + (Velocity vector of swipe per physics frame) 
            TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + MovePositionVelocity_TT_Active * Time.fixedDeltaTime);
            //Set in PlayerController, updated here, consider adding if(bool angleSet), here it doesn't need to change, not sure which is faster...
            TentacleTip_RB2D.rotation = MoveRotationAngle_TT_Active;

            //TODO: use actual UI events or plugin for UI; this is terrible.
            //UI_RotationValue = TentacleTip_RB2D.rotation.ToString();
        }
        
        public void TT_MoveTentacleTip(Vector2 swipePositionVelocity, float swipeAngle)
        {
            //be careful! if physics update is not finished and you MovePosition() in same update frame, unexpected behavior will occur!
            //Position = current position + (Velocity vector of swipe per physics frame) 
            TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + swipePositionVelocity * Time.fixedDeltaTime);
            //Set in PlayerController, updated here, consider adding if(bool angleSet), here it doesn't need to change, not sure which is faster...
            TentacleTip_RB2D.rotation = swipeAngle;
        }

        public void TT_MoveTentacleTip_WhileBroll(Vector2 swipePositionVelocity)
        {
            //Move at half delta time speed ~around sqrt the normal speed. Do not rotate, rotate seperately in another method.
        TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + (swipePositionVelocity * (Time.fixedDeltaTime* 0.5f)));
        }

            

        public bool IsTentacleAtMaxExtension()
        {
            return TentacleTip_RB2D.position.magnitude >= maxTentacleLength;
        }

        private void ReelBack()
        {
            TentacleTip_RB2D.MovePosition(tentacleReadyPosition); //just teleport for now. Later change state.
            MovePositionVelocity_TT_Active = Vector2.zero; //zero out velocity vector
            TentacleTip_RB2D.rotation = startTentacleRotation;
            MoveRotationAngle_TT_Active = startTentacleRotation;
        }

        public float BarrelRollin_rotate(float degreesRotatedSoFar)
        {
            //TentacleTip_RB2D.rotation = TentacleTip_RB2D.rotation + (20); //rotates along wrong centroid, from collider, not centerered....
            TentacleTip.transform.Rotate(0, 0, bRollRotateDegs, Space.World); //rotate gameobject via transform Space.World centroid, looks cooler.
            degreesRotatedSoFar += bRollRotateDegs;
            return degreesRotatedSoFar;
        }

        //For now is x position '-' instead of rights '+'; but juking may change in future, so leave as
        // two seperate methods.
        public void TT_JumpLeft()
        {
            TentacleTip_JumpLeft(TT_jukePosLeft_Amount);
        }
        private void TentacleTip_JumpLeft(float xPositionUnitstoJump)
        {
            Vector2 currentPositionVector = new Vector2(TentacleTip_RB2D.position.x - xPositionUnitstoJump, TentacleTip_RB2D.position.y);
            TentacleTip_RB2D.position = currentPositionVector;
        }

        //called from fixed update, inside a state's ProcessState() method.
        public void TT_JumpRight()
        {
            TentacleTip_JumpRight(TT_jukePosRight_Amount);
        }
        private void TentacleTip_JumpRight(float xPositionUnitstoJump)
        {
            Vector2 currentPositionVector = new Vector2(TentacleTip_RB2D.position.x + xPositionUnitstoJump, TentacleTip_RB2D.position.y);
            TentacleTip_RB2D.position = currentPositionVector;
        }

        private void OnDestroy()
        {
            //Unsubscribe from events
            if (JelFishController != null)
            {
                JelFishController = JellyfishEnemy.GetComponent<JellyfishController>();
                JelFishController.JellyfishHitByTentacleTip_Event -= Handle_JellyfishHitByTentacleTip_Event;

            }
        }

        
        //the subscriber class needs a reference to the publisher class in order to subscribe to its events.
        void Handle_JellyfishHitByTentacleTip_Event(object sender, EventArgs a)
        {
            //if (! BarrelRoll_Flag)
            //{
            //    // Change color/ZAP! and reset state into recharging
            //    m_SpriteRenderer.sprite = tentacleTipStung_Sprite;
            //}
            //Subscripe +=; Unsub -=;
            //publisher.RaiseCustomEvent += HandleCustomEvent;  
            //  publisher.RaiseCustomEvent -= HandleCustomEvent; 
        }
        
        //Juke to the right, eventaully will only work 3 times either way; called by player controller
        public void JukeRight_Please()
        {
            //Use InputFlag enum in tentacle state to raise correct flag, casted to int
            int RudderRight = (int)TentacleState.InputFlag_Enum.RudderRight;
            CurrentTentacleState.RaiseTentacleFlag_Request(RudderRight);
        }
        //TODO: spawn bubbles on Right side; spawn bubs on left for JukeRight()
        public void JukeLeft_Please()
        {
            int RudderLeft = (int)TentacleState.InputFlag_Enum.RudderLeft;
            CurrentTentacleState.RaiseTentacleFlag_Request(RudderLeft);
        }

        public void LaunchTentacle_Please(Vector2 SwipeDirectionVector, float SwipeAngle_Unity)
        {
            //Save requested swipe (linear intepolation of swipes over time, 
            //  with angles in RB2D.rotation friendly range)
            movePositionVelocity_TT_Requested = SwipeDirectionVector;
            moveRotationAngle_TT_Requested = SwipeAngle_Unity;

            int LaunchTentFlagID = (int)TentacleState.InputFlag_Enum.LaunchSwipe;
            CurrentTentacleState.RaiseTentacleFlag_Request(LaunchTentFlagID);
        }


        public bool BarrelRoll_Please()
        {
            int barrelRollFlagID = (int)TentacleState.InputFlag_Enum.BarrelRoll;
            bool successfullyRaised = 
            CurrentTentacleState.RaiseTentacleFlag_Request(barrelRollFlagID);
            return successfullyRaised;
        }



        public void ReelInTentacle()
        {
            //ReelBack_Flag = true;
            ResetTentacleTipSprite();

        }

        public void ResetTentacleTipRotation()
        {
            TentacleTip_RB2D.rotation = startTentacleRotation;
        }

        private void ResetTentacleTipSprite()
        {
            //reset tentacle tip sprite to starting sprite; reference set in the Start() method
            m_SpriteRenderer.sprite = m_SceneSprite;
        }


    }
}