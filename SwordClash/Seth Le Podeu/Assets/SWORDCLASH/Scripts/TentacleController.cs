﻿using System;
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
        public Text RotationValue_UI_Text;

        public GameObject JellyfishEnemy;
        public Sprite tentacleTipStung_Sprite;

        private JellyfishController JelFishController;

        private Vector2 movePositionVelocity_TT;
        private Rigidbody2D TentacleTip_RB2D;
        private float startTentacleLength;
        private Vector2 tentacleReadyPosition;
        private float startTentacleRotation;
        private string UI_RotationValue;
        private SpriteRenderer m_SpriteRenderer;

        private bool tempJukeFlag;
        private bool tempJukeFlagLeft;

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

        //the subscriber class needs a reference to the publisher class in order to subscribe to its events.
        void Handle_JellyfishHitByTentacleTip_Event(object sender, EventArgs a)
        {
            // Change color/ZAP! and reset state into recharging
            m_SpriteRenderer.sprite = tentacleTipStung_Sprite;

            //Subscripe +=; Unsub -=;
            //publisher.RaiseCustomEvent += HandleCustomEvent;  
            //  publisher.RaiseCustomEvent -= HandleCustomEvent; 
        }

      

        // Setup the component you are on right now (the "this" object); before all Start()s
        void Awake()
        {
            MovePositionVelocity_TT = Vector2.zero;
            startTentacleLength = 0;

            tempJukeFlag = false;
            tempJukeFlagLeft = false;
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

            //Set sprite renderer reference so tentacle can change color
            m_SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            RotationValue_UI_Text.text = UI_RotationValue;
        }

        void FixedUpdate()
        {

            if (tempJukeFlag)
            {
                //TODO: make x coord adjustment a public editor field
                Vector2 currentPositionVector = new Vector2(TentacleTip_RB2D.position.x + 1f, TentacleTip_RB2D.position.y);
                TentacleTip_RB2D.position = currentPositionVector;
                tempJukeFlag = false;
            }
            else if (tempJukeFlagLeft) {
                Vector2 currentPositionVector = new Vector2(TentacleTip_RB2D.position.x - 1f, TentacleTip_RB2D.position.y);
                TentacleTip_RB2D.position = currentPositionVector;
                tempJukeFlagLeft = false;

            }


            if (TentacleTip_RB2D.position.magnitude < maxTentacleLength)
            {
                //Position = current position + (Velocity vector of swipe per physics frame)
                TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + MovePositionVelocity_TT * Time.fixedDeltaTime);
                //Set in PlayerController, updated here, consider adding if(bool angleSet), here it doesn't need to change, not sure which is faster...
                TentacleTip_RB2D.rotation = MoveRotationAngle;
                //TODO: use actual UI events or plugin for UI; this is terrible.
                UI_RotationValue = TentacleTip_RB2D.rotation.ToString();
            }
            else
            {
                TentacleTip_RB2D.MovePosition(tentacleReadyPosition); //just teleport for now. Later change state.
                MovePositionVelocity_TT = Vector2.zero; //zero out velocity vector
                TentacleTip_RB2D.rotation = startTentacleRotation;
                MoveRotationAngle = startTentacleRotation;

            }
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

        //Juke to the right, eventaully will only work 3 times either way
        public void JukeRight()
        {
            tempJukeFlag = true; //TODO: make into IDISPOSABLE STATE MACHINE!!!
           // Vector2 currentPositionVector = TentacleTip_RB2D.position;
           // currentPositionVector.x += 100f;
           // TentacleTip_RB2D.position = currentPositionVector;

        }
        public void JukeLeft()
        {
            tempJukeFlagLeft = true; 
            //TODO: spawn bubbles on Right side; spawn bubs on left for JukeRight()

        }


    }
}