﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordClash
{
    public class TentacleController : MonoBehaviour
    {
        public GameObject TentacleTip;
        public float maxTentacleLength;

        private Vector2 movePositionVelocity_TT;
        private Rigidbody2D TentacleTip_RB2D;

        public Vector2 MovePositionVelocity_TT
        {
           get
            {
                return movePositionVelocity_TT;
            }

            set
            {
                movePositionVelocity_TT = value;
            }
        }

        // Setup the component you are on right now (the "this" object); before all Start()s
        void Awake()
        {
            MovePositionVelocity_TT = Vector2.zero;
        }

        // Use this for initialization; Here you setup things that depend on other components.
        void Start()
        {
            TentacleTip_RB2D = TentacleTip.GetComponent<Rigidbody2D>();
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        void FixedUpdate()
        {
            TentacleTip_RB2D.MovePosition(TentacleTip_RB2D.position + MovePositionVelocity_TT * Time.fixedDeltaTime);
        }

    }
}