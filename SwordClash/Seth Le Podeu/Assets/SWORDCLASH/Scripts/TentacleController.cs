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

        //// Update is called once per frame
        //void Update()
        //{
        //}

        void FixedUpdate()
        {
            if (TentacleTip_RB2D.position.magnitude < maxTentacleLength)
            {
                // Consider adding weight multiplier here to velocity as public field. In tutorial move with heavy weight to make it easier
                TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + MovePositionVelocity_TT * Time.fixedDeltaTime);
                //TentacleTip_RB2D.transform.Rotate(Vector3.forward * Time.fixedDeltaTime);
                //TentacleTip_RB2D.transform.Rotate(HeadingVector where tip will end up??? * Time.fixedDeltaTime);
                //TentacleTip_RB2D.rotation = -45.0f;
                //use starting position vector + velocity of swipe vector to calculute where it will end up 
                //also have starting magnitude of position vector for max distance tentacle can travel.

                TentacleTip_RB2D.rotation = MoveRotationAngle; 
                //TentacleTip_RB2D.rotation += 0.05f;

                RotationValue_UI_Text.text = TentacleTip_RB2D.rotation.ToString();


            }
            else
            {
                TentacleTip_RB2D.MovePosition(tentacleReadyPosition); //just teleport for now.
                MovePositionVelocity_TT = Vector2.zero; //zero out velocity vector
                TentacleTip_RB2D.rotation = startTentacleRotation;
                MoveRotationAngle = startTentacleRotation;

            }
        }


    }
}