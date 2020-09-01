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
using UnityEngine.UI;

namespace Menus.ClassSelectMenu {

    public class DifficultyMenu : MonoBehaviour {
    
        /* external component references */
        public Button beginButton;
        public ButtonTransitionState[] difficultyBtss;   /// <value> List of all class buttons </value> 
        public ClassSelectMenu csm;
        public LocalizedText difficultyTitle;            /// <value> Title of a difficulty </value>
        public LocalizedText difficultyDes;              /// <value> Displays information of a difficulty </value>
       
        public string[] partyComposition;                /// <value> </value>  

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
        }

        /// <summary>
        /// OnEnable to select first class button and revert previous selections due to switching menus
        /// </summary> 
        void OnEnable() {
            partyComposition = csm.partyComposition;
            if (GameManager.instance.difficultyModifier == 1f) {
                SelectDifficultyButton(1);
            }
            else {
                SelectDifficultyButton(0);
            }
        }

        /// <summary>
        /// Visually show that a class has been selected.
        /// Changes a class button's sprite state to pressed and enables the select button.
        /// Also changes the displayed class info.
        /// </summary> 
        /// <param name="cb"> Class button to change appearance of </param>
        public void SelectDifficultyButton(int index) {
            if (index == 0) {
                GameManager.instance.difficultyModifier = 0.75f;
                difficultyTitle.SetKey("normal_title");
                difficultyDes.SetKey("normal_des");
            }
            else {
                GameManager.instance.difficultyModifier = 1f;
                difficultyTitle.SetKey("hard_title");
                difficultyDes.SetKey("hard_des");
            }

            for (int i = 0; i < difficultyBtss.Length; i++) {
                if (i == index) {
                    difficultyBtss[i].SetColor("na0"); 
                }
                else {
                    difficultyBtss[i].SetColor("normal");                  
                }
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
            // partyComposition is going to be length 0 anyways on scene load

            GameManager.instance.StartLoadNextScene("Area");
        }
    }
}
