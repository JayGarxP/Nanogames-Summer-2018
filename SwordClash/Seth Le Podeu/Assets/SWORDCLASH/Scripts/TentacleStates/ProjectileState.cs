using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SwordClash
{
    class ProjectileState : TentacleState
    {
        /// <summary>  
        ///  Constructor to Initialize this state with another, (transition from coiled probably)
        ///  <para> 
        ///  Sets SwipeVelocityVector from swipeNormalVector using MultipyVectorComponentBySpeed()
        ///  </para>
        /// </summary>  
        /// <param name="oldState">CoiledState for example;;;</param>
        ///  <para>
        ///  <param name="swipeNormalVector">Unit vector (vector with magnitude of 1) 
        ///  representing swipe direction</param>
        ///  <param name="swipeAngle">Atan unit circle units -90.0f == Unity.RigidBody2D.Rotation angle,
        ///  responsibility of caller to convert angle to proper range</param>
        ///  </para>
        public ProjectileState(TentacleState oldState, Vector2 swipeNormalVector, float swipeAngle)
            : base(oldState.TentaControllerInstance)
        {
            this.SwipeVelocityVector = swipeNormalVector;
            this.SwipeAngle = swipeAngle;
            this.BarrelRollCount = 0;
            
            SwipeVelocityVector = MultiplyVectorComponentsBySpeed(
                SwipeVelocityVector,
                TentaControllerInstance.UPswipeSpeedConstant + TentaControllerInstance.UPswipeSpeedModifier
                );

            OnStateEnter();
        }

        // initialize from BarrelRollState which increments BrollCount as side-effect; BAD CODE
        public ProjectileState(TentacleState oldState, Vector2 swipeVelocityVector, float swipeAngle, 
            short BrollCount)
            : base(oldState.TentaControllerInstance)
        {
            this.SwipeVelocityVector = swipeVelocityVector;
            this.SwipeAngle = swipeAngle;
            //check if in bad range here???
            //TODO: fix tight coupling of Moving and Barrel Roll state
            this.BarrelRollCount = BrollCount; 
            OnStateEnter();

        }

        private Vector2 SwipeVelocityVector;
        private float SwipeAngle;
        private short JukeCount;
        private short BarrelRollCount;
       
        // Lower all inputflags set times juked count to zero
        public override void OnStateEnter()
        {
            // Set actual tentacle movement vars, save the previous ones if needed
            //  not needed right now...
            LowerAllInputFlags();
            JukeCount = 0;
        }

        // Recoil Tentacle and lower all input flags.
        public override void OnStateExit()
        {
            //just teleport for now
            TentaControllerInstance.PleaseRecoilTentacle();
            LowerAllInputFlags();
        }

        // ProjectileState hits many things
        public override void HandleCollisionByTag(string ObjectHitTag)
        {
            // Get stung and change sprite + recover
            if (ObjectHitTag == JellyfishEnemyGameObjectTag)
            {
                // Change color/ZAP! also go into recovery state
                TentaControllerInstance.PleaseStingTentacleSprite();
            }
            //else if (ObjectHitTag )
            //{

            //}
        }

        // WIP, See Game Design Doc for ProcessState's transition table
        // code summary here for projectile.processstate()
        public override void ProcessState()
        {
            // Free to process here,
            IsCurrentlyProcessing = false;

            // Check if barrel roll flag and haven't already brolled too much
            if ((BarrelRollCount < TentaControllerInstance.TimesCanBarrelRoll) &&
                (InputFlagArray[(int)HotInputs.BarrelRoll]))
            {
                TentaControllerInstance.CurrentTentacleState = new BarrelRollState(this, SwipeVelocityVector,
                    SwipeAngle, BarrelRollCount);
            }
            

            // check if tapping after checking if tapped out
            if (JukeCount < TentaControllerInstance.TTTimesAllowedToJuke)
            {
                // if juke - right input received
                if (InputFlagArray[(int)HotInputs.RudderRight])
                {
                    //TODO: make seperate jump methods for coiled jumps
                    TentaControllerInstance.TT_JumpRight(); 
                    InputFlagArray[(int)HotInputs.RudderRight] = false;
                    ++JukeCount;
                }

                if (InputFlagArray[(int)HotInputs.RudderLeft])
                {
                    TentaControllerInstance.TT_JumpLeft();
                    InputFlagArray[(int)HotInputs.RudderLeft] = false;
                    ++JukeCount;
                }

            }

            // move tentacle tip
            TentaControllerInstance.TT_MoveTentacleTip(SwipeVelocityVector, SwipeAngle);

            // Check if done moving
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
