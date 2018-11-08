﻿using UnityEngine;
using UnityEngine.UI;
using DigitalRubyShared; //FingersLite
using System;

namespace SwordClash
{
    public class PlayerController : MonoBehaviour
    {
        #region EDITOR_FIELDS
        public GameObject DotPrefab;
        public Camera CameraReference;
        public float swipeCircleRayCastRadius; //~10.0f
        #endregion
        private TapGestureRecognizer tapGesture;
        private SwipeGestureRecognizer upSwipeGesture;
        private SwipeGestureRecognizer leftSwipeGesture;
        private SwipeGestureRecognizer rightSwipeGesture;
        private SwipeGestureRecognizer downSwipeGesture;


        // Use this for initialization
        void Start()
        {
            CreateTapGesture();
            CreateSwipeGestures();
        }

        ////FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
        //void FixedUpdate()
        //{

        //}

        void Update()
        {



        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            //tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                Vector2 touchPosinWorldSpace = CameraReference.ScreenToWorldPoint(new Vector2(gesture.FocusX, gesture.FocusY));
                SpawnDot(touchPosinWorldSpace.x, touchPosinWorldSpace.y);
            }
        }

        private void CreateSwipeGestures()
        {
            CreateUpSwipeGesture();
           
        }

        private void CreateUpSwipeGesture()
        {
            upSwipeGesture = new SwipeGestureRecognizer();
            upSwipeGesture.Direction = SwipeGestureRecognizerDirection.Up;
            upSwipeGesture.StateUpdated += SwipeGestureCallback_UP;
            upSwipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(upSwipeGesture);
        }

        private void SwipeGestureCallback_UP(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                //TODO: use gesture.Properties to actually launch projectile with path
                FlickTentacle(gesture.FocusX, gesture.FocusY);
            }
        }

        private void FlickTentacle(float endX, float endY)
        {
            //For now, just change color of dot that was flicked
            //ray cast; hit; hit.rigidboy add force OR change sprite
            Vector2 flickStart = new Vector2(upSwipeGesture.StartFocusX, upSwipeGesture.StartFocusY);

            Vector3 flickStartWORLD = CameraReference.ScreenToWorldPoint(flickStart);
            //Building start and ends of swipe, need to convert to world coordinates though
            Vector3 flickEndWORLD = CameraReference.ScreenToWorldPoint(new Vector2(endX, endY));

            //float swipeDistance = Vector3.Distance(flickStartWORLDCoords, flickEndWORLDCoords);
            //Vector2 flickDirection = f


            var heading = flickEndWORLD - flickStartWORLD;
            var swipeDistance = heading.magnitude;
            var swipeDirection = heading / swipeDistance; // This is now the normalized direction, unit, magnitude == 1.


            //should world coords .z be set to 0.0f????

            ////Use this property for distance checks, sqRT is CPU intensive
            //if (heading.sqrMagnitude < maxRange * maxRange)
            //{
            //    // Target is within range.
            //}


            // Cast a ray where flick focus is???
            RaycastHit2D[] hitDots = Physics2D.CircleCastAll(flickStartWORLD, swipeCircleRayCastRadius,
                swipeDirection, swipeDistance);

            // If it hits something...
            if (hitDots.Length > 0)
            {
                foreach (var dot in hitDots)
                {
                    dot.rigidbody.AddForceAtPosition(heading * 1000, dot.point);
                }
            }
        }

        private void SpawnDot(float xCoordDot, float yCoordDot)
        {
            GameObject dot = Instantiate(DotPrefab) as GameObject;
            dot.transform.position = new Vector2(xCoordDot, yCoordDot);

        }

    
    }
}