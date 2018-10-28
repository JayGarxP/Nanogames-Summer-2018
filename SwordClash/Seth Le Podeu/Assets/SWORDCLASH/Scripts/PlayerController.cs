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
        #endregion
        private TapGestureRecognizer tapGesture;

        // Use this for initialization
        void Start()
        {
            CreateTapGesture();

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


        private void SpawnDot(float xCoordDot, float yCoordDot)
        {
            //Why this not instantiating at top of scene where it should be???
            GameObject dot = Instantiate(DotPrefab) as GameObject;
            dot.transform.position = new Vector2(xCoordDot, yCoordDot);

        }

    
    }
}