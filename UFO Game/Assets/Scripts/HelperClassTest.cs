using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
     static class HelperClassTest
    {
        //Return string value made from a String followed by an int, with an optional end string
        static public string UpdateTextField(string begin, int value) {
            return begin + value.ToString();
        }
    }
}
