/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The ClassSelectMenu class is used to control all elements on the class select menu UI.
* It is the screen where the user chooses their class (Warrior, Mage, Archer, Rogue) to start the
* game with.
* It stores information when the user clicks a ClassButton, and visually updates other UI
* gameObjects in it.
*
*/

using ClassConstants = Constants.ClassConstants;
using General;
using Party;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menus.ClassSelectMenu {

    public class ClassSelectMenu : MonoBehaviour {
    
        /* external component references */
        public Button selectButton;                 /// <value> Confirmation button  </value>
        public Button firstToSelect;                /// <value> First button to select on enabling </value>
        public Button selectedCompButton;           /// <value> Currently selected composition button </value>
        public ButtonTransitionState sbBts;         /// <value> Confirmation button's visual state controller </value>   
        public ButtonTransitionState[] classBtss;   /// <value> List of all class buttons </value>
        public ButtonTransitionState[] compBtss;    /// <value> List of all composition buttons </value>
        public ClassInfo classInfo;                 /// <value> Displays information of a class </value>
        public SpriteRenderer[] compSprites;        /// <value> Sprites currently visible in the composition buttons </value>

        private EventSystem es;                     /// <value> eventSystem reference </value>
        private Sprite warriorIcon;
        private Sprite mageIcon;
        private Sprite archerIcon;
        private Sprite rogueIcon;
       
        private string[] partyComposition;    /// <value> </value>  
        private int compIndex = 0;
        private int numPartyMembers = 0;
        private bool selectButtonEnabled = false;   /// <value> Select button will only move to next scene if a class is selected </value>

        /// <summary>
        /// Awake to intialize eventSystem and select button's alternate colour blocks
        /// </summary> 
        void Awake() {
            es = EventSystem.current;

            ColorBlock sbEnabledBlock = selectButton.colors; 
            ColorBlock btsPressedBlock = selectButton.colors;   // arbitrary, just need a color block
            ColorBlock btsNormalBlock = selectButton.colors;    // arbitrary, just need a color block
            sbEnabledBlock.normalColor = new Color32(255, 255, 255, 100);
            sbEnabledBlock.highlightedColor = new Color32(133, 133, 133, 255);
            sbEnabledBlock.pressedColor = new Color32(255, 255, 255, 255);

            btsPressedBlock.normalColor = new Color32(255, 255, 255, 255);
            btsPressedBlock.highlightedColor = new Color32(255, 255, 255, 255);
            btsPressedBlock.pressedColor = new Color32(255, 255, 255, 255);
            btsPressedBlock.disabledColor = new Color32(61, 61, 61, 255);
            
            foreach(ButtonTransitionState bts in compBtss) {
                bts.SetColorBlock("normal", btsNormalBlock);
                bts.SetColorBlock("normalAlternate", btsPressedBlock);   
            }
            foreach(ButtonTransitionState bts in classBtss) {
                bts.SetColorBlock("normal", btsNormalBlock);
                bts.SetColorBlock("normalAlternate", btsPressedBlock);
            }

            warriorIcon = Resources.Load<Sprite>("Sprites/Classes/WarriorIcon");
            mageIcon = Resources.Load<Sprite>("Sprites/Classes/MageIcon");
            archerIcon = Resources.Load<Sprite>("Sprites/Classes/ArcherIcon");
            rogueIcon = Resources.Load<Sprite>("Sprites/Classes/RogueIcon");

            sbBts.SetColorBlock("normalAlternate", sbEnabledBlock);
        }

        /// <summary>
        /// OnEnable to select first class button and revert previous selections due to switching menus
        /// </summary> 
        void OnEnable() {
            partyComposition = new string[] {"", ""};
            numPartyMembers = 0;
            compIndex = 0;

            foreach (ButtonTransitionState bts in compBtss) {
                bts.SetColor("normal");
            }
            foreach (ButtonTransitionState bts in classBtss) {
                bts.SetColor("normal");
            }
            foreach (SpriteRenderer sr in compSprites) {
                sr.sprite = null;
            }
            
            SelectCompositionButton(compIndex);
            
            classInfo.SetClassInfo("");
            if (selectButtonEnabled) {
                SetSelectButtonEnabled(false);
            }
        }

        /// <summary>
        /// Visually show that a class has been selected.
        /// Changes a class button's sprite state to pressed and enables the select button.
        /// Also changes the displayed class info.
        /// </summary> 
        /// <param name="cb"> Class button to change appearance of </param>
        public void SelectClassButton(int index) {
            if (partyComposition[compIndex] == "") {
                numPartyMembers++;
            }

            for (int i = 0; i < classBtss.Length; i++) {
                if (i == index) {
                    classBtss[i].SetColor("normalAlternate"); 
                }
                else {
                    classBtss[i].SetColor("normal");                  
                }
            }

            classBtss[index].SetColor("normalAlternate");

            if (index == 0) {
                partyComposition[compIndex] = ClassConstants.WARRIOR;
                compSprites[compIndex].sprite = warriorIcon;
            }
            else if (index == 1) {
                partyComposition[compIndex] = ClassConstants.MAGE;
                compSprites[compIndex].sprite = mageIcon;
            }
            else if (index == 2) {
                partyComposition[compIndex] = ClassConstants.ARCHER;
                compSprites[compIndex].sprite = archerIcon;
            }
            else if (index == 3) {
                partyComposition[compIndex] = ClassConstants.ROGUE;
                compSprites[compIndex].sprite = rogueIcon;
            }

            classInfo.SetClassInfo(partyComposition[compIndex]);
            SetSelectButtonEnabled(numPartyMembers >= 2);
        }

        public void SelectCompositionButton(int index) {
            int correspondingClassIndex = -1;
            for (int i = 0; i < compBtss.Length; i++) {
                if (i == index) {
                    compBtss[i].SetColor("normalAlternate"); 
                    compIndex = index;
                }
                else {
                    compBtss[i].SetColor("normal");                  
                }
            }

            if (partyComposition[index] == ClassConstants.WARRIOR) {
                correspondingClassIndex = 0;
            }
            else if (partyComposition[index] == ClassConstants.MAGE) {
                correspondingClassIndex = 1;
            }
            else if (partyComposition[index] == ClassConstants.ARCHER) {
                correspondingClassIndex = 2;
            }
            else if (partyComposition[index] == ClassConstants.ROGUE) {
                correspondingClassIndex = 3;
            }

            for (int i = 0; i < classBtss.Length; i++) {
                if (i == correspondingClassIndex) {
                    classBtss[i].SetColor("normalAlternate");       
                }
                else {
                    classBtss[i].SetColor("normal");                  
                }
            }
            
            if (correspondingClassIndex == -1) {
                classInfo.SetClassInfo("");
            }
            else {
                classInfo.SetClassInfo(partyComposition[compIndex]);
            }
        }

        /// <summary>
        /// Rotates from the current comp button to another by a set amount
        /// </summary>
        /// <param name="amount"> Number of indices to rotate by </param>
        public void RotateSelectedCompIndex(int amount) {
            compIndex += amount;
            if (compIndex == compBtss.Length ) {
                compIndex = 0;
            }
            else if (compIndex < 0) {
                compIndex = compBtss.Length - 1;
            }

            SelectCompositionButton(compIndex);
        }

        /// <summary>
        /// Toggle select button's colouring to show if its enabled or disabled
        /// </summary> 
        public void SetSelectButtonEnabled(bool value) {
            selectButtonEnabled = value;

            if (value == false) {
                sbBts.SetColor("normal");
            }
            else {
                sbBts.SetColor("normalAlternate");
            }
        }

        /// <summary>
        /// Starts the game by loading the next world map scene
        /// </summary> 
        /// <remark> TODO: World map scene still has to be made </remark>
        public void BeginGame() {
            if (selectButtonEnabled) {
                PartyManager.instance.ResetGame();
                foreach (string pm in partyComposition) {
                    PartyManager.instance.AddPartyMember(pm);
                }

                GameManager.instance.LoadAreaScene("GreyWastes");
            }
        }
    }
}
