using Localization;
using PartyManager = Party.PartyManager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class ToastPanel : MonoBehaviour {
        
        /* external component references */
        public Animator flameAnimator;
        public Button b;
        public LocalizedText titleText;
        public LocalizedText descriptionText;
        public Image textBackground;        /// <value> Image background for text component </value>
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>
        public LayoutElement le;
        public SpriteRenderer toastSprite;
        public SpriteRenderer achievementIcon;
        public enum toastType { HP, MP, EXP, WAX, SE, PROGRESS, QUESTCOMPLETE };

        private Coroutine fader;            /// <value> Store the coroutine responsible for visibility to stop it if notification changes suddenly </value>
        private Coroutine fadeOuter;        /// <value> Store the coroutine responsible for fading out to stop it if notification changes suddenly </value>
        private float lerpSpeed = 4;        /// <value> Speed at which canvas fades in and out </value>

        public void SetNotification(bool[] types, string[] amounts) {
            int typesCount = 0;
            string titleKey = "";
            string[] descriptionKeys = new string[]{ "none_label", "none_label", "none_label", "none_label", "none_label", "none_label", "none_label" };
            string[] amountStrings = new string[7];

            if (types[(int)toastType.HP] == true) {
                titleKey = "HP_toast";
                descriptionKeys[0] = "HP_label_coloured";
                amountStrings[0] = "<color=#EA323C>" + amounts[0] + "</color>";
                typesCount++;
            }
            if (types[(int)toastType.MP] == true) {
                titleKey = "MP_toast";
                descriptionKeys[1] = "MP_label_coloured";
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
        public void SetAchievementNotification(Sprite sprite, int index) {
            titleText.SetKey("ACHIEVEMENT_toast");
            descriptionText.SetKey(index + "_ach_title");
            flameAnimator.gameObject.SetActive(false);
            achievementIcon.gameObject.SetActive(true);
            achievementIcon.sprite = sprite;
            achievementIcon.color = GetAchievementColor(index);
            

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
            yield return new WaitForSeconds(2);
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
                    fader = StartCoroutine(Fade(0));
                }
            }
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
                le.preferredHeight = textBackground.rectTransform.sizeDelta.y;  // then size the preferred size of the layout element
                le.preferredWidth = textBackground.rectTransform.sizeDelta.x;
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
                achievementIcon.gameObject.SetActive(false);
                flameAnimator.SetTrigger("normal");
                flameAnimator.ResetTrigger("normal");   // reset the trigger, because otherwise animator will swap back immediately on repeat triggers
                gameObject.SetActive(false);
            }
        }
    }
}