/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The DifficultyMenu is where the player can choose the difficulty for their run
*
*/

using General;
using Localization;
using Party;
using UIEffects;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

namespace Menus.ClassSelectMenu {

    public class DifficultyMenu : MonoBehaviour {
    
        /* external component references */
        public Button beginButton;
        public GameObject difficultyButtons;
        public GameObject aromaButtons;
        public ButtonTransitionState[] difficultyBtss;   /// <value> List of all btss for difficulty buttons </value> 
        public ButtonTransitionState[] aromaBtss;        /// <value> List of all btss for modifier buttons </value> 
        public ClassSelectMenu csm;
        public Light2D freeForm;
        public Light2D particleLight;
        public LocalizedText menuTitle;
        public LocalizedText difficultyTitle;            /// <value> Title of a difficulty </value>
        public LocalizedText difficultyDes;              /// <value> Displays information of a difficulty </value>
        public LocalizedText scoreModifierText;          /// <value> Displays the current score modifier </value>
        public LocalizedText toggleButtonText;      
        public ParticleSystem ps;
        public SpriteRenderer[] aromas;                  /// <value> Currently displayed sprites on all aroma buttons </value>
        
        private Color[] aromaColours = new Color[] { 
            new Color32(209, 201, 255, 255), 
            new Color32(236, 100, 75, 255), 
            new Color32(65, 15, 255, 255), 
            new Color32(125, 237, 164, 255) 
        };
        public Sprite[] aromaSprites;
        public Sprite[] aromaActiveSprites;
        public string[] partyComposition;                 /// <value> store the party composition if player moves back a menu </value>  

        private float scoreModifier;
        private float difficultyModifierToBe;
        private bool[] aromasToBe; 

        /// <summary>
        /// Awake to intialize eventSystem and select button's alternate colour blocks
        /// </summary> 
        void Awake() {
            ColorBlock btsPressedBlock = beginButton.colors;   // arbitrary, just need a color block
            ColorBlock btsNormalBlock = beginButton.colors;    // arbitrary, just need a color block

            btsPressedBlock.normalColor = new Color32(255, 255, 255, 255);     
            btsPressedBlock.highlightedColor = new Color32(255, 255, 255, 255);
            btsPressedBlock.pressedColor = new Color32(255, 255, 255, 255);     
            btsPressedBlock.disabledColor = new Color32(61, 61, 61, 255);       
            
            foreach(ButtonTransitionState bts in difficultyBtss) {
                bts.SetColorBlock("normal", btsNormalBlock);
                bts.SetColorBlock("na0", btsPressedBlock);
            }

            foreach(ButtonTransitionState bts in aromaBtss) {
                bts.SetColorBlock("normal", btsNormalBlock);
                bts.SetColorBlock("na0", btsPressedBlock);
            }
        }

        /// <summary>
        /// OnEnable to select first class button and revert previous selections due to switching menus
        /// </summary> 
        void OnEnable() {
            partyComposition = csm.partyComposition;
            difficultyModifierToBe = GameManager.instance.difficultyModifier;
            aromasToBe = new bool[aromaBtss.Length];
            if (difficultyModifierToBe == 0.75f) {
                SelectDifficultyButton(0);
            }
            else {
                SelectDifficultyButton(1);
            }
            for (int i = 0; i < aromaBtss.Length; i++) {
                aromasToBe[i] = GameManager.instance.gsData.aromas[i];
                if (aromasToBe[i] == true) {
                    aromas[i].sprite = aromaActiveSprites[i];
                    aromaBtss[i].SetColor("na0");  
                }
                else {
                    aromas[i].sprite = aromaSprites[i];
                    aromaBtss[i].SetColor("normal");
                }
            }
            
            CalculateScoreModifier(true);
        }

        /// <summary>
        /// Visually show that a class has been selected.
        /// Changes a class button's sprite state to pressed and enables the select button.
        /// Also changes the displayed class info.
        /// </summary> 
        /// <param name="cb"> Class button to change appearance of </param>
        public void SelectDifficultyButton(int index) {
            if (index == 0) {
                difficultyModifierToBe = 0.75f;            
                difficultyTitle.SetKey("casual_title");
                difficultyDes.SetKey("casual_des");
            }
            else {
                difficultyModifierToBe = 1f;
                difficultyTitle.SetKey("normal_title");
                difficultyDes.SetKey("normal_des");
            }

            for (int i = 0; i < difficultyBtss.Length; i++) {
                if (i == index) {
                    difficultyBtss[i].SetColor("na0"); 
                }
                else {
                    difficultyBtss[i].SetColor("normal");                  
                }
            }

            CalculateScoreModifier(true);
        }

        /// <summary>
        /// Selects an aroma button, toggling its activity and updating visuals to match
        /// </summary>
        /// <param name="index"></param>
        public void SelectAromaButton(int index) {
            var colorOverLifetime = ps.colorOverLifetime;

            aromasToBe[index] = !aromasToBe[index];
            if (aromasToBe[index] == true) {
                aromas[index].sprite = aromaActiveSprites[index];
                aromaBtss[index].SetColor("na0");
            }
            else {
                aromas[index].sprite = aromaSprites[index];
                aromaBtss[index].SetColor("normal");
            }
            colorOverLifetime.color = aromaColours[index];
            freeForm.color = aromaColours[index];
            particleLight.color = aromaColours[index];

            difficultyTitle.SetKey("aroma" + index + "_title");
            difficultyDes.SetKey("aroma" + index + "_des");
            
            CalculateScoreModifier(true);
        }

        public void ToggleDifficultyAndAromas() {
            if (difficultyButtons.activeSelf == true) {
                difficultyButtons.SetActive(false);
                aromaButtons.SetActive(true);
                menuTitle.SetKey("aromas_menu_title");
                toggleButtonText.SetKey("difficulty_button");
                
                difficultyTitle.SetKey("aromas_title");
                difficultyDes.SetKey("aromas_des");
            }
            else {
                difficultyButtons.SetActive(true);
                aromaButtons.SetActive(false);
                menuTitle.SetKey("difficulty_menu_title");
                toggleButtonText.SetKey("aromas_button");

                if (difficultyModifierToBe == 0.75f) {
                    difficultyTitle.SetKey("casual_title");
                    difficultyDes.SetKey("casual_des");
                }
                else {
                    difficultyTitle.SetKey("normal_title");
                    difficultyDes.SetKey("normal_des");
                }
            }
        }

        /// <summary>
        /// Calculate the player's score modifier based on their difficulty and active aromas
        /// </summary>
        /// /// <param name="updateText"> update the displayed text value if true, update general save data otherwise </param>
        public void CalculateScoreModifier(bool updateText) {
            scoreModifier = 1;

            if (aromasToBe[0] == true) {
                scoreModifier += 0.5f;
            }
            if (aromasToBe[1] == true) {
                scoreModifier += 0.5f;
            }
            if (aromasToBe[2] == true) {
                scoreModifier += 1f;
            }
            if (aromasToBe[3] == true) {
                scoreModifier += 1f;
            }

            if (difficultyModifierToBe == 0.75f) {
                scoreModifier *= 0.5f;
            }

            if (updateText == true) {
                scoreModifierText.SetKeyAndAppend("score_modifier_title", "x" + scoreModifier);
            }
            else {
                GameManager.instance.gsData.scoreModifier = scoreModifier;
            }
        }

        /// <summary>
        /// Starts the game by loading the next world map scene
        /// </summary> 
        /// <remark> TODO: World map scene still has to be made </remark>
        public void BeginGame() {
            GameManager.instance.DeleteSaveData();
            foreach (string pm in partyComposition) {
                PartyManager.instance.AddPartyMember(pm);
            }
            // only modifier all gsData properties on game start (otherwise player can change game properties mid run)
            GameManager.instance.difficultyModifier = difficultyModifierToBe;
            for (int i = 0; i < aromasToBe.Length; i++) {
                GameManager.instance.gsData.aromas[i] = aromasToBe[i];
            }
            CalculateScoreModifier(false);
            
            // partyComposition is going to be length 0 anyways on scene load
            GameManager.instance.StartLoadNextScene("Area");
        }
    }
}
