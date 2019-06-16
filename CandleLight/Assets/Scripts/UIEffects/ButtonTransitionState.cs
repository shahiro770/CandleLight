/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* The ButtonTransiionState class is used to manage the displayed sprite/colouring of a button.
* Unity button states (normal, hovered, pressed, disabled) are sometimes needed to persist.
* This class allows a user to change the normal sprite/colouring to match a specific state so it
* visibly persists, even after the action that put the button in that state is gone.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIEffects {

    public class ButtonTransitionState : MonoBehaviour {
        
        private Button b;                           /// <value> Button to have its state managed </value>
        private Image i;                            /// <value> Image is used to get a button's default sprite </value>
        private ColorBlock normalBlock;             /// <value> Default colour block </value>
        private ColorBlock normalAlternateBlock;    /// <value> Alternate normal colour block for buttons that have two sets of colours (e.g. "false enabled" and disabled) </value>
        private ColorBlock highlightedBlock;        /// <value> Hovered colour block </value>
        private ColorBlock pressedBlock;            /// <value> Clicked colour block </value>
        private ColorBlock disabledBlock;           /// <value> Disabled colour block </value>
        private Sprite normalSprite;                /// <value> Default sprite </value>
        private SpriteState normalState;            /// <value> Default sprite state  </value>
        private SpriteState highlightedState;       /// <value> Hovered sprite state </value>
        private SpriteState pressedState;           /// <value> Clicked sprite state </value>
        private SpriteState disabledState;          /// <value> Disabled sprite state </value>

        /// <summary>
        /// Initializes all the relevant colour blocks/sprite states depending on the button's transition type
        /// </summary>
        void Awake() {
            b = GetComponent<Button>();
            i = GetComponent<Image>();

            if (b.transition == Selectable.Transition.ColorTint) {
                ColorBlock initial = b.colors;
                normalBlock = initial;
                highlightedBlock = initial;
                pressedBlock =  initial;
                disabledBlock = initial;

                pressedBlock.normalColor = pressedBlock.pressedColor;
                pressedBlock.highlightedColor = pressedBlock.pressedColor;
                pressedBlock.disabledColor = pressedBlock.pressedColor;

                disabledBlock.normalColor = disabledBlock.disabledColor;
                disabledBlock.highlightedColor = disabledBlock.disabledColor;
                disabledBlock.pressedColor = disabledBlock.disabledColor;
            }
            else if (b.transition == Selectable.Transition.SpriteSwap) {
                SpriteState initial = b.spriteState;
                normalState = initial;
                highlightedState = initial;
                pressedState = initial;
                disabledState = initial;

                normalSprite = i.sprite;
                pressedState.highlightedSprite = initial.pressedSprite;
                disabledState.highlightedSprite = initial.disabledSprite;
            }
        }

        /// <summary>
        /// Sets the colour of the button depending on the type
        /// </summary>
        /// <param name="type"> String indicating which colour to become the normal colour </param>
        public void SetColor(string type) {
            if (type == "normal") {
                b.colors = normalBlock;
            }
            else if (type == "normalAlternate") { 
                b.colors = normalAlternateBlock;
            }
            else if (type == "highlighted") {
                b.colors = highlightedBlock;
            }
            else if (type == "pressed") {
                b.colors = pressedBlock;
            }
            else if (type == "disabled") {
                b.colors = disabledBlock;
            } else {
                Debug.Log("invalid type");
            }
        }

        /// <summary>
        /// Sets a state's colour block to an inputted colour block
        /// </summary>
        /// <param name="blockType"> Which block to change </param>
        /// <param name="cb"> The inputted colour block </param>
        public void SetColorBlock(string blockType, ColorBlock cb) {
            if (blockType == "normal") {
                normalBlock = cb;
            }
            else if (blockType == "normalAlternate") {
                normalAlternateBlock = cb;
            }
            else {
                Debug.Log("invalid type");
            }
        }

        /// <summary>
        /// Sets the sprite of the button depending on the type
        /// </summary>
        /// <param name="type"> String indicating which sprite to become the normal sprite </param>
        public void SetSprite(string type) {
            if (type == "normal") {
                i.sprite = normalSprite;
                b.spriteState = normalState;
            }
            else if (type == "highlighted") {
                b.spriteState = highlightedState;
            }
            else if (type == "pressed") {
                i.sprite = pressedState.pressedSprite;
                b.spriteState = pressedState;
            }
            else if (type == "disabled") {
                i.sprite = disabledState.disabledSprite;
                b.spriteState = disabledState;
            } else {
                Debug.Log("invalid type");
            }
        }
    }
}
