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
            : base(oldState.tentaControllerInstance)
        {
            this.m_SwipeVelocityVector = swipeNormalVector;
            this.m_SwipeAngle = swipeAngle;
            this.m_BrollCount = 0;

            
            m_SwipeVelocityVector = MultiplyVectorComponentsBySpeed(
                m_SwipeVelocityVector,
                tentaControllerInstance.UPswipeSpeedConstant + tentaControllerInstance.UPswipeSpeedModifier
                );

            OnStateEnter();

        }

        //initialize from BarrelRollState which increments BrollCount as side-effect; BAD CODE
        public ProjectileState(TentacleState oldState, Vector2 swipeVelocityVector, float swipeAngle, 
            short BrollCount)
            : base(oldState.tentaControllerInstance)
        {
            this.m_SwipeVelocityVector = swipeVelocityVector;
            this.m_SwipeAngle = swipeAngle;
            //check if in bad range here???
            this.m_BrollCount = BrollCount; //TODO: fix tight coupling of Moving and Barrel Roll state
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
            tentaControllerInstance.TT_RecoilTentacle();

            LowerAllInputFlags();
        }

        public override void ProcessState()
        {
            //Free to process here,
            m_Is_Currently_Processing = false;

            //Check if barrel roll flag and haven't already brolled too much
            if (m_BrollCount < tentaControllerInstance.TimesCanBarrelRoll  &&  m_InputFlagArray[(int)InputFlag_Enum.BarrelRoll])
            {
                //OnStateExit();
                tentaControllerInstance.CurrentTentacleState = new BarrelRollState(this, m_SwipeVelocityVector,
                    m_SwipeAngle, m_BrollCount);
            }
            

            //check if tapping after checking if tapped out
            if (m_JukeCount < tentaControllerInstance.TT_timesAllowedToJuke)
            {
                //if juke - right input received
                if (m_InputFlagArray[(int)InputFlag_Enum.RudderRight])
                {
                    tentaControllerInstance.TT_JumpRight(); //TODO: make seperate jump methods for coiled jumps
                    m_InputFlagArray[(int)InputFlag_Enum.RudderRight] = false;
                    ++m_JukeCount;
                }

                if (m_InputFlagArray[(int)InputFlag_Enum.RudderLeft])
                {
                    tentaControllerInstance.TT_JumpLeft();
                    m_InputFlagArray[(int)InputFlag_Enum.RudderLeft] = false;
                    ++m_JukeCount;
                }

            }

            //move tentacle tip
            tentaControllerInstance.TT_MoveTentacleTip(m_SwipeVelocityVector, m_SwipeAngle);

            //Check if done moving
            if (tentaControllerInstance.IsTentacleAtMaxExtension())
            {
                //TODO: Recovery mode state

                
                OnStateExit();
                tentaControllerInstance.CurrentTentacleState = new CoiledState(this);
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
