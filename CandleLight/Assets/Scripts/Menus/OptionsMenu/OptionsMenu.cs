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

        public TooltipTextMesh effectstt;           /// <value> sound effects text tooltip </value>
        public TooltipTextMesh musictt;             /// <value> music text tooltip </value>
        public TooltipTextMesh tutorialtt;          /// <value> tutorial text tooltip </value>
        public TooltipTextMesh tipstt;              /// <value> tips text tooltip </value>
        public TooltipTextMesh animationSpeedtt;    /// <value> animation speed text tooltip </value>
        public TooltipTextMesh timertt;             /// <value> timer text tooltip </value>

        private float labelWidths = 180f;
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

            tutorialtt.SetKey("title", "tutorial_title");
            tutorialtt.SetKey("subtitle", "tutorial_des");

            tutorialtt.SetImageDisplayBackgroundWidth(labelWidths);
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
    }
}