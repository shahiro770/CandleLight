/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

using Attack = Combat.Attack;
using AttackConstants = Constants.AttackConstants;
using Characters;
using Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class EventDescription : MonoBehaviour {
        
        /* external component references */
        public LocalizedText eventText;     /// <value> Text for event </value>
        public Image textBackground;        /// <value> Image background for text component </value>
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>
        
        public Attack displayedAttack;      /// <value> Attack information currently being displayed </value>

        private Color32 normalColour = new Color32(255, 255, 255, 255);             /// <value> White </value>
        private Color32 normalColourHalfAlpha = new Color32(255, 255, 255, 128);    /// <value> White half alpha to appear darker </value>
        private Color32 unusableColour = new Color32(196, 36, 48, 255);             /// <value> Red colour to indicate unusable attack </value>
        private Color32 unusableColourHalfAlpha = new Color32(196, 36, 48, 128);    /// <value> Red colour half alpha to indicate unusable attack </value>
        private float lerpSpeed = 4;        /// <value> Speed at which canvas fades in and out </value>
        private string costText = LocalizationManager.instance.GetLocalizedValue("cost_text");  /// <value> Localized text for the word "cost" </value>
        private string critHitText = LocalizationManager.instance.GetLocalizedValue("crit_hit_text");      /// <value> Localized text for the phrase "Critical Hit!" </value>
        private string critHealText = LocalizationManager.instance.GetLocalizedValue("crit_heal_text");    /// <value> Localized text for the phrase "Critical Heal!" </value>
        private string damageText = LocalizationManager.instance.GetLocalizedValue("damage_text");  /// <value> Localized text for the word "damage" </value>
        private string dodgedText = LocalizationManager.instance.GetLocalizedValue("dodged_text");  /// <value> Localized text for the word "dodged" </value>
        private string lostText = LocalizationManager.instance.GetLocalizedValue("lost_text");      /// <value> Localized text for the word "lost" </value>
        private string noReviveText = LocalizationManager.instance.GetLocalizedValue("no_revive_text"); /// <value> Localized text for the phrase "No one needs to be revived." </value>
        private string noRekindleText = LocalizationManager.instance.GetLocalizedValue("no_rekindle_text"); /// <value> Localized text for the phrase "Only equipped candles can be rekindled." </value>
        private string noMoveText = LocalizationManager.instance.GetLocalizedValue("no_move_text");     /// <value> Localized text for the phrase " can't move!" </value>
        private string HPText = LocalizationManager.instance.GetLocalizedValue("HP_text");          /// <value> Localized text for the text "HP" </value>
        private string MPText = LocalizationManager.instance.GetLocalizedValue("MP_text");          /// <value> Localized text for the text "MP" </value>
        private string healText = LocalizationManager.instance.GetLocalizedValue("heal_text");      /// <value> Localized text for the text "heal" </value>
        private string wasHealedForText = LocalizationManager.instance.GetLocalizedValue("was_healed_for_text");     /// <value> Localized text for the text "HP" </value>
        private string restoredText = LocalizationManager.instance.GetLocalizedValue("restored_text");     /// <value> Localized text for the text "restored" </value>
        private string summonText = LocalizationManager.instance.GetLocalizedValue("summon_text");  /// <value> Localized text for the text "summon a" </value>
        private string colour = "normal";   /// <value> Current colour state </value>
        private bool appendMode = false;     /// <value> true to append </value>

        /// <summary>
        /// Awake to set displayedattack to null (so unity doesn't do its fake null setting)
        /// </summary>
        void Awake() {
            displayedAttack = null;
        }

        public void SetAppendMode(bool value) {
            appendMode = value;
        }

        /// <summary>
        /// Changes the displayed text
        /// </summary>
        /// <param name="textKey"> Localized key for text to display </param>
        public void SetKey(string textKey) {
            if (this.colour != "normal") {
                SetColour("normal");
            }
            textBackgroundCanvas.alpha = 1;
            eventText.SetKey(textKey);
            displayedAttack = null;
        }

        /// <summary>
        /// Changes the displayed text and fades in
        /// </summary>
        /// <param name="textKey"> Text to display </param>
        public void SetKeyAndFadeIn(string textKey) {
            if (this.colour != "normal") {
                SetColour("normal");
            }
            eventText.SetKey(textKey);
            StartCoroutine(Fade(1));
            displayedAttack = null;
        }
        
        /// <summary>
        /// Fades the display out
        /// </summary>
        public void FadeOut() {
            StartCoroutine(Fade(0));
        }

        /// <summary>
        /// Changes the displayed text to show how much damage a partyMember took
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="amount"> Positive int amount </param>
        public void SetPMDamageText(PartyMember pm, int amount) {
            string damagedText = pm.pmName + " " + lostText + " <color=#EA323C>" + amount.ToString() + " " + HPText + "</color>";
            if (appendMode == false) { 
                eventText.SetText(damagedText);
            }
            else {
                eventText.AppendText("\n" + damagedText);
            }
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show how much damage a partyMember took from a critical hit
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="amount"> Positive int amount </param>
        public void SetPMDamageCritText(PartyMember pm, int amount) {
            string damagedText = critHitText + " " + pm.pmName + " " + lostText + " <color=#EA323C>" + amount.ToString() + " " + HPText + "</color>";
            if (appendMode == false) { 
                eventText.SetText(damagedText);
            }
            else {
                eventText.AppendText("\n" + damagedText);
            }
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember dodged the attack
        /// </summary>
        /// <param name="pm"> PartyMember that dodged the attack </param>
        public void SetPMDodgeText(PartyMember pm) {
            string dodgedString = pm.pmName + " " + dodgedText;
            if (appendMode == false) { 
                eventText.SetText(dodgedString);
            }
            else {
                eventText.AppendText("\n" + dodgedString);
            }
            displayedAttack = null;
            textBackgroundCanvas.alpha = 1;
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember was healed by a healing attack
        /// </summary>
        /// <param name="pm"> PartyMember that was healed </param>
        /// <param name="amount"> Amount that was healed for </param>
        public void SetPMHealText(PartyMember pm, int amount) {
            string healedText = pm.pmName + " " + wasHealedForText + " <color=#EA323C>" + amount.ToString() + " " + HPText;
            eventText.SetText(healedText);
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember was healed by a healing attack
        /// </summary>
        /// <param name="pm"> PartyMember that was healed </param>
        /// <param name="amount"> Amount that was healed for </param>
        public void SetPMHealCritText(PartyMember pm, int amount) {
            string healedText = critHealText + " " + pm.pmName + " " + wasHealedForText + " <color=#EA323C>" + amount.ToString() + " " + HPText;
            eventText.SetText(healedText);
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember recovered MP by a focus attack
        /// </summary>
        /// <param name="pm"> PartyMember that was focus'd </param>
        /// <param name="amount"> Amount of MP recovered </param>
        public void SetPMFocusText(PartyMember pm, int amount) {
            string focusText = pm.pmName + " " + restoredText + " <color=#502BFF>" + amount.ToString() + " " + MPText;
            eventText.SetText(focusText);
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember recovered MP by a focus attack
        /// </summary>
        /// <param name="pm"> PartyMember that was healed </param>
        /// <param name="amount"> Amount of MP recovered </param>
        public void SetPMFocusCritText(PartyMember pm, int amount) {
            string focusText = critHealText + " " + pm.pmName + " " + restoredText + " <color=#502BFF>" + amount.ToString() + " " + MPText;
            eventText.SetText(focusText);
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember was healed by a healing attack
        /// </summary>
        /// <param name="pm"> PartyMember that was healed </param>
        /// <param name="amount"> Amount that was healed for </param>
        public void SetMHealText(Monster m, int amount) {
            string healedText = LocalizationManager.instance.GetLocalizedValue(m.monsterSpriteName + "_monster") + " " + wasHealedForText + " <color=#EA323C>" + amount.ToString() + " " + HPText;
            eventText.SetText(healedText);
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }

        /// <summary>
        /// Changes the displayed text to show that a partyMember was healed by a healing attack
        /// </summary>
        /// <param name="pm"> PartyMember that was healed </param>
        /// <param name="amount"> Amount that was healed for </param>
        public void SetMHealCritText(Monster m, int amount) {
            string healedText = critHealText + " " + LocalizationManager.instance.GetLocalizedValue(m.monsterSpriteName + "_monster") + " " + wasHealedForText + " <color=#EA323C>" + amount.ToString() + " " + HPText;
            eventText.SetText(healedText);
            displayedAttack = null;

            if (this.colour != "normal") {
                SetColour("normal");
            }
        }
    
        /// <summary>
        /// Changes the displayed text to show the cost and effects of an attack action
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="isUsable"> True if usable, false otherwise </param>
        public void SetAttackText(Attack a, bool isUsable) {
           SetAttackText(a);

            if (!isUsable) {
                SetColour("unusable");
            }
            else {
                SetColour("normal");
            }
            textBackgroundCanvas.alpha = 1;
        }

        /// <summary>
        /// Sets the attack text, but maintains the original usability colouring
        /// (Only used when swapping between formulaText and attackText, as usability doesn't change betwene those)
        /// </summary>
        /// <param name="a"></param>
        public void SetAttackText(Attack a) {
            string attackString;
            string costString;
            if (a.costType == "MP") {
                costString = "<color=#502BFF>" + a.costValue + " " + a.costType + "</color>";
            }
            else {
                costString = "<color=#EA323C>" + a.costValue + " " + a.costType + "</color>";
            }

            if (a.type == AttackConstants.BUFF || a.type == AttackConstants.DEBUFF) {   
                attackString = costString;
            }
            else if (a.type == AttackConstants.HEALHP) {
                attackString = costString + " " + healText + " " + a.attackValue + " " + HPText; 
            }
            else if (a.type == AttackConstants.SUMMON) {
                attackString = costString + " " + summonText + " " + LocalizationManager.instance.GetLocalizedValue(a.nameKey + "_des");
            }
            else {  // physical or magical attack
                attackString = costString + " " + a.attackValue + " " + a.type + " " + damageText;
            }

            if (a.seName != "none") {
                if (a.seDuration == 1) {
                    attackString += ". " + a.seChance + "% chance to " + a.seNameKey + " for " + a.seDuration  + " turn";
                }
                else {
                    attackString += ". " + a.seChance + "% chance to " + a.seNameKey + " for " + a.seDuration  + " turns";
                }
            }
            if (a.scope != "single") {
                if (a.scope == "adjacent") {
                    if (a.seName != "none") {
                        attackString += ".\nHits adjacents";
                    }
                    else {
                        attackString += ". Hits adjacents";
                    }
                }
                else if (a.scope == "allEnemies") {
                    if (a.seName != "none") {
                        attackString += ".\nHits all enemies";
                    }
                    else {
                        attackString += ". Hits all enemies";
                    }
                }
            }
            
            displayedAttack = a;
            eventText.SetText(attackString);
        }

        /// <summary>
        /// Sets formula text for an attack
        /// </summary>
        /// <param name="isUsable"> True if usable, false otherwise </param>
        /// <param name="a"> Attack to set formula text to, but may be null if it can be assumed an attack is already being displayed </param>
        public void SetFormulaText(bool isUsable, Attack a = null) {
            SetFormulaText(a);

            if (!isUsable) {
                SetColour("unusable");
            }
            else {
                SetColour("normal");
            }
            textBackgroundCanvas.alpha = 1;
        }

        /// <summary>
        /// Sets formula text, but doesn't change usability colouring
        /// </summary>
        /// <param name="a"> Attack to set formula text to, but may be null if it can be assumed an attack is already being displayed </param>
        public void SetFormulaText(Attack a = null) {
            if (a != null) {
                displayedAttack = a;
            }
            string formulaString;

            if (displayedAttack.costType == "MP") {
                formulaString = "<color=#502BFF>" + displayedAttack.costFormula + " " + displayedAttack.costType + "</color>";
            }
            else {
                formulaString = "<color=#EA323C>" + displayedAttack.costFormula + " " + displayedAttack.costType + "</color>";
            }

            if (displayedAttack.type == AttackConstants.BUFF || displayedAttack.type == AttackConstants.DEBUFF) {   

            }
            else if (displayedAttack.type == AttackConstants.HEALHP) {
                formulaString += " " + healText + " " + displayedAttack.damageFormula + " " + HPText; 
            }
            else if (displayedAttack.type == AttackConstants.SUMMON) {
                formulaString += " " + summonText + " " + LocalizationManager.instance.GetLocalizedValue(displayedAttack.nameKey + "_des");
            }
            else {  // physical or magical attack
                formulaString += " " + displayedAttack.damageFormula + " " + displayedAttack.type + " " + damageText;
            }
                
            if (displayedAttack.seName != "none") {
                if (displayedAttack.seDuration == 1) {
                    formulaString += ". " + displayedAttack.seChance + "% chance to " + displayedAttack.seNameKey + " for " + displayedAttack.seDuration  + " turn";
                }
                else {
                    formulaString += ". " + displayedAttack.seChance + "% chance to " + displayedAttack.seNameKey + " for " + displayedAttack.seDuration  + " turns";
                }
            }
            if (displayedAttack.scope != "single") {
                if (displayedAttack.scope == "adjacent") {
                    if (displayedAttack.seName != "none") {
                        formulaString += ".\nHits adjacents";
                    }
                    else {
                        formulaString += ". Hits adjacents";
                    }
                }
                else if (displayedAttack.scope == "allEnemies") {
                    if (displayedAttack.seName != "none") {
                        formulaString += ".\nHits all enemies";
                    }
                    else {
                        formulaString += ". Hits all enemies";
                    }
                }
            }

            eventText.SetText(formulaString);
        }

        /// <summary>
        /// Displays text informing the player a partyMember can't do anything on their combat turn
        /// </summary>
        /// <param name="cname"></param>
        public void SetNoMoveTextPM(string pmname) {
            eventText.SetText(pmname + " " + noMoveText);
            textBackgroundCanvas.alpha = 1;
            displayedAttack = null;
        }

        /// <summary>
        /// Displays text informing the player a monster can't do anything on their combat turn
        /// </summary>
        /// <param name="cname"></param>
        public void SetNoMoveTextM(string mname) {
            eventText.SetText(LocalizationManager.instance.GetLocalizedValue(mname + "_monster") + " " + noMoveText);
            textBackgroundCanvas.alpha = 1;
            displayedAttack = null;
        }

        /// <summary>
        /// Displays text informing the player they can't revive anyone
        /// </summary>
        public void SetNoReviveText() {
            eventText.SetText(noReviveText);
        }

        /// <summary>
        /// Displays text informing the player they have nothing to rekindle
        /// </summary>
        public void SetNoRekindleText() {
            eventText.SetText(noRekindleText);
        }

        /// <summary>
        /// Stop displaying and remove all text
        /// </summary>
        public void ClearText() {
            textBackgroundCanvas.alpha = 0;
            eventText.Clear();
        }

        /// <summary>
        /// Returns true if eventText has text, false otherwise
        /// </summary>
        /// <returns> Boolean </returns>
        public bool HasText() {
            return eventText.HasText();
        }

        /// <summary>
        /// Sets the current colour of the text
        /// </summary>
        /// <param name="colour"> String, "normal" or "unusable" </param>
        public void SetColour(string colour) {
            this.colour = colour;

            if (colour == "normal") {
                textBackground.color = normalColourHalfAlpha;
                eventText.SetColour(normalColour);
            }
            else if (colour == "unusable") {
                textBackground.color = unusableColourHalfAlpha;
                eventText.SetColour(unusableColour);
            }
            
        }

        /// <summary>
        /// Changes the alpha of the display to the target value
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        public IEnumerator Fade(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = textBackgroundCanvas.alpha;

            while (textBackgroundCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                textBackgroundCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
