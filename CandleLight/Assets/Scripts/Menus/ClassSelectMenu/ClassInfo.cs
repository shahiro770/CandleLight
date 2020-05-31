/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* ClassInfo holds the text descriptions of the various classes. It allows for the description
* to be set or cleared.
*
*/

using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus.ClassSelectMenu {

    public class ClassInfo : MonoBehaviour {
        
        /* external component references */
        public LocalizedText[] classInfo;      /// <value> Reference to localized text </value>
        
        /// <summary>
        /// Change the displayed class description
        /// </summary>
        /// <param name="classString"> Changes the class description depending on the class. "" to clear </param>
        public void SetClassInfo(string classString) {
            if (classString == "") {
                classInfo[0].Clear();
                classInfo[1].Clear();
            }
            else {
                classInfo[0].SetKey(classString + "_title");
                classInfo[1].SetKey(classString + "_description");
            }
        }
    }
}
