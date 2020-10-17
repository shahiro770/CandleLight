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

        private TimeSpan timeCounter;
        private bool timerGoing;

        private float elapsedTime;

        public void ResetTimer() {
            timeDisplay.text = "00:00:00.00";
            elapsedTime = 0f;
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
                if (timeCounter.TotalMinutes >= 7) {
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