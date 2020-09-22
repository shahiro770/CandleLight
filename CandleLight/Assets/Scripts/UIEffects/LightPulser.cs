/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The LightPulser class is used to make a light source's alpha value change over time
*/

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace UIEffects {

    public class LightPulser : MonoBehaviour {

        public Light2D l;
        public float minInt = 0f;
        public float maxInt = 2f;
        public float lerpSpeed = 0.02f;

        private float percentageComplete = 0f;

        void Start() {
            l.intensity = minInt;
        }
       
        void Update() {
            percentageComplete += lerpSpeed;
            l.intensity = Mathf.Lerp(minInt, maxInt, percentageComplete);
            if (percentageComplete >= 1 || percentageComplete <= 0) {
                lerpSpeed *= -1;
            }
        }
    }
}