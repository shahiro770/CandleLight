/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The Quest class represents a visual reminder to the player that
* there is some on-going goal that can be fulfilled.
*
*/

using Localization;
using System.Collections;
using UnityEngine;

namespace PlayerUI {

    public class Quest : MonoBehaviour{

        /* external component references */
        public LocalizedText titleText;
        public LocalizedText subtitleText;
        public SpriteRenderer questSprite;
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>

        public string questName = "";
        public string startEvent = "";
        public string nextEvent = "";

        private float lerpSpeed = 4;        /// <value> Speed at which canvas fades in and out </value>

        /// <summary>
        /// Sets the quest display, showing the quest name and
        /// an informative prompt, before making it visible
        /// </summary>
        /// <param name="questName"></param>
        public void SetQuest(string questName, string startEvent, string nextEvent) {
            this.questName = questName;
            this.startEvent = startEvent;
            this.nextEvent = nextEvent;

            titleText.SetKey(questName + "_quest_title");
            subtitleText.SetKey(questName + "_quest_sub");
            questSprite.sprite = Resources.Load<Sprite>("Sprites/Quests/" + questName);

            SetVisible(true);
        }

        public void UpdateQuestProgress(string nextEvent) {
            this.nextEvent = nextEvent;
        }

        /// <summary>
        /// Clear the quest, making it invisible
        /// </summary>
        public void CompleteQuest() {
            questName = "";
            SetVisible(false);
        }

        /// <summary>
        /// Returns important data about this quest
        /// </summary>
        /// <returns></returns>
        public string[] GetQuestData() {
            return new string[] { questName, startEvent, nextEvent };
        }

        /// <summary>
        /// Fades the display out
        /// </summary>
        public void SetVisible(bool value) {
            if (value == true) {
                gameObject.SetActive(true);
                if (gameObject.activeInHierarchy) {
                    StartCoroutine(Fade(1));
                }
                else {
                    textBackgroundCanvas.alpha = 1;
                }
            }
            else {
                if (gameObject.activeInHierarchy) {
                    StartCoroutine(Fade(0));
                }
                else {
                    textBackgroundCanvas.alpha = 0;
                }
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Changes the alpha of the display to the target value
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        private IEnumerator Fade(int targetAlpha) {
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

            if (targetAlpha == 0) {
                gameObject.SetActive(false);
            }
        }
    }
}