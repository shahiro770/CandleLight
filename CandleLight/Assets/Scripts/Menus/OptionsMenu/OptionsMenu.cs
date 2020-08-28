/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The OptionsMenu class is used to modify aspects of the game.
*/

using Audio;
using General;
using PlayerUI;
using System.Linq;
using TutorialConstants = Constants.TutorialConstants;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.OptionsMenu {

    public class OptionsMenu : MonoBehaviour {

        /* external component references */
        public Button b;
        public ButtonTransitionState tutorialYes;
        public ButtonTransitionState tutorialNo;
        public ButtonTransitionState tipsYes;
        public ButtonTransitionState tipsNo;
        public ButtonTransitionState as1;
        public ButtonTransitionState as2;
        public ButtonTransitionState timerYes;
        public ButtonTransitionState timerNo;
        public Slider bgmSlider;
        public Slider sfxSlider;
        public CanvasGroup cg;
        public TooltipTextMesh sfxtt;               /// <value> sound effects text tooltip </value>
        public TooltipTextMesh bgmtt;               /// <value> music text tooltip </value>
        public TooltipTextMesh tutorialtt;          /// <value> tutorial text tooltip </value>
        public TooltipTextMesh tipstt;              /// <value> tips text tooltip </value>
        public TooltipTextMesh animationSpeedtt;    /// <value> animation speed text tooltip </value>
        public TooltipTextMesh timertt;             /// <value> timer text tooltip </value>
        public Timer timer;                         /// <value> Timer reference </value>

        private float labelWidths = 180f;
        private float labelWidthsBig = 230f;
        private ColorBlock optionEnabled = new ColorBlock();

        void Awake() {
            optionEnabled.normalColor = new Color32(255, 255, 255, 255);
            optionEnabled.highlightedColor = new Color32(255 ,255, 255, 255);
            optionEnabled.pressedColor = new Color32(255 ,255, 255, 200);
            optionEnabled.disabledColor = new Color32(61 ,61, 61, 255);
            optionEnabled.colorMultiplier = 1;

            tutorialYes.SetColorBlock("normal", b.colors);  // for some reason this is all bugged, so need to set the normal colour block
            tutorialNo.SetColorBlock("normal", b.colors);
            tutorialYes.SetColorBlock("na0", optionEnabled);
            tutorialNo.SetColorBlock("na0", optionEnabled);
            tipsYes.SetColorBlock("normal", b.colors);
            tipsNo.SetColorBlock("normal", b.colors);
            tipsYes.SetColorBlock("na0", optionEnabled);
            tipsNo.SetColorBlock("na0", optionEnabled);
            as1.SetColorBlock("normal", b.colors);
            as2.SetColorBlock("normal", b.colors);
            as1.SetColorBlock("na0", optionEnabled);
            as2.SetColorBlock("na0", optionEnabled);
            timerYes.SetColorBlock("normal", b.colors);
            timerNo.SetColorBlock("normal", b.colors);
            timerYes.SetColorBlock("na0", optionEnabled);
            timerNo.SetColorBlock("na0", optionEnabled);

            bgmtt.SetKey("title", "bgm_title");
            bgmtt.SetKey("subtitle", "bgm_des");
            sfxtt.SetKey("title", "sfx_title");
            sfxtt.SetKey("subtitle", "sfx_des");
            tutorialtt.SetKey("title", "tutorial_title");
            tutorialtt.SetKey("subtitle", "tutorial_des");
            tipstt.SetKey("title", "tips_title");
            tipstt.SetKey("subtitle", "tips_des");
            animationSpeedtt.SetKey("title", "as_title");
            animationSpeedtt.SetKey("subtitle", "as_des");
            timertt.SetKey("title", "timer_title");
            timertt.SetKey("subtitle", "timer_des");
            
            bgmtt.SetImageDisplayBackgroundWidth(labelWidthsBig);
            sfxtt.SetImageDisplayBackgroundWidth(labelWidthsBig);
            tutorialtt.SetImageDisplayBackgroundWidth(labelWidths);
            tipstt.SetImageDisplayBackgroundWidth(labelWidths);
            animationSpeedtt.SetImageDisplayBackgroundWidth(labelWidthsBig);
            timertt.SetImageDisplayBackgroundWidth(labelWidths);
        }

        void OnEnable() {
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                tutorialYes.SetColor("na0");
                tutorialNo.SetColor("normal");
            }
            else {
                tutorialYes.SetColor("normal");
                tutorialNo.SetColor("na0");
            }

            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true) {
                tipsYes.SetColor("na0");
                tipsNo.SetColor("normal");
            }
            else {
                tipsYes.SetColor("normal");
                tipsNo.SetColor("na0");
            }

            if (GameManager.instance.animationSpeed == 1f) {
                as1.SetColor("na0");
                as2.SetColor("normal");
            }
            else {
                as1.SetColor("normal");
                as2.SetColor("na0");
            }

            if (UIManager.instance.isTimer == true) {
                timerYes.SetColor("na0");
                timerNo.SetColor("normal");
            }
            else {
                timerYes.SetColor("normal");
                timerNo.SetColor("na0");
            }

            bgmSlider.value = AudioManager.instance.bgmVolume;
            sfxSlider.value = AudioManager.instance.sfxVolume;
        }

        /// <summary>
        /// Save settings and hide any tooltips visible 
        /// </summary>
        void OnDisable() {
            bgmtt.SetVisible(false);
            sfxtt.SetVisible(false);
            tutorialtt.SetVisible(false);
            tipstt.SetVisible(false);
            animationSpeedtt.SetVisible(false);
            timertt.SetVisible(false);
            GeneralSaveData gsData = new GeneralSaveData(null, GameManager.instance.gsData.hsds, GameManager.instance.tutorialTriggers, GameManager.instance.achievementsUnlocked, 
            GameManager.instance.partyCombos, UIManager.instance.isTimer, GameManager.instance.animationSpeed, AudioManager.instance.bgmVolume, AudioManager.instance.sfxVolume);
            GameManager.instance.SaveGeneralData(gsData);
        }

        public void SetTutorial(bool value) {
            GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] = value;

            if (value == true) {    // setting tutorial to true, will reset all tips
                GameManager.instance.tutorialTriggers = Enumerable.Repeat<bool>(true, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray();
                tutorialYes.SetColor("na0");
                tutorialNo.SetColor("normal");
            }
            else {
                tutorialYes.SetColor("normal");
                tutorialNo.SetColor("na0");
            }
        }

        public void SetTips(bool value) {
            GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] = value;

            if (value == true) {
                tipsYes.SetColor("na0");
                tipsNo.SetColor("normal");
            }
            else {
                tipsYes.SetColor("normal");
                tipsNo.SetColor("na0");
            }
        }

        public void SetAnimationSpeed(float value) {
            GameManager.instance.animationSpeed = value;

            if (value == 1f) {
                as1.SetColor("na0");
                as2.SetColor("normal");
            }
            else {
                as1.SetColor("normal");
                as2.SetColor("na0");
            }
        }

        public void SetTimer(bool value) {
            UIManager.instance.isTimer = value;
            if (timer != null) {
                timer.SetVisible(value);
            }

            if (UIManager.instance.isTimer == true) {
                timerYes.SetColor("na0");
                timerNo.SetColor("normal");
            }
            else {
                timerYes.SetColor("normal");
                timerNo.SetColor("na0");
            }
        }

        public void SetBgmVolume(float value) {
            AudioManager.instance.ChangeBGMVolume(value);
        }

        public void SetSfxVolume(float value) {
            AudioManager.instance.sfxVolume = value;
        }
    }
}