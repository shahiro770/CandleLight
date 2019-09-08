/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The RewardsPanel class is used to display all the results of combat. This includes
* EXP earned, items and wax found, and highlights of combat (such as who did the most damage).
*
*/

using Characters;
using Localization;
using PanelConstants = Constants.PanelConstants;
using Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class RewardsPanel : Panel {

        /* external component references */
        public CanvasGroup imgCanvas;   /// <value> Alpha controller for display </value>
        public PartyMemberDisplay[] pmDisplays = new PartyMemberDisplay[4]; /// <value> Array of partyMemberDisplays </value>
        public MonsterResultsDisplay[] monsterResultDisplays = new MonsterResultsDisplay[5];    /// <value> Array of monsterResultDisplays</value>
        public LocalizedText amountTextEXP; /// <value> Display of total EXP earned in combat instance </value>
        public LocalizedText amountTextWAX; /// <value> Display of total WAX earned in combat instance </value>
        
        private Monster[] monstersToDisplay;    /// <value> Monsters to display in monsterResultDisplays </value>
        private float lerpSpeed = 4;    /// <value> Speed at which rewardsPanel fades in and out </value>
        private int[] monsterCounts;    /// <value> Number of each monster in each monsterResultDisplays killed</value>
        private int amountEXP;      /// <value> Total EXP earned in combat </value>
        private int amountWAX;      /// <value> Total WAX earned in combat </value>

        /// <summary>
        /// Initializes partyMemberDisplays and monsterResultDisplays, and sets text values to empty
        /// </summary>
        /// <param name="pms"> PartyMembers list (max count of 4) </param>
        /// <param name="monstersKilled"> Monsters list (max unique monsters of 5) </param>
        public IEnumerator Init(List<PartyMember> pms, List<Monster> monstersKilled) {
            for (int i = 0; i < pmDisplays.Length; i++) {
                pmDisplays[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < monsterResultDisplays.Length;i++) {
                monsterResultDisplays[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < pms.Count; i++) {
                pmDisplays[i].gameObject.SetActive(true);
                pmDisplays[i].InitRewardsDisplay(pms[i].pmvc);
            }

            monsterCounts = new int[5];
            monstersToDisplay = new Monster[5];
            amountEXP = 0;
            amountWAX = 0;
            amountTextEXP.SetText("");
            amountTextWAX.SetText("");
            yield return (StartCoroutine(DisplayMonstersDisplays(pms, monstersKilled)));
            yield return (StartCoroutine(UpdateEXPBars(pms)));
        }

        /// <summary>
        /// Displays the monsterResultDisplays, initializing them
        /// </summary>
        /// <param name="pms"> List of partyMembers </param>
        /// <param name="monstersKilled"> List of monsters killed </param>
        /// <returns> IEnumerator for smooth animation</returns>
        private IEnumerator DisplayMonstersDisplays(List<PartyMember> pms ,List<Monster> monstersKilled) {
            for (int i = 0; i < monstersKilled.Count; i++) {
                for (int j = 0; j < monstersToDisplay.Length; j++) {
                    if (monsterCounts[j] == 0) {
                        monsterCounts[j]++;
                        monstersToDisplay[j] = monstersKilled[i];
                        break;
                    }
                    else if (monstersToDisplay[j].monsterDisplayName == monstersKilled[i].monsterDisplayName) {
                        monsterCounts[j]++;
                        break;
                    } 
                }
            }

            for (int i = 0; i < monstersToDisplay.Length; i++) {
                if (monsterCounts[i] > 0) {
                    monsterResultDisplays[i].gameObject.SetActive(true);
                    yield return (StartCoroutine(monsterResultDisplays[i].Init(monstersToDisplay[i], monsterCounts[i])));
                    amountEXP += monstersToDisplay[i].EXP * monsterCounts[i];
                    amountWAX += monstersToDisplay[i].WAX * monsterCounts[i];
                }
            }

            amountTextEXP.SetText(amountEXP.ToString());
            amountTextWAX.SetText(amountWAX.ToString());
            PartyManager.instance.AddWAX(amountWAX);
        }

        /// <summary>
        /// Updates the EXP bars of each partyMemberDisplay
        /// </summary>
        /// <param name="pms"></param>
        private IEnumerator UpdateEXPBars(List<PartyMember> pms) {
            PartyManager.instance.AddEXP(amountEXP);

            while (!pms[0].doneEXPGaining) {   // wait for bars to finish filling 
                yield return null;
            }
        }

        /// <summary>
        /// Makes the rewardsDisplay visible
        /// </summary>
        /// <param name="value"></param>
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
        /// Fades the rewardsPanel to the target alpha
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        private IEnumerator Fade(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = imgCanvas.alpha;

            while (imgCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                imgCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                yield return new WaitForEndOfFrame();
            }

            if (targetAlpha == 0) {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.REWARDSPANEL;
        }

        /// <summary>
        /// Returns the Button that adjacent panels will navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public override Button GetNavigatableButton() {
            return null; // will need to change this later
        }
    }
}