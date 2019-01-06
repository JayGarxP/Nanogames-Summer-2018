using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwordClash
{
    //private??? possible this could be a static state, since all instances of it are the same...
    public class CoiledState : TentacleState
    {

        //initialize with another state, resuming coiled state
        public CoiledState(TentacleState oldState)
            :base(oldState.tentaControllerInstance)
        {
            OnStateEnter();

        }

        //initialize with new tentacle controller, first coil of game
        public CoiledState(TentacleController tsc) : base(tsc)
        {
            //m_FlagDict = new Dictionary<InputFlag_Enum, bool>();
            //AddAllFlagstoDict(m_FlagDict);

            OnStateEnter();
        }

        public override void OnStateEnter()
        {
            //set all flags false
            LowerAllInputFlags();
        }

        public override void OnStateExit()
        {
            //throw new NotImplementedException();
            
        }


        // compile Sandcastle XML comments with: -doc:DocFileName.xml 

        /// <summary>
        /// Processes InputFlags and makes callbacks to TentacleController move thangs
        /// </summary>
        /// <remarks>
        /// Called inside Unity update loops, CoiledState.ProcessState() does not block inputflag sets
        /// </remarks>
        public override void ProcessState()
        {
            //Coiled States ProcessState does NOT block flag changes while executing.
            m_Is_Currently_Processing = false;

            //if player up-swipe, they tryna  *L A U N C H*
            if (m_InputFlagArray[(int)InputFlag_Enum.LaunchSwipe])
            {
                //OnStateExit();
                tentaControllerInstance.CurrentTentacleState = new ProjectileState(this,
                    tentaControllerInstance.movePositionVelocity_TT_Requested,
                    tentaControllerInstance.moveRotationAngle_TT_Requested);
            }

            //if juke-right input received
            if (m_InputFlagArray[(int)InputFlag_Enum.RudderRight])
            {
                tentaControllerInstance.TT_JumpRight(); //TODO: make seperate jump methods for coiled jumps
                m_InputFlagArray[(int)InputFlag_Enum.RudderRight] = false;
            }
            else if (m_InputFlagArray[(int)InputFlag_Enum.RudderLeft])
            {
                tentaControllerInstance.TT_JumpLeft();
                m_InputFlagArray[(int)InputFlag_Enum.RudderLeft] = false;

            }

            ////TODO: Make Barrel Roll OWN state
            //if (BarrelRoll_Flag)
            //{
            //    //tentaControllerInstance.Do_A_BarrelRoll();
            //}
            }

    }

   

}
