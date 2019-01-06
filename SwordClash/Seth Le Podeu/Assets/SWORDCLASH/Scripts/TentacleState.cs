using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwordClash
{
    //Sadly has to be abandoned??? game updates can only happen in Monobehavior Update() and FixedUpdate()...
    //Fighting unity's architecture will only lead to endless pain. A dictionary of booleans passed around each frame... seems bad...

    //Abstract state class, attempting to fit the OO gang of four 'state' pattern into Unity
    //instead of the traditional procedural giant case statement over enum states.
    //Each state is own class, responsible for its own transitions.
    //Context keeps a reference to current state in TentacleController.cs instance
    public abstract class TentacleState
    {
        public TentacleController tentaControllerInstance;

        ///*raised flags == true; means that player controller has received a gesture and set the appropriate values, flags, scalars, coords, etc.
        //* The tentaclecontroller responds in different ways to the same inputs (same flags) depending on tentacle state machine, see Game Design Doc. 
        //     */
        //private bool m_RudderRight_Flag; //Player Controller 1:1 dependency on Tentacle Controller; Flags mean player controller has received input corresponding to flag
        //private bool m_RudderLeft_Flag;
        //private bool m_BarrelRoll_Flag; //Double tap (or circle gesture) sent by player controller recently == true
        //private bool m_ReelBack_Flag; //downward swipe toward player flag, 'reels' in tentacle back to coiled state.


        //mutex / semaphore???
        protected bool m_Is_Currently_Processing;

        //Inputs received each frame of physics FixedUpdate (for now)
        public enum InputFlag_Enum
        {
            RudderRight = 0,
            RudderLeft = 1,
            BarrelRoll = 2,
            ReelBack = 3,
            LaunchSwipe = 4
        };

        /* //https://stackoverflow.com/questions/8447/what-does-the-flags-enum-attribute-mean-in-c
         * [Flags]
public enum MyEnum
{
    None   = 0,
    First  = 1 << 0,
    Second = 1 << 1,
    Third  = 1 << 2,
    Fourth = 1 << 3
}
         * */


        //Not sure about best data structure for list of bools; hashtable probably better.
        //see also HybridDictionary and other .NET dictionarys that work well with simple value types.
        //protected Dictionary<InputFlag_Enum, bool> m_FlagDict;

        protected bool[] m_InputFlagArray;
        protected int m_InputFlagCount; // number of InputEnum values

        //public bool RudderRight_Flag
        //{
        //    get
        //    {
        //        return m_RudderRight_Flag;
        //    }

        //    protected set
        //    {
        //        m_RudderRight_Flag = value;
        //    }
        //}

        //public bool RudderLeft_Flag
        //{
        //    get
        //    {
        //        return m_RudderLeft_Flag;
        //    }

        //    protected set
        //    {
        //        m_RudderLeft_Flag = value;
        //    }
        //}

        //public bool BarrelRoll_Flag
        //{
        //    get
        //    {
        //        return m_BarrelRoll_Flag;
        //    }

        //    protected set
        //    {
        //        m_BarrelRoll_Flag = value;
        //    }
        //}

        //public bool ReelBack_Flag
        //{
        //    get
        //    {
        //        return m_ReelBack_Flag;
        //    }

        //    protected set
        //    {
        //        m_ReelBack_Flag = value;
        //    }
        //}

        ////Sets all flags in collection, hardcoded in tentacleController for now, to false, returns number of flags set.
        //public int SetAllFlagsFalse()
        //{
        //    int numFlagsSet = 0;

        //    //  for each flag, flag.false();
        //    foreach (var flagKey in m_FlagDict.Keys)
        //    {
        //        m_FlagDict[flagKey] = false;
        //        numFlagsSet += 1;
        //    }
        //    return numFlagsSet;
        //}

        public void LowerAllInputFlags()
        {
            for (int i = 0; i < m_InputFlagCount; i++)
            {
                m_InputFlagArray[i] = false;
            }
        }

        //public int AddAllFlagstoDict(Dictionary<InputFlag_Enum, bool> flagDict)
        //{
        //    int numberFlagsAdded = 0;
        //    int startingFlagDictCount = flagDict.Count;

        //    flagDict.Add(InputFlag_Enum.RudderRight, RudderRight_Flag);
        //    flagDict.Add(InputFlag_Enum.RudderLeft, RudderLeft_Flag);
        //    flagDict.Add(InputFlag_Enum.BarrelRoll, BarrelRoll_Flag);
        //    flagDict.Add(InputFlag_Enum.ReelBack, ReelBack_Flag);

        //    numberFlagsAdded = startingFlagDictCount - flagDict.Count;
        //    return numberFlagsAdded;
        //}

        //Used by other classes to attempt to raise a flag.
        //returns true if flag was raised false if currently processing state
        //please pass in InputFlag_Enum.FlagValue as parameter
        //assume InputFlag_Enum starts at 0
        public bool RaiseTentacleFlag_Request(int requestedFlagtoRaise)
        {
            bool yesFlagRaised = false;

            //only try to raise if: requested flag is inside InputFlagEnum range
            if (requestedFlagtoRaise >= 0 && requestedFlagtoRaise < m_InputFlagCount)
            {
                if (m_Is_Currently_Processing == false)
                {
                    //TODO: try catch here?
                    m_InputFlagArray[requestedFlagtoRaise] = true;
                    yesFlagRaised = true;
                }
            }
            return yesFlagRaised;
        }

        protected bool RaiseTentacleFlag_Force(int requestedFlagtoRaise)
        {
            bool yesFlagRaised = false;

            //only try to raise if: requested flag is inside InputFlagEnum range
            if (requestedFlagtoRaise >= 0 && requestedFlagtoRaise < m_InputFlagCount)
            {
                    m_InputFlagArray[requestedFlagtoRaise] = true;
                    yesFlagRaised = true;
            }

            return yesFlagRaised;
        }

        protected bool LowerTentacleFlag_Force(int requestedFlagtoLower)
        {
            bool yesFlagLowered = false;

            //only try to raise if: requested flag is inside InputFlagEnum range
            if (requestedFlagtoLower >= 0 && requestedFlagtoLower < m_InputFlagCount)
            {
                m_InputFlagArray[requestedFlagtoLower] = false;
                yesFlagLowered = true;
            }
            return yesFlagLowered;
        }

        //constructor needed? //would this be pass by ref NOT value right?
        public TentacleState(TentacleController tc)
        {
            tentaControllerInstance = tc;
            m_Is_Currently_Processing = false;

            //initialize input flag array to length of InputFlag_Enum, default value is false.
            m_InputFlagCount = Enum.GetNames(typeof(InputFlag_Enum)).Length;
            m_InputFlagArray = new bool[m_InputFlagCount];

            //OnStateEnter(); not working if abstract method; must be VIRTUAL to work
        }

        //public virtual void OnStateEnter() { }
        //public virtual void OnStateExit() { }

        public abstract void OnStateEnter();
        public abstract void OnStateExit();
        public abstract void ProcessState();
    }



}
