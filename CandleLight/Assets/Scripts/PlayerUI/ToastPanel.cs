using Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class ToastPanel : MonoBehaviour {
        
        public LocalizedText titleText;
        public LocalizedText descriptionText;
        public Image textBackground;        /// <value> Image background for text component </value>
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>

        public enum toastType { HP, MP, EXP, SE, PROGRESS };

        private float lerpSpeed = 4;        /// <value> Speed at which canvas fades in and out </value>

        public void SetNotification(bool[] types, string[] amounts) {
            int typesCount = 0;
            string titleKey = "";
            string[] descriptionKeys = new string[]{ "none_label", "none_label", "none_label", "none_label", "none_label" };
            string[] amountStrings = new string[5];

            if (types[(int)toastType.HP] == true) {
                titleKey += "HP_";
                descriptionKeys[0] = "HP_label";
                amountStrings[0] = amounts[0];
                typesCount++;
            }
            if (types[(int)toastType.MP] == true) {
                titleKey += "MP_";
                descriptionKeys[1] = "MP_label";
                amountStrings[1] = amounts[1];
                typesCount++;
            }
            if (types[(int)toastType.EXP] == true) {
                titleKey += "EXP_";
                descriptionKeys[2] = "EXP_label";
                amountStrings[2] = amounts[2];
                typesCount++;
            }
            if (types[(int)toastType.SE] == true) {
                titleKey += "SE_";
                descriptionKeys[3] = "SE_label";
                amountStrings[3] = amounts[3];
                typesCount++;
            }
            if (types[(int)toastType.PROGRESS] == true) {
                titleKey += "PROG_";
                descriptionKeys[4] = "PROG_label";
                amountStrings[4] = amounts[4] + "%";
                typesCount++;
            }

            titleKey += "toast";
            if (typesCount > 1) {
                titleText.SetKey("generic_toast");
            }
            else {
                titleText.SetKey(titleKey);
            }
            descriptionText.SetMultipleKeysAndAppend(descriptionKeys, amountStrings);

            SetVisible(true);
            StartCoroutine(FadeOut());
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
                StartCoroutine(Fade(1));
            }
            else {
                if (gameObject.activeSelf) {
                    StartCoroutine(Fade(0));
                }
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