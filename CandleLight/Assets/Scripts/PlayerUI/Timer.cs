/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: September 28, 2019
* 
* The Timer class is used to show the player how long their run has been taking
*
*/
using aromaConstants = Constants.AromaConstants.aromaConstants;
using EventManager = Events.EventManager;
using GameManager = General.GameManager;
using System.Collections;
using TimeSpan = System.TimeSpan;
using TMPro;
using UnityEngine;

namespace PlayerUI {

    public class Timer : MonoBehaviour {

        public CanvasGroup cg;
        public TextMeshProUGUI timeDisplay;

        private Color32 aromaColour = new Color32(125, 237, 164, 255);
        private Color32 warningColour = new Color32(234, 50, 60, 255);
        private TimeSpan timeCounter;
        private float elapsedTime;
        private bool timerGoing;
        private bool swappedColours = false;
       

        public void ResetTimer() {
            timeDisplay.text = "00:00:00.00";
            elapsedTime = 0f;
            if (swappedColours == true) {   // for aroma timers that have swapped colours, swap them back
                timeDisplay.color = aromaColour;
                swappedColours = false;
            }
        }

        public void StartTimer(bool value) {
            if (value == true) {                
                timerGoing = true;

                StartCoroutine(UpdateTimer());
            }
            else {
                timerGoing = false;
            }   
        }

        public void StartTimerAroma(bool value) {
            if (value == true) {                
                timerGoing = true;

                StartCoroutine(UpdateTimerAroma());
            }
            else {
                timerGoing = false;
            }   
        }

        public void SetVisible(bool value) {
            cg.alpha = value == true ? 1 : 0;

            if (GameManager.instance.gsData.aromas[(int)aromaConstants.WILTINGWINTERGREEN] == true) {
                if (value == true) {
                    EventManager.instance.aromaTimer.gameObject.transform.localPosition = new Vector3(-390, -100);
                }
                else {
                    EventManager.instance.aromaTimer.gameObject.transform.localPosition = new Vector3(-390, -130);
                }
            }
        }

        public void SetVisibleAromaTimer(bool value) {
            cg.alpha = value == true ? 1 : 0;
        }

        public IEnumerator UpdateTimer() {
            while (timerGoing) {
                elapsedTime += Time.deltaTime;
                timeCounter = TimeSpan.FromSeconds(elapsedTime);
                
                timeDisplay.text = timeCounter.ToString(@"hh\:mm\:ss\.ff");
                if (timeCounter.TotalDays >= 1) {
                    timeDisplay.text = "Too Long";
                }

                yield return null;
            }
        }

        public IEnumerator UpdateTimerAroma() {
            while (timerGoing) {
                elapsedTime += Time.deltaTime;
                timeCounter = TimeSpan.FromSeconds(elapsedTime);
                
                timeDisplay.text = timeCounter.ToString(@"hh\:mm\:ss\.ff");
                if (timeCounter.TotalMinutes >= 6 && swappedColours == false) {
                    timeDisplay.color = warningColour;
                    swappedColours = true;
                }
                else if (timeCounter.TotalMinutes >= 7) {
                    StartCoroutine(EventManager.instance.DisplayGameOver(false));
                }

                yield return null;
            }
        }

        public string GetTime() {
            return timeCounter.ToString(@"hh\:mm\:ss\.ff");
        }
        
        public float GetElapsedTime() {
            return elapsedTime;
        }

        public void SetElapseTimed(float t) {
            elapsedTime = t;
        }
    }
}