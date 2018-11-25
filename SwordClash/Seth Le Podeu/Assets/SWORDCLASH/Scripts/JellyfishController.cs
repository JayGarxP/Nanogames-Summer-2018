using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordClash
{
    public class JellyfishController : MonoBehaviour
    {
        //OnCollisionEnter() is event Publisher for trigger collision (hit jellyfish); Subscriber to collision event is tentacleController (where state machine will be implemented)
        public event EventHandler JellyfishHitByTentacleTip_Event;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

     

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Raise collide event, subscribed to in TentacleController
            JellyfishHitByTentacleTip_Event(this, EventArgs.Empty);

            ////the subscriber class needs a reference to the publisher class in order to subscribe to its events.
            //void HandleCustomEvent(object sender, CustomEventArgs a)
            //{
            //    // Do something useful here.  
            //}

            //Subscripe +=; Unsub -=;
            //publisher.RaiseCustomEvent += HandleCustomEvent;  
            //  publisher.RaiseCustomEvent -= HandleCustomEvent; 

        }





    }
}
