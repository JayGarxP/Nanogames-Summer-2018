using UnityEngine;
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
       
        public GameObject LeftTentacle;
        #endregion
        private TapGestureRecognizer tapGesture;
        private SwipeGestureRecognizer upSwipeGesture;
        private SwipeGestureRecognizer leftSwipeGesture;
        private SwipeGestureRecognizer rightSwipeGesture;
        private SwipeGestureRecognizer downSwipeGesture;

        private short dotCount;
        private TentacleController tentaController;
        

        // Use this for initialization
        void Start()
        {
            CreateTapGesture();
            CreateSwipeGestures();
            dotCount = 0;

                    
            tentaController = LeftTentacle.GetComponent<TentacleController>(); //how check if null???

        }

    ////FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    //void FixedUpdate()
    //{

    //}

    //void Update()
    //    {



    //    }


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
            //tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                if (dotCount < 4)
                {
                    Vector2 touchPosinWorldSpace = CameraReference.ScreenToWorldPoint(new Vector2(gesture.FocusX, gesture.FocusY));
                    SpawnDot(touchPosinWorldSpace.x, touchPosinWorldSpace.y);
                    dotCount++;
                }
             }
        }

        private void CreateUpSwipeGesture()
        {
            upSwipeGesture = new SwipeGestureRecognizer();
            upSwipeGesture.Direction = SwipeGestureRecognizerDirection.Up;
            upSwipeGesture.StateUpdated += SwipeGestureCallback_UP;
            upSwipeGesture.DirectionThreshold = 1.5f; // 1.5* greater y axis movement
            FingersScript.Instance.AddGesture(upSwipeGesture);
        }

        private void SwipeGestureCallback_UP(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                //TODO: use gesture.Properties to actually launch projectile with path

                Vector2 velocityPixels = new Vector2(gesture.VelocityX, gesture.VelocityY);
                Vector2 forceofSwipe = CameraReference.ScreenToWorldPoint(velocityPixels);

                //Have to apply smoothing; floor; ceiling; and implement extension length...
                //TODO: Kinematic vs Dynamic for projectiles what the fuck... Fixed Update vs normal update WTF...
                tentaController.MovePositionVelocity_TT = forceofSwipe;

                FlickTentacle(gesture as SwipeGestureRecognizer);
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
                FlickTentacle(gesture as SwipeGestureRecognizer);
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
            whichSwipe.DirectionThreshold = 1.5f;
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
                        dot.rigidbody.AddForce(forceofSwipe / 2, ForceMode2D.Impulse);
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