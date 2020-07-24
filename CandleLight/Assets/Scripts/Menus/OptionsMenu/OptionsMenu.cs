/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The OptionsMenu class is used to modify aspects of the game.
*/

using General;
using Localization;
using PlayerUI;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menus.OptionsMenu {

    public class OptionsMenu : MonoBehaviour {

        public Button b;
        public ButtonTransitionState tutorialYes;
        public ButtonTransitionState tutorialNo;
        public ButtonTransitionState tipsYes;
        public ButtonTransitionState tipsNo;
        public ButtonTransitionState as1;
        public ButtonTransitionState as2;
        public ButtonTransitionState timerYes;
        public ButtonTransitionState timerNo;

        public TooltipTextMesh effectstt;           /// <value> sound effects text tooltip </value>
        public TooltipTextMesh musictt;             /// <value> music text tooltip </value>
        public TooltipTextMesh tutorialtt;          /// <value> tutorial text tooltip </value>
        public TooltipTextMesh tipstt;              /// <value> tips text tooltip </value>
        public TooltipTextMesh animationSpeedtt;    /// <value> animation speed text tooltip </value>
        public TooltipTextMesh timertt;             /// <value> timer text tooltip </value>

        private float labelWidths = 180f;
        private float labelWidthsBig = 230f;
        private ColorBlock optionEnabled = new ColorBlock();

        void Awake() {
            optionEnabled.normalColor = new Color32(255, 255, 255, 255);
            optionEnabled.highlightedColor = new Color32(255 ,255, 255, 255);
            optionEnabled.pressedColor = new Color32(255 ,255, 255, 200);
            optionEnabled.disabledColor = new Color32(61 ,61, 61, 255);
            optionEnabled.colorMultiplier = 1;

            tutorialYes.SetColorBlock("normal", b.colors);
            tutorialNo.SetColorBlock("normal", b.colors);
            tutorialYes.SetColorBlock("normalAlternate", optionEnabled);
            tutorialNo.SetColorBlock("normalAlternate", optionEnabled);
            tipsYes.SetColorBlock("normal", b.colors);
            tipsNo.SetColorBlock("normal", b.colors);
            tipsYes.SetColorBlock("normalAlternate", optionEnabled);
            tipsNo.SetColorBlock("normalAlternate", optionEnabled);
            as1.SetColorBlock("normal", b.colors);
            as2.SetColorBlock("normal", b.colors);
            as1.SetColorBlock("normalAlternate", optionEnabled);
            as2.SetColorBlock("normalAlternate", optionEnabled);
            timerYes.SetColorBlock("normal", b.colors);
            timerNo.SetColorBlock("normal", b.colors);
            timerYes.SetColorBlock("normalAlternate", optionEnabled);
            timerNo.SetColorBlock("normalAlternate", optionEnabled);

            tutorialtt.SetKey("title", "tutorial_title");
            tutorialtt.SetKey("subtitle", "tutorial_des");
            tipstt.SetKey("title", "tips_title");
            tipstt.SetKey("subtitle", "tips_des");
            animationSpeedtt.SetKey("title", "as_title");
            animationSpeedtt.SetKey("subtitle", "as_des");
            timertt.SetKey("title", "timer_title");
            timertt.SetKey("subtitle", "timer_des");

            tutorialtt.SetImageDisplayBackgroundWidth(labelWidths);
            tipstt.SetImageDisplayBackgroundWidth(labelWidths);
            animationSpeedtt.SetImageDisplayBackgroundWidth(labelWidthsBig);
            timertt.SetImageDisplayBackgroundWidth(labelWidths);
        }

        void OnEnable() {
            if (GameManager.instance.isTutorial == true) {
                tutorialYes.SetColor("normalAlternate");
                tutorialNo.SetColor("normal");
            }
            else {
                tutorialYes.SetColor("normal");
                tutorialNo.SetColor("normalAlternate");
            }

            if (GameManager.instance.isTips == true) {
                tipsYes.SetColor("normalAlternate");
                tipsNo.SetColor("normal");
            }
            else {
                tipsYes.SetColor("normal");
                tipsNo.SetColor("normalAlternate");
            }

            if (GameManager.instance.animationSpeed == 1f) {
                as1.SetColor("normalAlternate");
                as2.SetColor("normal");
            }
            else {
                as1.SetColor("normal");
                as2.SetColor("normalAlternate");
            }

            if (UIManager.instance.isTimer == true) {
                timerYes.SetColor("normalAlternate");
                timerNo.SetColor("normal");
            }
            else {
                timerYes.SetColor("normal");
                timerNo.SetColor("normalAlternate");
            }
        }

        public void SetTutorial(bool value) {
            GameManager.instance.isTutorial = value;

            if (value == true) {
                tutorialYes.SetColor("normalAlternate");
                tutorialNo.SetColor("normal");
            }
            else {
                tutorialYes.SetColor("normal");
                tutorialNo.SetColor("normalAlternate");
            }
        }

        public void SetTips(bool value) {
            GameManager.instance.isTips = value;

            if (value == true) {
                tipsYes.SetColor("normalAlternate");
                tipsNo.SetColor("normal");
            }
            else {
                tipsYes.SetColor("normal");
                tipsNo.SetColor("normalAlternate");
            }
        }

        public void SetAnimationSpeed(float value) {
            GameManager.instance.animationSpeed = value;

            if (value == 1f) {
                as1.SetColor("normalAlternate");
                as2.SetColor("normal");
            }
            else {
                as1.SetColor("normal");
                as2.SetColor("normalAlternate");
            }
        }

        public void SetTimer(bool value) {
            UIManager.instance.isTimer = value;

            if (UIManager.instance.isTimer == true) {
                timerYes.SetColor("normalAlternate");
                timerNo.SetColor("normal");
            }
            else {
                timerYes.SetColor("normal");
                timerNo.SetColor("normalAlternate");
            }
        }
    }
}