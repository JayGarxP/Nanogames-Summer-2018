﻿using UnityEngine;
using UnityEngine.UI;
using DigitalRubyShared; //FingersLite
using System;

namespace SwordClash
{
    public class PlayerController : MonoBehaviour
    {
        #region EDITOR_FIELDS
        public float swipeCircleRayCastRadius; //~10.0f
        public float swipeSpeedConstant; //constant speed of tentacle
        public float swipeSpeedModifier; //Added to constant speed of tentacle
        //TODO: need more fine-grained control than just dividend, need clamp and smoothing + better gesture properties to not have deadzones!
        public float UPSwipeGestureDirectionThreshold; //= 1f; // where 1.5 means 1.5* greater y axis movement needed to for gesture event to fire.
        public float L_R_D_SwipeGestureDirectionThreshold; //left right down swipes need to be more precise
        public float maxDotsSpawnable;
        public float doubleTapTimeThreshold; //time between taps allowed for double tap
        //TODO: consider naming convention for editor fields, like field_EF;
        public GameObject DotPrefab;
        public Camera CameraReference;
        public Text SwipeAngleText;
       
        public GameObject LeftTentacle;
        #endregion

        private TapGestureRecognizer tapGesture; //juke by which half of screen tapped
        private TapGestureRecognizer doubleTapGesture; //dodge roll if double tap on tentacle tip
        private SwipeGestureRecognizer upSwipeGesture; //send out tentacle
        private SwipeGestureRecognizer leftSwipeGesture; //wall jump off right wall
        private SwipeGestureRecognizer rightSwipeGesture; //wall jump off left wall
        private SwipeGestureRecognizer downSwipeGesture; //Reel in tentacle

        private short dotCount;
        private TentacleController tentaController;
        private Vector2 tentacleTipStartPosition;

        private string swipeAngleTextString;
        

        // Use this for initialization
        void Start()
        {
            CreateDoubleTapGesture(); //TODO: find event order solution: https://stackoverflow.com/questions/374398/are-event-subscribers-called-in-order-of-subscription

            CreateTapGesture();
            //CreateDoubleTapGesture(); //test if order matters; it does sadly... :(
            CreateSwipeGestures();
            dotCount = 0;
            swipeAngleTextString = SwipeAngleText.text;

           tentaController = LeftTentacle.GetComponent<TentacleController>(); //how check if null???
           tentacleTipStartPosition = tentaController.GetComponent<Rigidbody2D>().position;

        }

        ////FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
        //void FixedUpdate()
        //{

        //}

        void Update()
        {

            SwipeAngleText.text = swipeAngleTextString; //UI only updated here??? why don't events work wtf!!!

        }


        private void CreateSwipeGestures()
        {
            CreateUpSwipeGesture();
            CreateDownSwipeGesture();
            CreateLeftSwipeGesture();
            CreateRightSwipeGesture();

        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {

                // Vector2 touchPosinWorldSpace = CameraReference.ScreenToWorldPoint(new Vector2(gesture.FocusX, gesture.FocusY));
                // SpawnDot(touchPosinWorldSpace.x, touchPosinWorldSpace.y);
                //CameraScaledWidth lines up nicely with gesture.Focus units somehow...
                float ScreeenWidth = CameraReference.scaledPixelWidth;
                ScreeenWidth = ScreeenWidth / 2.0f;

                //Determine what side of screen is tapped, 'juke' to that side.
                if (gesture.FocusX >= ScreeenWidth)
                {

                    tentaController.JukeRight();
                }
                else {
                    tentaController.JukeLeft();
                }

                    
                
             }
        }

        private void CreateDoubleTapGesture()
        {
            doubleTapGesture = new TapGestureRecognizer();
            doubleTapGesture.NumberOfTapsRequired = 2;
            doubleTapGesture.ThresholdSeconds = doubleTapTimeThreshold;
            doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
            //doubleTapGesture.RequireGestureRecognizerToFail = tripleTapGesture;
            FingersScript.Instance.AddGesture(doubleTapGesture);
        }

        private void DoubleTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                //DebugText("Double tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
                //RemoveAsteroids(gesture.FocusX, gesture.FocusY, 16.0f);
                //SpinTentacle
                tentaController.BarrelRoll();

            }
        }

        private void CreateUpSwipeGesture()
        {
            upSwipeGesture = new SwipeGestureRecognizer();
            upSwipeGesture.Direction = SwipeGestureRecognizerDirection.Up;
            upSwipeGesture.StateUpdated += SwipeGestureCallback_UP;
            upSwipeGesture.DirectionThreshold = UPSwipeGestureDirectionThreshold; //still has 6 degree dead zone??? 39 to 32 if dirThresh is set to 1
            FingersScript.Instance.AddGesture(upSwipeGesture);
        }

        private void SwipeGestureCallback_UP(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                Vector2 normalizedSwipeVelocityVector = new Vector2(gesture.VelocityX, gesture.VelocityY).normalized;

                //BAD! Do not do!!! FingersLite uses arbitrary Iphone inch pixel units, not actual pixels, ScrrentoWorldPoint() has big rounding errors!
                //Vector2 forceofSwipe = CameraReference.ScreenToWorldPoint(velocityPixels);

                //MovePosition Velocity = direction vector * speed
                tentaController.MovePositionVelocity_TT = normalizedSwipeVelocityVector * (swipeSpeedConstant + swipeSpeedModifier); 


                // swipe angle is the swipe gesture's launch angle = inverse tan(change in y position / change x position)
                float swipeAngle = Mathf.Rad2Deg * Mathf.Atan2(gesture.DeltaY, gesture.DeltaX);

                // need to subtract 90 since RB2D.rotation units are clockwise: 0 @noon, -90 @3pm, -179 @5:59pm, 180 @6pm, 90 @9pm
                //versus the normal unit circle units that Atan2 spits out clockwise: 90 @noon, 0 @3pm, -89 @5:59pm, -90 @6pm, -180 @9pm -270 @midnight, -360 @3am 
                tentaController.MoveRotationAngle = Mathf.Round(swipeAngle - 90.0f); //rotation has little precision, rounding feels better in-game
                                                                                     //rotation units are wonky, only go to 180 to negative 180 and straight up is 0 degrees not 90.
                                                                                     //Since the up swipe only allows swipe angle to be unit circle degrees ~39 to ~136, simply subtracting 90 translates fine.

                swipeAngleTextString += "  " + Mathf.Floor(swipeAngle).ToString();

                //FlickTentacle(gesture as SwipeGestureRecognizer);
            }
        }

        private void CreateRightSwipeGesture()
        {
            CreateDotSwipeGesture(rightSwipeGesture, SwipeGestureRecognizerDirection.Right, SwipeGestureCallback_RIGHT);
        }

        private void CreateLeftSwipeGesture()
        {
            CreateDotSwipeGesture(leftSwipeGesture, SwipeGestureRecognizerDirection.Left, SwipeGestureCallback_LEFT);
        }

        private void CreateDownSwipeGesture()
        {
            CreateDotSwipeGesture(downSwipeGesture, SwipeGestureRecognizerDirection.Down, SwipeGestureCallback_DOWN);
        }

        private void SwipeGestureCallback_DOWN(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                //FlickTentacle(gesture as SwipeGestureRecognizer);
                tentaController.ResetTentacleTipSprite();
            }
        }

        private void SwipeGestureCallback_LEFT(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                FlickTentacle(gesture as SwipeGestureRecognizer);

            }
        }

        private void SwipeGestureCallback_RIGHT(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                FlickTentacle(gesture as SwipeGestureRecognizer);

            }
        }

        private void CreateDotSwipeGesture(SwipeGestureRecognizer whichSwipe, SwipeGestureRecognizerDirection direction, GestureRecognizerStateUpdatedDelegate GestureCallback)
        {
            whichSwipe = new SwipeGestureRecognizer();
            whichSwipe.Direction = direction;
            whichSwipe.StateUpdated += GestureCallback;
            whichSwipe.DirectionThreshold = L_R_D_SwipeGestureDirectionThreshold;
            FingersScript.Instance.AddGesture(whichSwipe);
        }

        private void FlickTentacle(SwipeGestureRecognizer swipeGesture)
        {
            Vector2 velocityPixels = new Vector2(swipeGesture.VelocityX, swipeGesture.VelocityY);

            Vector2 flickStart = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);

            Vector3 flickStartWORLD = CameraReference.ScreenToWorldPoint(flickStart);
            //Building start and ends of swipe, need to convert to world coordinates though
            Vector3 flickEndWORLD = CameraReference.ScreenToWorldPoint(new Vector2(swipeGesture.FocusX, swipeGesture.FocusY));

            flickStartWORLD.z = 0.0f;
            flickEndWORLD.z = 0.0f; //zero out z values just in case


            var heading = flickEndWORLD - flickStartWORLD;
            var swipeDistance = heading.magnitude;
            var swipeDirection = heading / swipeDistance; // This is now the normalized direction, unit, magnitude == 1.

            //Vector2 forceofSwipe = new Vector2(heading.x, heading.y);
            Vector2 forceofSwipe = CameraReference.ScreenToWorldPoint(velocityPixels);

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
                    DotController swipedDot = dot.collider.GetComponent<DotController>();
                    if (swipedDot != null) //TODO: why does raycast hit everything WTF!!!!
                    {
                        swipedDot.OnSwipe(swipeGesture.Direction);
                        //dot.rigidbody.AddForce(forceofSwipe / swipeVelocityDividend, ForceMode2D.Impulse);
                        dot.rigidbody.AddForce(forceofSwipe / 4, ForceMode2D.Impulse);

                    }
                }
            }
        }


     

        private void SpawnDot(float xCoordDot, float yCoordDot)
        {
            //TODO: count dots in scene to set max first, make that a public field
            GameObject dot = Instantiate(DotPrefab) as GameObject;
            dot.transform.position = new Vector2(xCoordDot, yCoordDot);

        }

    
    }
}