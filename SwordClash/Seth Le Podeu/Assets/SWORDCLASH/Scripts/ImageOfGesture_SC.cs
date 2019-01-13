using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRubyShared;
using UnityEngine;

namespace SwordClash
{
    class ImageOfGesture_SC : MonoBehaviour
    {
        //ImageScript set in editor, to recognize circles,
        // see Image and Shape Recognition Training with Fingers - Touch Gestures for Unity by Jeff Johnson Digital Ruby
        // https://www.youtube.com/watch?v=ljQkuqo1dV0
        public FingersImageGestureHelper_SC_BarrelRoll ImageScript;

        private void LateUpdate()
        {
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    ImageScript.Reset();
            //}
            
                ImageGestureImage match = ImageScript.CheckForImageMatch();
                if (match != null)
                {
                    //send barrel roll flag
                    
                }
                else
                {
                    
                }

              
        }

    }
}
