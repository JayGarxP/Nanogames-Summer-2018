using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SwordClash
{
    //End goal is to get players to paint/trace patterns on the glass, like fighting game inputs
    class BarrelRollState : TentacleState
    {
        //Same members across multiple states, could put into abstract base class,
        //  could also create object pool for natural playing of game (pool of 10 states or so)
        //  worried about memory hogging / fragmentations from recreating vectors over and over, but
        //      maybe the constructor uses a reference somehow??? Not sure how it works???????
        private Vector2 m_SwipeVelocityVector;
        private float m_SwipeAngle;
        //BAD CODE; two concrete classes share same variables: put in base class???
        //  maintain object/fields in Controller that tracts these variables?
        //  Should barrel roll be sub-state of projectile somehow??? then lose collision code...
        private short m_BrollCount;
       

        private float m_CurrentBrollDegreesRotated;

        public BarrelRollState(TentacleState oldState, Vector2 swipeVelocityVector, float swipeAngle, short brollCount)
              : base(oldState.TentaControllerInstance)
        {
            this.m_SwipeVelocityVector = swipeVelocityVector;
            this.m_SwipeAngle = swipeAngle;
            this.m_BrollCount = brollCount;
            OnStateEnter();

        }

        public override void OnStateEnter()
        {
            m_CurrentBrollDegreesRotated = 0.0f;
            LowerAllInputFlags();
        }

        public override void OnStateExit()
        {
            LowerAllInputFlags();
        }

        //Rotate, move at half speed while rotating, check if done rotating to return to moving state pass in barrelRoll
        //don't check any input flags, don't process any bad collision events, still do good ones tho
        public override void ProcessState()
        {
            //NOT Free to process here!
            IsCurrentlyProcessing = true;

            m_CurrentBrollDegreesRotated = TentaControllerInstance.BarrelRollin_rotate(m_CurrentBrollDegreesRotated);

            //still move, but more slowly
            TentaControllerInstance.TT_MoveTentacleTip_WhileBroll(m_SwipeVelocityVector);


            //If the barrelroll is over; the total spin 360, 720, etc. has been overcome by degrees of rotation per frame
            if (m_CurrentBrollDegreesRotated >= TentaControllerInstance.BROLLEndSpinRotationDegrees)
            {
                TentaControllerInstance.ResetTentacleTipRotation();
                OnStateExit();
                //increment barrel roll count
                m_BrollCount++;
                TentaControllerInstance.CurrentTentacleState = new ProjectileState(this, m_SwipeVelocityVector, m_SwipeAngle, m_BrollCount);
            }
        }
    }
}
