/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* ClassButtons are UI Button that the user can click to choose their starting class.
* This class button changes visually to represent when it is clicked.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClassButton : MonoBehaviour {
    
    public Button b;                                    /// <value> Button component of the attached GO </value>
    
    public string classString;                          /// <value> String of class, can't be a property because unity won't allow inspector access </value>    
    public bool isReady { get; private set; } = false;  /// <value> Flag for when button is ready for other scripts to reference </value>
    
    private ButtonTransitionState bts;      /// <value> Button's visual state controller </value>
    private string classDescription;        /// <value> Description of class  </value>

    /// <summary>
    /// Awake to intialize bts and let other scripts know the button isReady
    /// </summary> 
    void Awake() {
        bts = b.GetComponent<ButtonTransitionState>();
        isReady = true;
    }

    /// <summary>
    /// Call the button's OnSelect
    /// This is done so the classButton can be treated as a wrapper for the normal button script
    /// </summary> 
    /// <param name="eventData"> Data for event system to read </param>
    public void OnSelect(BaseEventData eventData) {
        b.OnSelect(eventData);
    }
    
    /// <summary>
    /// Update the button's sprite state when selected or not selected 
    /// </summary> 
    /// <param name="type"> Type of the sprite </param>
    /// <seealso> ButtonTransitionState </seealso>
    public void SetSprite(string type) {
        bts.SetSprite(type);
    }

    /// <summary>
    /// Returns the class string
    /// </summary>
    /// <returns> String containing the name of the class selected </returns>
    public string GetClassString() {
        return classString;
    }
}