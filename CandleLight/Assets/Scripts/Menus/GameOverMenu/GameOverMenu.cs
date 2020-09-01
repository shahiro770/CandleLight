/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The GameOverMenu class shows stats and other stuff at the end of a run
*/

using achievementConstants = Constants.AchievementConstants.achievementConstants;
using EventManager = Events.EventManager;
using General;
using Localization;
using PartyManager = Party.PartyManager;
using System.Collections;
using System.Collections.Generic;
using TimeSpan = System.TimeSpan;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.GameOverMenu {

    public class GameOverMenu : MonoBehaviour {

        /* external component references */
        public CanvasGroup cg;                  /// <value> Canvas that displays all elements </value>
        public Image frontFill;                 /// <value> Image that displays overtop of progress bar's background </value>
        public LocalizedText gameOverTitle;
        public LocalizedText gameOverDes;
        public LocalizedText enemiesAmount;
        public LocalizedText WAXAmount;
        public LocalizedText eventsAmount;
        public LocalizedText timeAmount;
        public LocalizedText scoreTitle;
        public LocalizedText scoreAmount;
        public ProgressBarIcon[] pbis;          /// <value> List of progress bar icons (TODO: Make a pool of these) </value>
       
        private float lerpSpeed = 3.5f;         /// <value> Speed to lerp with </value>
        private float fillAmount;               /// <value> Percentage that bar should be filled </value>
        private float midPointBarSeg;           /// <value> Length of each segment between progressBarIcons </value>
        private bool isWin;                     /// <value> True if this game ended in victory </value>

        /// <summary>
        /// Initializes all properties so the gameOverMenu can show the correct info
        /// </summary>
        /// <param name="isWin"> true if game was won (player reached last subArea), false otherwise </param>
        /// <param name="midPoints"> List of subArea indexes that were visited </param>
        /// <param name="subAreaProg"> Current subArea's progress</param>
        public void Init(bool isWin, List<int> midPoints, string areaName, int subAreaIndex, int subAreaProg,  string timeString) {
            gameObject.SetActive(true);
            this.isWin = isWin;

            if (isWin == true) {
                gameOverTitle.SetKey("game_completed_title");
                gameOverDes.SetKey("game_completed_des");
                scoreTitle.SetKey("final_score_title");
            }
            else {
                gameOverTitle.SetKey("game_over_title");
                gameOverDes.SetKey("game_over_" + areaName + subAreaIndex);
                scoreTitle.SetKey("score_title");
            }

            /*
                First icon will be the start, last icon will be the end (so only midpoints need to be positioned and have their sprites changed).
                Note that for now, the start and end icons are hardcoded via the scene (TODO: Change that)
            */
            float midPointDist = (int)(frontFill.rectTransform.sizeDelta.x / (midPoints.Count + 1));
            for (int i = 0; i < midPoints.Count; i++) {
                pbis[i + 1].SetPosition((int)midPointDist * (i + 1), 0);
                pbis[i + 1].SetSprite(Resources.Load<Sprite>("Sprites/Menu/" + GameManager.instance.areaName + "Icons/" + midPoints[i]));
                if (midPoints[i] != -1) {   // leave progressBarICons as ? if they were not reached
                    fillAmount += midPointDist / frontFill.rectTransform.sizeDelta.x;
                }
            }  
            midPointBarSeg = 1f / (midPoints.Count + 1);
            fillAmount += (subAreaProg / 100f) * (1f / (midPoints.Count + 1));

            enemiesAmount.SetText(GameManager.instance.enemiesKilled.ToString());
            WAXAmount.SetText(GameManager.instance.WAXobtained.ToString());
            eventsAmount.SetText(GameManager.instance.totalEvents.ToString());
            timeAmount.SetText(timeString);
            scoreAmount.SetText(CalculateScore().ToString());

            if (GameManager.instance.achievementsUnlocked[(int)achievementConstants.ABSOLUTEMASTERY] == false && isWin == true
            && GameManager.instance.difficultyModifier == 1f) {
                string[] partyComp = PartyManager.instance.GetPartyCompositionSorted();
                for (int i = 0; i < GameManager.instance.partyCombos.GetLength(0); i++) {
                    bool match = true;
                    for (int j = 0; j < partyComp.Length; j++) {
                        if (GameManager.instance.partyCombos[i,j] != partyComp[j]) {
                            match = false;
                            break;
                        } 
                    }
                    if (match == true) {
                        GameManager.instance.partyCombos[i, 0] = null;  // setting first entry in a combo to null means its been completed
                        break;
                    }
                }
                for (int i = 0 ; i < GameManager.instance.partyCombos.GetLength(0); i++) {
                    if (GameManager.instance.partyCombos[i, 0] != null) {
                        break;
                    }
                    else if (i == GameManager.instance.partyCombos.GetLength(0) - 1) {
                        GameManager.instance.achievementsUnlocked[(int)achievementConstants.ABSOLUTEMASTERY] = true;
                        EventManager.instance.SetAchievementNotification((int)achievementConstants.ABSOLUTEMASTERY, true);
                    }
                }  
            }
            if (GameManager.instance.achievementsUnlocked[(int)achievementConstants.NOTIME] == false && isWin == true 
            && GameManager.instance.difficultyModifier == 1f) {
                if (TimeSpan.FromSeconds(GameManager.instance.timeTaken).TotalSeconds < 720) {
                    print(TimeSpan.FromSeconds(GameManager.instance.timeTaken).TotalSeconds);
                    GameManager.instance.achievementsUnlocked[(int)achievementConstants.NOTIME] = true;
                    EventManager.instance.SetAchievementNotification((int)achievementConstants.NOTIME, true);
                }      
            }

            GameManager.instance.AddHighScoreData(CalculateScore(), subAreaIndex);
        }
        
        /// <summary>
        /// Calculates the player's final score
        /// </summary>
        /// <returns></returns>
        private int CalculateScore() {
            int score = GameManager.instance.enemiesKilled * 10 + GameManager.instance.WAXobtained * 20 + GameManager.instance.totalEvents * 5;
            if (isWin) {
                score += 1000;
            }
            if (GameManager.instance.difficultyModifier == 1f) {
                score *= 2;
            }

            return score;
        }

        /// <summary>
        /// Sets the gameOverMenu's visibility
        /// </summary>
        /// <param name="value"> True if visible, false otherwise </param>
        public void SetVisible(bool value) {
            if (value == true) {
                StartCoroutine(FadeGameOverScreen(1));
            }
            else {
                StartCoroutine(FadeGameOverScreen(0));
            }
        }

        /// <summary>
        /// Changes the alpha of the gameOverscreen, as well as the progressBarIcons
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        private IEnumerator FadeGameOverScreen(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = cg.alpha;
            float newAlpha;

            /*
                IMPORTANT: sprites with animators can't have their alpha physically changed, so an animation
                that changes the alpha in the exact same number of frames is used (if the lerp time changes, the animation
                will also have to change)
            */
            if (targetAlpha == 1) {
                foreach(ProgressBarIcon pbi in pbis) {
                    pbi.PlayAnimation("show");
                }
            }
            else {
                foreach(ProgressBarIcon pbi in pbis) {
                    pbi.PlayAnimation("hide");
                }
            }
            while (cg.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                newAlpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                cg.alpha = newAlpha;
                
                yield return new WaitForEndOfFrame();
            }
            if (targetAlpha == 1) {
                yield return StartCoroutine(Fill());
            }
            if (targetAlpha == 0) {       
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Fills the progress bar, triggering a progressBarICon's animations should the progress pass it
        /// This means final boss subAreas will show the icon, but not trigger an animation
        /// </summary>
        /// <returns></returns>
        private IEnumerator Fill() {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = Time.deltaTime * lerpSpeed;

            int currMidPoint = 1;

            if (frontFill.fillAmount != fillAmount) {      
                while (timeSinceStarted < 2) {  // lerping is approaching infinity so stop after 3 seconds         
                    timeSinceStarted = Time.time - timeStartedLerping;
                    percentageComplete = Time.deltaTime * lerpSpeed;

                    frontFill.fillAmount = Mathf.Lerp(frontFill.fillAmount, fillAmount, percentageComplete); // NOTE THIS IS BUGGED (if you die at 0% progress, the number is wrong)
                    if (System.Math.Round(frontFill.fillAmount, 2) >= midPointBarSeg * currMidPoint) {
                        pbis[currMidPoint].PlayAnimation("pop");
                        currMidPoint++;
                    }
                    yield return new WaitForEndOfFrame();        
                }
            }
        }
    }
}