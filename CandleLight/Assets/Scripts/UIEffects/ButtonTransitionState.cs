﻿/*
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

        public bool autoInitialize = true;          /// <value> Some btss don't initialize in time during awake unless done manually </value>
        
        private Button b;                           /// <value> Button to have its state managed </value>
        private Image i;                            /// <value> Image is used to get a button's default sprite </value>
        private ColorBlock normalBlock;             /// <value> Default colour block </value>
        private ColorBlock naBlock0;               /// <value> Normal alternate colour block for buttons that have two sets of colours (e.g. "false enabled" and disabled) </value>
        private ColorBlock naBlock1;
        private ColorBlock naBlock2;
        private ColorBlock naBlock3;
        private ColorBlock naBlock4;
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

            if (autoInitialize == true) {
                if (b.transition == Selectable.Transition.ColorTint) {
                    ColorBlock initial = b.colors;
                    normalBlock = initial;
                    highlightedBlock = initial;
                    pressedBlock =  initial;
                    disabledBlock = initial;

                    highlightedBlock.normalColor = pressedBlock.pressedColor;

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
        }

        /// <summary>
        /// Sets the colour block of the button depending on the type
        /// </summary>
        /// <param name="type"> String indicating which colour to become the normal colour </param>
        public void SetColor(string type) {
            if (b == null) {                // hack because for skillDisplays, awake doesn't happen in time
                b = GetComponent<Button>();
            }
            if (type == "normal") {     
                b.colors = normalBlock;
            }
            else if (type == "na0") { 
                b.colors = naBlock0;
            }
            else if (type == "na1") { 
                b.colors = naBlock1;
            }
            else if (type == "na2") { 
                b.colors = naBlock2;
            }
            else if (type == "na3") { 
                b.colors = naBlock3;
            }
            else if (type == "na4") { 
                b.colors = naBlock4;
            }
            else if (type == "highlighted") {
                b.colors = highlightedBlock;
            }
            else if (type == "pressed") {
                b.colors = pressedBlock;
            }
            else if (type == "disabled") {
                b.colors = disabledBlock;
            } 
            else {
                Debug.LogError("Invalid button colour type");
            }
        }

        /// <summary>
        /// Sets the colour block of the button depending on the type, using ints for marginally faster times
        /// </summary>
        /// <param name="type"> String indicating which colour to become the normal colour </param>
        public void SetColor(int type) {
            if (b == null) {                // hack because for skillDisplays, awake doesn't happen in time
                b = GetComponent<Button>();
            }
            if (type == 0) {     
                b.colors = normalBlock;
            }
            else if (type == 1) { 
                b.colors = naBlock0;
            }
            else if (type == 2) { 
                b.colors = naBlock1;
            }
            else if (type == 3) { 
                b.colors = naBlock2;
            }
            else if (type == 4) { 
                b.colors = naBlock3;
            }
            else if (type == 5) { 
                b.colors = naBlock4;
            }
            else if (type == 6) {
                b.colors = highlightedBlock;
            }
            else if (type == 7) {
                b.colors = pressedBlock;
            }
            else if (type == 8) {
                b.colors = disabledBlock;
            } 
            else {
                Debug.LogError("Invalid button colour type");
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
            else if (blockType == "highlighted") {
                highlightedBlock = cb;
            }
            else if (blockType == "pressed") {
                pressedBlock = cb;
            }
            else if (blockType == "na0") {
                naBlock0 = cb;
            }
            else if (blockType == "na1") {
                naBlock1 = cb;
            }
            else if (blockType == "na2") {
                naBlock2 = cb;
            }
            else if (blockType == "na3") {
                naBlock3 = cb;
            }
            else if (blockType == "na4") {
                naBlock4 = cb;
            }
            else if (blockType == "disabled") {
                disabledBlock = cb;
            }
            else {
                Debug.LogError("Invalid button block type");
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
            } 
            else {
                Debug.LogError("Invalid button sprite type");
            }
        }
    }
}
