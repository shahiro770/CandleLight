/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2020
* 
* The ToastPanel class is used to control all aspects of a toast notification that may show up
* on the screen to give the player status updates
* 
*/

using GameManager = General.GameManager;
using Localization;
using PartyManager = Party.PartyManager;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class ToastPanel : MonoBehaviour {
        
        /* external component references */
        public Animator arrowAnimator;
        public Animator flameAnimator;
        public Button b;
        public GameObject arrow;
        public LocalizedText titleText;
        public LocalizedText descriptionText;
        public Image textBackground;        /// <value> Image background for text component </value>
        public Canvas c;
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>
        public LayoutElement le;
        public SpriteRenderer arrowRenderer;
        public SpriteRenderer toastSprite;
        public SpriteRenderer achievementIcon;
        public Sprite[] arrowSprites = new Sprite[4];
        public enum arrowType { UP, RIGHT, DOWN, LEFT };
        public enum toastType { HP, MP, EXP, WAX, SE, PROGRESS, QUESTCOMPLETE };
        public bool isArrowHolder = false;

        private Coroutine fader;            /// <value> Store the coroutine responsible for visibility to stop it if notification changes suddenly </value>
        private Coroutine fadeOuter;        /// <value> Store the coroutine responsible for fading out to stop it if notification changes suddenly </value>
        private string partySTRText = LocalizationManager.instance.GetLocalizedValue("partySTR_text");  /// <value> Localized text for the text "Party STR: " </value>
        private string partyDEXText = LocalizationManager.instance.GetLocalizedValue("partyDEX_text");  /// <value> Localized text for the text "Party DEX: " </value>
        private string partyINTText = LocalizationManager.instance.GetLocalizedValue("partyINT_text");  /// <value> Localized text for the text "Party INT: " </value>
        private string partyLUKText = LocalizationManager.instance.GetLocalizedValue("partyLUK_text");  /// <value> Localized text for the text "Party LUK: " </value>
        private string threshSTRText = LocalizationManager.instance.GetLocalizedValue("threshSTR_text");      /// <value> Localized text for the text "Threshold STR: " </value>
        private string threshDEXText = LocalizationManager.instance.GetLocalizedValue("threshDEX_text");      /// <value> Localized text for the text "Threshold STR: " </value>
        private string threshINTText = LocalizationManager.instance.GetLocalizedValue("threshINT_text");      /// <value> Localized text for the text "Threshold STR: " </value>
        private string threshLUKText = LocalizationManager.instance.GetLocalizedValue("threshLUK_text");      /// <value> Localized text for the text "Threshold STR: " </value>
        private float lerpSpeed = 4;        /// <value> Speed at which canvas fades in and out </value>
        private static bool isShowingArrow = false;

        public void SetNotification(bool[] types, string[] amounts) {
            int typesCount = 0;
            string titleKey = "";
            string[] descriptionKeys = new string[]{ "none_label", "none_label", "none_label", "none_label", "none_label", "none_label", "none_label" };
            string[] amountStrings = new string[7];

            if (types[(int)toastType.HP] == true) {
                titleKey = "HP_toast";
                descriptionKeys[0] = "HP_label";
                amountStrings[0] = "<color=#EA323C>" + amounts[0] + "</color>";
                typesCount++;
            }
            if (types[(int)toastType.MP] == true) {
                titleKey = "MP_toast";
                descriptionKeys[1] = "MP_label";
                amountStrings[1] = "<color=#502BFF>" + amounts[1] + "</color>";
                typesCount++;
            }
            if (types[(int)toastType.EXP] == true) {
                titleKey = "EXP_toast";
                descriptionKeys[2] = "EXP_label";
                amountStrings[2] = amounts[2];
                typesCount++;
            }
            if (types[(int)toastType.WAX] == true) {
                titleKey = "WAX_toast";
                descriptionKeys[3] = "WAX_label_coloured";
                amountStrings[3] = "<color=#FFCD02>" + amounts[3] + "</color>";
                typesCount++;
            }
            if (types[(int)toastType.SE] == true) {
                titleKey = "SE_toast";
                descriptionKeys[4] = amounts[4];    // for status effects, just showing the SE name is enough (no fancy descriptors)
                amountStrings[4] = "";
                typesCount++;
            }
            if (types[(int)toastType.PROGRESS] == true) {
                titleKey = "PROG_toast";
                descriptionKeys[5] = "PROG_label";
                amountStrings[5] = amounts[5] + "%";
                typesCount++;
            }
            if (types[(int)toastType.QUESTCOMPLETE] == true) {
                titleKey = amounts[6] + "_quest_complete_title";     // Completing a quest overrides all possible title keys
                descriptionKeys[6] = "none_label";
                amountStrings[6] = "";
                typesCount++;
            }

            if (typesCount > 1 && types[(int)toastType.QUESTCOMPLETE] == false) {
                titleText.SetKey("generic_toast");
            }
            else {
                titleText.SetKey(titleKey);
            }
            descriptionText.SetMultipleKeysAndAppend(descriptionKeys, amountStrings);

            b.interactable = false;
            SetVisible(true);
            fadeOuter = StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Turns the toast into a temporary display for the player's WAX, that doesn't fade immediately 
        /// Used during shops
        /// </summary>
        public void SetShopNotification() {
            titleText.SetKey("shop_toast");
            descriptionText.SetKeyAndAppend("WAX_label_coloured", "<color=#FFCD02>" + PartyManager.instance.WAX.ToString() + "</color>");

            b.interactable = false;
            if (fadeOuter != null) {
                StopCoroutine(fadeOuter);
            }
            SetVisible(true);
            flameAnimator.SetTrigger("shop");   // shops have a different flame colour
        }

        /// <summary>
        /// Special notification indicating a quest has been taken
        /// </summary>
        /// <param name="questName"></param>
        public void SetQuestNotification(string questName) {
            titleText.SetKey("QUEST_toast");
            descriptionText.SetKey(questName + "_quest_title");

            b.interactable = false;
            SetVisible(true);
            fadeOuter = StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Special notification showing a tutorial to help player's understand the game
        /// </summary>
        /// <param name="tutorialName"></param>
        public void SetTutorialNotification(string tutorialName) {
            titleText.SetKey("TUTORIAL_toast");
            descriptionText.SetKey(tutorialName + "_tutorial");

            b.interactable = true;
            if (fadeOuter != null) {
                StopCoroutine(fadeOuter);
            }

            switch (tutorialName) {
                case "special0":
                    ShowTutorialArrow(-210, -110, true);   
                    break;
                case "party0":
                    ShowTutorialArrow(210, -110, true);   
                    break;
                case "gear0":
                    ShowTutorialArrow(205, 50, false);   
                    break;
                case "skills0":
                    ShowTutorialArrow(320, -110, true);   
                    break;
                case "info0":
                    ShowTutorialArrow(425, -110, true);
                    break;
            }
            
            SetVisible(true);
        }

        /// <summary>
        /// Special notification indicating a partyMember has joined
        /// </summary>
        /// <param name="pmName"></param>
        public void SetPartyMemberNotification(string pmName) {
            titleText.SetKey("PARTYMEMBER_toast");
            descriptionText.TextAndAppendKey(pmName, "joined_party");

            b.interactable = false;
            SetVisible(true);
            fadeOuter = StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Special notification for indicating an achievement has been unlocked
        /// </summary>
        /// <param name="sprite"> Sprite to be displayed in the notification </param>
        /// <param name="index"> Index of the sprite </param>
        public void SetAchievementNotification(Sprite sprite, int index, bool isHighLayer) {
            titleText.SetKey("ACHIEVEMENT_toast");
            descriptionText.SetKey(index + "_ach_title");
            flameAnimator.gameObject.SetActive(false);
            achievementIcon.gameObject.SetActive(true);
            achievementIcon.sprite = sprite;
            achievementIcon.color = GetAchievementColor(index);
            if (isHighLayer == true) {
                c.sortingOrder = 6;
                achievementIcon.sortingOrder = 7;
            }

            b.interactable = false;
            SetVisible(true);      
            fadeOuter = StartCoroutine(FadeOut());      
        }

        /// <summary>
        /// Shows a notificatino for the results of a stat checking interaction
        /// </summary>
        /// <param name="isSucc"></param>
        /// <param name="checkIndicator"></param>
        /// <param name="stat"></param>
        /// <param name="threshold"></param>
        public void SetStatCheckNotification(bool isSucc, int checkIndicator, int stat, int threshold) {
            if (isSucc == true) {
                titleText.SetKey("SUCC_toast");
            }
            else {
                titleText.SetKey("FAIL_toast");
            }

            if (checkIndicator == 1) {
                descriptionText.SetText(partySTRText + "<color=#B91D00>" + stat + "</color>" + "\n" + threshSTRText + "<color=#B91D00>" + threshold + " </color>");
            }
            else if (checkIndicator == 2) {
                descriptionText.SetText(partyDEXText + "<color=#5AC54F>" + stat + "</color>" + "\n" + threshDEXText + "<color=#5AC54F>" + threshold + " </color>");
            }
            else if (checkIndicator == 3) {
                descriptionText.SetText(partyINTText  + "<color=#0098DC>" + stat + "</color>" + "\n" + threshINTText + "<color=#0098DC>" + threshold + " </color>");
            }
            else if (checkIndicator == 4) {
                descriptionText.SetText(partyLUKText  + "<color=#FFCD02>" + stat + "</color>" + "\n" + threshLUKText + "<color=#FFCD02>" + threshold + " </color>");
            }
            

            b.interactable = false;
            SetVisible(true);
            fadeOuter = StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Hard code the achievement's unlocked colour for easy use
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Color GetAchievementColor(int index) {
            byte startAlpha = (byte)(255 * textBackgroundCanvas.alpha);
            switch (index) {
                case 0:
                    return new Color32(133, 133, 133, startAlpha);
                case 1:
                    return new Color32(234, 50, 60, startAlpha);
                case 2:
                    return new Color32(92, 138, 57, startAlpha);
                case 3:
                    return new Color32(255, 205, 2, startAlpha);
                case 4:
                    return new Color32(155, 66, 28, startAlpha);
                default:
                    return new Color32(61, 61, 61, startAlpha);
            }
        }

        /// <summary>
        /// Updates the amount of WAX being displayed in the notification.
        /// Assumes the notification is a shop notification
        /// </summary>
        public void UpdateWAXAmount() {
            descriptionText.SetKeyAndAppend("WAX_label_coloured", "<color=#FFCD02>" + PartyManager.instance.WAX.ToString() + "</color>");
        }

        public IEnumerator FadeOut() {
            yield return new WaitForSeconds(2.8f - (-1f + GameManager.instance.gsDataCurrent.animationSpeed));
            SetVisible(false);
        }
        
        /// <summary>
        /// Fades the display out
        /// </summary>
        public void SetVisible(bool value) {
            if (value == true) {
                gameObject.SetActive(true);
                if (fader != null) {
                    StopCoroutine(fader);
                }
                fader = StartCoroutine(Fade(1));
            }
            else {
                if (gameObject.activeSelf == true) {
                    if (fader != null) {
                        StopCoroutine(fader);
                    }
                    if (arrow != null && isShowingArrow == true && isArrowHolder == true) {
                        HideTutorialArrow();
                    }
                    fader = StartCoroutine(Fade(0));
                }
            }
        }

        public void ShowTutorialArrow(int x, int y, bool isV) {
            if (isShowingArrow == false) {
                isShowingArrow = true;
                isArrowHolder = true;
                arrow.transform.localPosition = new Vector3(x, y);
                
                if (isV == true){
                    arrowRenderer.sprite = arrowSprites[(int)arrowType.DOWN];
                    arrowAnimator.ResetTrigger("vbob");    
                    arrowAnimator.SetTrigger("vbob");
                }
                else {
                    arrowRenderer.sprite = arrowSprites[(int)arrowType.LEFT];
                    arrowAnimator.ResetTrigger("hbob");    
                    arrowAnimator.SetTrigger("hbob");
                }
                arrowAnimator.ResetTrigger("stop");
                arrowAnimator.ResetTrigger("show");
                arrowAnimator.SetTrigger("show"); 
            }
        }

        public void HideTutorialArrow() {
            isShowingArrow = false;
            isArrowHolder = false;
            arrowAnimator.ResetTrigger("hide");
            arrowAnimator.SetTrigger("hide");
            arrowAnimator.SetTrigger("stop");
        }

        /// <summary>
        /// Changes the alpha of the display to the target value
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        private IEnumerator Fade(int targetAlpha) {
            if (targetAlpha == 1) {
                le.flexibleHeight = 1;  // lets the textbackground expand to fit the text first
                le.flexibleWidth = 1;
                yield return new WaitForEndOfFrame();   // wait a frame to update
                // then size the min size of the layout element
                if (textBackground.rectTransform.sizeDelta.y == 87.5f) {    // HACK to deal with two four line tips squishing eachother (its always 87.5 for some reason)
                    le.minHeight = 97.02f;
                }
                else {
                    le.minHeight = textBackground.rectTransform.sizeDelta.y;  
                }
                le.minWidth = textBackground.rectTransform.sizeDelta.x;
                le.flexibleHeight = 0;  // disable the flexibility after  to not squish additional toastPanels
                le.flexibleWidth = 0;
            }
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = textBackgroundCanvas.alpha;
            float newAlpha;

            while (textBackgroundCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                newAlpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                textBackgroundCanvas.alpha = newAlpha;
                toastSprite.color = new Color(255, 255, 255, newAlpha);
                achievementIcon.color = new Color(achievementIcon.color.r, achievementIcon.color.g, achievementIcon.color.b, newAlpha);

                yield return new WaitForEndOfFrame();
            }

            if (targetAlpha == 0) {
                flameAnimator.gameObject.SetActive(true);
                achievementIcon.sortingOrder = 5;
                achievementIcon.gameObject.SetActive(false);
                c.sortingOrder = 4;
                flameAnimator.SetTrigger("normal");
                flameAnimator.ResetTrigger("normal");   // reset the trigger, because otherwise animator will swap back immediately on repeat triggers
                gameObject.SetActive(false);
            }
        }
    }
}