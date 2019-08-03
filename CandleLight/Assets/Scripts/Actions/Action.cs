/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Action class is used to take decisions on the UI.
* The action can be an attack, dialog option, a direction to travel in, or more, depending on the event.
*
*/

using ActionConstants = Constants.ActionConstants;
using Combat;
using Events;
using Localization;
using System.Collections;
using TMPro;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace Actions {
    public class Action : MonoBehaviour {

        /* external component references */
        public LocalizedText actionText;            /// <value> Component reference to text to be displayed </value>
        public CanvasGroup textCanvas;
        
        public Attack a { get; private set; }       /// <value> Attack stored if attack </value>
        public Button b { get; private set; }       /// <value> Button component </value>
        public Interaction i { get; private set; }  /// <value> Interaction stored if interraction </value>
        public string actionType { get; private set; }       /// <value> Action type (Attack, flee, undo) </value>
        public bool isUsable { get; private set; } = true;   /// <value> Flag for if action button is usable </value>

        /* internal component references */
        private ButtonTransitionState bts;          /// <value> Button's visual state controller </value>
        private Image img;                          /// <value> Button's sprite </value>
        private TextMeshProUGUI t;                  /// <value> Text display </value>
        
        private float lerpSpeed = 3f;               /// <value> Speed at which aciton text fades in </value>

        /// <summary>
        /// Awake to get all components attached to the Action button
        /// </summary>
        void Awake() {
            b = GetComponent<Button>();
            bts = GetComponent<ButtonTransitionState>();
            img = GetComponent<Image>();
            t = GetComponentInChildren<TextMeshProUGUI>();

            ColorBlock normalBlock = b.colors; 
            ColorBlock unusableBlock = b.colors;
            
            unusableBlock.normalColor = new Color32(196, 36, 48, 255);
            unusableBlock.highlightedColor = new Color32(255, 0, 64, 255);
            unusableBlock.pressedColor = new Color32(120, 36, 48, 255);
            unusableBlock.disabledColor = new Color32(120, 36, 48, 255);

            bts.SetColorBlock("normal", normalBlock);
            bts.SetColorBlock("normalAlternate", unusableBlock);
        }

        /// <summary>
        /// Sets the actionType
        /// </summary>
        /// <param name="actionType"> Type of action (flee, undo) </param>
        public void SetAction(string actionType) {
            if (actionType == ActionConstants.FLEE) {
                SetKey("flee_action");
            }
            else if (actionType == ActionConstants.TRAVEL) {
                SetKey("travel_action");
            }
            else if (actionType == ActionConstants.UNDO) {
                SetKey("undo_action");
            } 
            else if (actionType == ActionConstants.NONE) {
                SetKey("none_action");
            }

            this.a = null;
            this.i = null;
            this.actionType = actionType;
        }

        /// <summary>
        /// Sets the actionType. Overloaded to accept an attack specifically
        /// </summary>
        /// <param name="actionType"> Type of action (attack) </param>
        /// <param name="a"> Attack object to be stored </param>
        public void SetAction(string actionType, Attack a) {
            if (actionType == ActionConstants.ATTACK) {
                if (a.nameKey == "none_attack") {   // disable action if it does nothing
                    this.actionType = ActionConstants.NONE;
                    this.a = null;
                    this.i = null;
                } 
                else {
                    this.a = a;
                    this.i = null;
                    this.actionType = actionType;
                }
                SetKey(a.nameKey);
            }
        }

        /// <summary>
        /// Sets the actionType. Overloaded to accept an interaction specifically
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="i"></param>
        public void SetAction(string actionType, Interaction i) {
            if (actionType == ActionConstants.INTERACTION) {
                if (i.nameKey == "none_int") {
                    this.actionType = ActionConstants.NONE;
                    this.a = null;
                    this.i = null;
                }
                else {
                    this.i = i;
                    this.a = null;
                    this.actionType = actionType;
                }
                SetKey(i.nameKey);
            }
        }

        public void SetActionType(string actionType) {
            this.actionType = actionType;
        }

        /// <summary>
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
        public void SetInteractable(bool value) {
            b.interactable = value;
            img.raycastTarget = value;

            if (value == false && isUsable == true) {
                ShowActionUnselected();   
            }            
        }
        
        /// <summary>
        /// Checks if the action is interactable (can be clicked and selected)
        /// </summary>
        /// <returns> True if interactable, false otherwise</returns>
        public bool IsInteractable() {
            return b.interactable;
        }

        /// <summary>
        /// Change button colour to show it is selected after user navigates away from it after selecting
        /// </summary>
        public void ShowActionSelected() {
            bts.SetColor("pressed");
        }

        /// <summary>
        /// Change button colour back to default
        /// </summary>
        public void ShowActionUnselected() {
            bts.SetColor("normal");
        }

        /// <summary>
        /// Sets the button to usable (can be used by the player, nothing to do with the button's functionality).
        /// Colours the button to visually represent if its usable.
        /// </summary>
        /// <param name="value"> True if the button is usable, false otherwise </param>
        public void SetUsable(bool value) {
            isUsable = value;

            if (value == true) {
                bts.SetColor("normal");
            }
            else {
                bts.SetColor("normalAlternate");
            }
        }

        /// <summary>
        /// Fades the action text to the target alpha
        /// </summary>
        /// <param name="targetAlpha"> Int alpha must be 0 or 1 </param>
        /// <returns> IEnumerator to fade smoothly through frames </returns>
        public IEnumerator Fade(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = textCanvas.alpha;

            while (textCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                textCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Sets the text to be displayed
        /// </summary>
        /// <param name="key"> String key that corresponds to dictionary </param>
        public void SetKey(string key) {
            actionText.SetKey(key);
        }
    }
}
