using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SwordClash
{
    class ProjectileState : TentacleState
    {
        //initialize with another state, resuming coiled state
        public ProjectileState(TentacleState oldState, Vector2 swipeNormalVector, float swipeAngle)
            : base(oldState.TentaControllerInstance)
        {
            this.m_SwipeVelocityVector = swipeNormalVector;
            this.m_SwipeAngle = swipeAngle;
            this.m_BrollCount = 0;

            
            m_SwipeVelocityVector = MultiplyVectorComponentsBySpeed(
                m_SwipeVelocityVector,
                TentaControllerInstance.UPswipeSpeedConstant + TentaControllerInstance.UPswipeSpeedModifier
                );

            OnStateEnter();

        }

        //initialize from BarrelRollState which increments BrollCount as side-effect; BAD CODE
        public ProjectileState(TentacleState oldState, Vector2 swipeVelocityVector, float swipeAngle, 
            short BrollCount)
            : base(oldState.TentaControllerInstance)
        {
            this.m_SwipeVelocityVector = swipeVelocityVector;
            this.m_SwipeAngle = swipeAngle;
            //check if in bad range here???
            //TODO: fix tight coupling of Moving and Barrel Roll state
            this.m_BrollCount = BrollCount; 
            OnStateEnter();

        }

        private Vector2 m_SwipeVelocityVector;
        private float m_SwipeAngle;
        private short m_JukeCount;
        private short m_BrollCount;
       
        public override void OnStateEnter()
        {
            //Set actual tentacle movement vars, save the previous ones if needed
            //  not needed right now...
            LowerAllInputFlags();

            m_JukeCount = 0;
            

            
            
        }

        public override void OnStateExit()
        {
            //just teleport for now
            TentaControllerInstance.TT_RecoilTentacle();

            LowerAllInputFlags();
        }

        public override void ProcessState()
        {
            //Free to process here,
            IsCurrentlyProcessing = false;

            //Check if barrel roll flag and haven't already brolled too much
            if ((m_BrollCount < TentaControllerInstance.TimesCanBarrelRoll) &&
                (InputFlagArray[(int)HotInputs.BarrelRoll]))
            {
                //OnStateExit();
                TentaControllerInstance.CurrentTentacleState = new BarrelRollState(this, m_SwipeVelocityVector,
                    m_SwipeAngle, m_BrollCount);
            }
            

            //check if tapping after checking if tapped out
            if (m_JukeCount < TentaControllerInstance.TT_timesAllowedToJuke)
            {
                //if juke - right input received
                if (InputFlagArray[(int)HotInputs.RudderRight])
                {
                    //TODO: make seperate jump methods for coiled jumps
                    TentaControllerInstance.TT_JumpRight(); 
                    InputFlagArray[(int)HotInputs.RudderRight] = false;
                    ++m_JukeCount;
                }

                if (InputFlagArray[(int)HotInputs.RudderLeft])
                {
                    TentaControllerInstance.TT_JumpLeft();
                    InputFlagArray[(int)HotInputs.RudderLeft] = false;
                    ++m_JukeCount;
                }

            }

            //move tentacle tip
            TentaControllerInstance.TT_MoveTentacleTip(m_SwipeVelocityVector, m_SwipeAngle);

            //Check if done moving
            if (TentaControllerInstance.IsTentacleAtMaxExtension())
            {
                //TODO: Recovery mode state

                
                OnStateExit();
                TentaControllerInstance.CurrentTentacleState = new CoiledState(this);
            }

            
        }

        //private Vector2 MultiplyVectorYComponentBySpeed(Vector2 DirectionVector, float speed)
        //{
        //    //Return velocity vector with [x, y*=speed]
        //    return new Vector2(DirectionVector.x, DirectionVector.y * speed);
        //}

        private Vector2 MultiplyVectorComponentsBySpeed(Vector2 DirectionVector, float speed)
        {
            //Return velocity vector with [x*=speed, y*=speed]
            return new Vector2(DirectionVector.x * speed, DirectionVector.y * speed);
        }
    }
}
