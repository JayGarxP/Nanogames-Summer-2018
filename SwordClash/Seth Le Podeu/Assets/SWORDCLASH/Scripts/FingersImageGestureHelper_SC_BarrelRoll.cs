using DigitalRubyShared;
using System.Collections.Generic;

namespace SwordClash
{
    public class FingersImageGestureHelper_SC_BarrelRoll : ImageGestureRecognizerComponentScript
    {
        private ImageGestureImage matchedInputImage;

        //Required gesturest to fail before image can be matched
        public int RequireTheseGesturesToFail(List<GestureRecognizer> needTaFails)
        {
            int numGestureRecsFiredBeforeBarrelRollCircleImage = 0;
            foreach (var jester in needTaFails)
            {
                Gesture.AddRequiredGestureRecognizerToFail(jester);
                numGestureRecsFiredBeforeBarrelRollCircleImage++;
            }

            return numGestureRecsFiredBeforeBarrelRollCircleImage;
        }

        public ImageGestureImage CheckForImageMatch()
        {
            if (matchedInputImage == null)
            {
                //if (MatchText != null)
                //{
                //    MatchText.text = "No match found!";
                //}
            }
            else
            {
                //if (MatchText != null)
                //{
                //    MatchText.text = "You drew a " + matchedInputImage.Name;
                //}

                // image gesture must be manually reset when a shape is recognized
                Gesture.Reset();

            }

            return matchedInputImage;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        
        public void GestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
            
            if (gesture.State == GestureRecognizerState.Began)
            {
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                // save off the matched image, the gesture may reset if max path count has been reached
                matchedInputImage = this.Gesture.MatchedGestureImage;
            }
            //else
            //{

            //    return;
            //}
        }

        /// <summary>
        /// Reset state, set matched inputImage to null
        /// </summary>
        public void Reset()
        {
            this.Gesture.Reset();
            matchedInputImage = null; //jp added this to solve all gestures == barrel roll bug
        }
    }
}