/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Action class is used to take decisions on the UI.
* The action can be an attack, interaction, choose a direction to travel in, or more, depending on the event.
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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class Action : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        /* external component references */
        public ActionsPanel actionsPanel;
        public LocalizedText actionText;            /// <value> Text to be displayed </value>
        public CanvasGroup textCanvas;              /// <value> Canvas group for fading </value>
        public EventDescription eventDescription;   /// <value> Some actions will display what they do in the eventDescription </value>

        public Attack a { get; private set; }               /// <value> Attack stored if attack </value>
        public Button b { get; private set; }               /// <value> Button component </value>
        public Interaction i { get; private set; }          /// <value> Interaction stored if interraction </value>
        public string actionType { get; private set; }      /// <value> Action type (see ActionConstants) </value>
        public bool isUsable { get; private set; } = true;  /// <value> Flag for if action button is usable </value>

        /* internal component references */
        private ButtonTransitionState bts;          /// <value> Button's visual state controller </value>
        private Image img;                          /// <value> Button's sprite </value>
        private TextMeshProUGUI t;                  /// <value> Text display </value>
        
        private string btsBlockName = "";
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
            ColorBlock highlightedBlock = b.colors;
            ColorBlock disabledBlock = b.colors;
            ColorBlock STRBlock = b.colors;
            ColorBlock DEXBlock = b.colors;
            ColorBlock INTBlock = b.colors;
            ColorBlock LUKBlock = b.colors;
            
            unusableBlock.normalColor = new Color32(196, 36, 48, 255);
            unusableBlock.highlightedColor = new Color32(255, 0, 64, 255);
            unusableBlock.pressedColor = new Color32(120, 36, 48, 255);
            unusableBlock.disabledColor = new Color32(120, 36, 48, 255);

            disabledBlock.normalColor = disabledBlock.disabledColor;
            disabledBlock.highlightedColor = disabledBlock.disabledColor;
            disabledBlock.pressedColor = disabledBlock.disabledColor;

            highlightedBlock.normalColor = normalBlock.pressedColor;

            STRBlock.normalColor = new Color32(185, 29, 0, 125);
            STRBlock.highlightedColor = new Color32(185, 29, 0, 175);
            STRBlock.pressedColor = new Color32(185, 29, 0, 255);
            STRBlock.disabledColor = new Color32(61, 61, 61, 255);

            DEXBlock.normalColor = new Color32(90, 197, 79, 125);
            DEXBlock.highlightedColor = new Color32(90, 197, 79, 175);
            DEXBlock.pressedColor = new Color32(90, 197, 79, 255);
            DEXBlock.disabledColor = new Color32(90, 197, 79, 255);

            INTBlock.normalColor = new Color32(0, 152, 220, 125);
            INTBlock.highlightedColor = new Color32(0, 152, 220, 175);
            INTBlock.pressedColor = new Color32(0, 152, 220, 255);
            INTBlock.disabledColor = new Color32(61, 61, 61, 255);

            LUKBlock.normalColor = new Color32(255, 205, 2, 125);
            LUKBlock.highlightedColor = new Color32(255, 205, 2, 175);
            LUKBlock.pressedColor = new Color32(255, 205, 2, 255);
            LUKBlock.disabledColor = new Color32(61, 61, 61, 255);

            bts.SetColorBlock("normal", normalBlock);
            bts.SetColorBlock("na0", unusableBlock);
            bts.SetColorBlock("na1", STRBlock);
            bts.SetColorBlock("na2", DEXBlock);
            bts.SetColorBlock("na3", INTBlock);
            bts.SetColorBlock("na4", LUKBlock);
            bts.SetColorBlock("highlighted", highlightedBlock);
            bts.SetColorBlock("disabled", disabledBlock);
        }

        /// <summary>
        /// Sets the actionType
        /// </summary>
        /// <param name="actionType"> Type of action (must be ActionConstants type) </param>
        public void SetAction(string actionType) {
            if (actionType == ActionConstants.FLEE) {
                SetKey("flee_action");
            }
            else if (actionType == ActionConstants.TAKEALL) {
                SetKey("take_all_action");
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
            if (a.nameKey == "none_attack") {   // disable action if it does nothing
                this.actionType = ActionConstants.NONE;
                this.a = null;
                this.i = null;
            } 
            else {
                this.a = a;
                this.i = null;
                this.actionType = actionType;   // ActionConstants.ATTACK
            }
            SetKey(a.nameKey);
        }

        /// <summary>
        /// Sets the actionType. Overloaded to accept an interaction specifically
        /// </summary>
        /// <param name="actionType"> Type of action (interaction) </param>
        /// <param name="i"> Interaction object to be stored </param>
        public void SetAction(string actionType, Interaction i) {
            if (i.nameKey == "none_int") {
                this.actionType = ActionConstants.NONE;
                this.a = null;
                this.i = null;
            }
            else {
                this.i = i;
                this.a = null;
                this.actionType = actionType;   // ActionConstants.INTERACTION
            }
            SetKey(i.nameKey);    
        }

        /// <summary>
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
        public void SetInteractable(bool value) {
            b.interactable = value;
            img.raycastTarget = value;

            if (value == false && isUsable == true) {
                ShowActionDisabled();
            }
            else if (value == true && isUsable == false) {
                SetUsable(false);
            }      
            else if (value == true) {
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
            bts.SetColor("highlighted");
            btsBlockName = "highlighted";
        }

        /// <summary>
        /// Change button colour back to default
        /// </summary>
        public void ShowActionUnselected() {
            if (isUsable == true) {
                if (btsBlockName == "highlighted" || btsBlockName == "unusable" || btsBlockName == "na0" || btsBlockName == "disabled" ) {
                    bts.SetColor("normal");
                    btsBlockName = "normal";
                }
            }
            else {
                bts.SetColor("na0");
                btsBlockName = "na0";
            }
        }

        /// <summary>
        /// Change button colour back to default
        /// </summary>
        public void ShowActionDisabled() {
            bts.SetColor("disabled");
            btsBlockName = "disabled";
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
                btsBlockName = "normal";
            }
            else {
                bts.SetColor("na0");
                btsBlockName = "na0";
            }
        }

        /// <summary>
        /// Sets the colour of an interaction based off a stat check indicator
        /// </summary>
        /// <param name="value"> Check indicator value </param>
        public void SetCheckColor(int value) {
            bts.SetColor("na" + value);
            btsBlockName = "na" + value;
        }

        /// <summary>
        /// Sets the text to be displayed
        /// </summary>
        /// <param name="key"> String key that corresponds to dictionary </param>
        public void SetKey(string key) {
            actionText.SetKey(key);
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

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (b.interactable && a != null) {
                if (Input.GetButton("Shift") == true) {
                    eventDescription.SetFormulaText(isUsable, a);
                }
                else { 
                    eventDescription.SetAttackText(a, isUsable);
                }
            }
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData) {
            if (b.interactable && a != null) {
                if (actionsPanel.selectedAction != null) {
                    if (Input.GetButton("Shift") == true) {
                        eventDescription.SetFormulaText(actionsPanel.selectedAction.isUsable, actionsPanel.selectedAction.a);
                    }
                    else {
                        eventDescription.SetAttackText(actionsPanel.selectedAction.a, actionsPanel.selectedAction.isUsable);
                    }
                }
                else {
                    eventDescription.ClearText();
                    eventDescription.displayedAttack = null;
                }  
            }
        }
    }
}
