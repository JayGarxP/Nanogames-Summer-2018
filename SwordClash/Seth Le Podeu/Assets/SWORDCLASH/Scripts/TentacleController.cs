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

        private Vector2 movePositionVelocity_TT;
        private Rigidbody2D TentacleTip_RB2D;
        private float startTentacleLength;
        private Vector2 tentacleReadyPosition;
        private float startTentacleRotation;
        private string UI_RotationValue;

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
        }

        // Use this for initialization; Here you setup things that depend on other components.
        void Start()
        {
            TentacleTip_RB2D = TentacleTip.GetComponent<Rigidbody2D>();
            tentacleReadyPosition = TentacleTip_RB2D.position;
            startTentacleLength = tentacleReadyPosition.magnitude;
            maxTentacleLength = startTentacleLength * 2;
            startTentacleRotation = TentacleTip_RB2D.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            RotationValue_UI_Text.text = UI_RotationValue;
        }

        void FixedUpdate()
        {
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


    }
}