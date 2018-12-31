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
        public float maxTentacleLength;
        public float bRollRotateDegs; //Degrees to rotate tentacle tip per physics update, ~20 looks good.
        public float BROLLEndSpinRotationDegrees; //snowboard style degrees of rotation until barrel roll ends, two rotations = 720.
        public Text RotationValue_UI_Text;
        public float TT_jukePosRight_Amount;
        public float TT_jukePosLeft_Amount;



        public GameObject JellyfishEnemy;
        public GameObject MovingJellyfishEnemy;
        public Sprite tentacleTipStung_Sprite;

        //Tried to implement enterprise OO gang of four State pattern, was denied by Unity Component architecture...
        public TentacleState CurrentTentacleState { get; set; }

        private JellyfishController JelFishController;

        private Vector2 movePositionVelocity_TT;
        private Rigidbody2D TentacleTip_RB2D;
        private float startTentacleLength;
        private Vector2 tentacleReadyPosition;
        private float startTentacleRotation;
        private string UI_RotationValue;
        private SpriteRenderer m_SpriteRenderer;
        private Sprite m_SceneSprite; //sprite object starts with
        private float BROLLFlagCurrentRotationDegs; //Barrel roll degrees rotated currently, used to keep rotating the tentacle tip until the degrees specified in editor(EndSpin) are hit
       

        public Vector2 MovePositionVelocity_TT
        {
           get
            {
                return movePositionVelocity_TT;
            }

            set
            {
                //TODO: Check refactory period timer here???
                movePositionVelocity_TT = value;
            }
        }

        public float MoveRotationAngle { get; set; }

        // Setup the component you are on right now (the "this" object); before all Start()s
        void Awake()
        {
            MovePositionVelocity_TT = Vector2.zero;
            startTentacleLength = 0;
            BROLLFlagCurrentRotationDegs = 0.0f;
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
        public void MoveTentacleTip()
        {

            //be careful! if physics update is not finished and you MovePosition() in same update frame, unexpected behavior will occur!
            //Position = current position + (Velocity vector of swipe per physics frame) 
            TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + MovePositionVelocity_TT * Time.fixedDeltaTime);
            //Set in PlayerController, updated here, consider adding if(bool angleSet), here it doesn't need to change, not sure which is faster...
            TentacleTip_RB2D.rotation = MoveRotationAngle;
            //TODO: use actual UI events or plugin for UI; this is terrible.
            UI_RotationValue = TentacleTip_RB2D.rotation.ToString();
        }

        public bool IsTentacleAtMaxExtension()
        {
            return TentacleTip_RB2D.position.magnitude >= maxTentacleLength;
        }

        private void ReelBack()
        {
            TentacleTip_RB2D.MovePosition(tentacleReadyPosition); //just teleport for now. Later change state.
            MovePositionVelocity_TT = Vector2.zero; //zero out velocity vector
            TentacleTip_RB2D.rotation = startTentacleRotation;
            MoveRotationAngle = startTentacleRotation;
        }

        private void Do_A_BarrelRoll()
        {
            ////TentacleTip_RB2D.rotation = TentacleTip_RB2D.rotation + (20); //rotates along wrong centroid, from collider, not centerered....
            //TentacleTip.transform.Rotate(0, 0, bRollRotateDegs, Space.World); //rotate gameobject via transform Space.World centroid, looks cooler.
            //BROLLFlagCurrentRotationDegs += bRollRotateDegs;

            ////still move, but more slowly
            ////Position = current position + (Velocity vector of swipe per physics frame)
            //TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + (MovePositionVelocity_TT * (Time.fixedDeltaTime * 0.5f)));

            ////If the barrelroll is over; the total spin 360, 720, etc. has been overcome by degrees of rotation per frame
            //if (BROLLFlagCurrentRotationDegs >= BROLLEndSpinRotationDegrees)
            //{
            //    BarrelRoll_Flag = false;
            //    BROLLFlagCurrentRotationDegs = 0;
            //    TentacleTip_RB2D.rotation = startTentacleRotation;
            //}
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


        //TODO: solve signal problem
        //Player controller receives input events
        //Call method directly on current state?
        //Use simple mutex around bool that does not allow flag changes during ProcessState()?
        //'processing state' in base class variable
        //no logic in the setters
        //check that the state is not being processed before setting flag outside of ProcessState()
        //inside ProcessState() just change the flag.

        //Juke to the right, eventaully will only work 3 times either way; called by player controller
        public void JukeRight_Please()
        {
            //Use InputFlag enum in tentacle state to raise correct flag, casted to int
            int RudderRight = (int)TentacleState.InputFlag_Enum.RudderRight;
            CurrentTentacleState.RaiseTentacleFlag_Request(RudderRight);
        }
        //TODO: spawn bubbles on Right side; spawn bubs on left for JukeRight()
        public void JukeLeft()
        {
            int RudderLeft = (int)TentacleState.InputFlag_Enum.RudderLeft;
            CurrentTentacleState.RaiseTentacleFlag_Request(RudderLeft);
        }

        public void BarrelRoll()
        {
            //i-frames begin
           // BarrelRoll_Flag = true;

            //for now, just rotate tip...

        }

        public void ReelInTentacle()
        {
            //ReelBack_Flag = true;
            ResetTentacleTipSprite();

        }



        private void ResetTentacleTipSprite()
        {
            //reset tentacle tip sprite to starting sprite; reference set in the Start() method
            m_SpriteRenderer.sprite = m_SceneSprite;
        }


    }
}