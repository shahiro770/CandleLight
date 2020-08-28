using System;
using System.Collections; 
using UnityEngine;

namespace Audio {

    public class AudioManager: MonoBehaviour {

        public static AudioManager instance;    /// <value> Singleton </value>

        public Sound currentBGM = null;
        public Sound[] sfxs;
        public Sound[] bgms;
        public float bgmVolume;
        public float sfxVolume;
        public bool shouldChangeBGM = false;    /// <value> Flag for if the BGM should change </value>

        private Coroutine fadeInner;        /// <value> Store the coroutine responsible for fading in to stop it if audio suddenly changes </value>
        private Coroutine fadeOuter;        /// <value> Store the coroutine responsible for fading out to stop it if audio suddenly changes </value>
        private float lerpSpeed = 1f;    /// <value> Speed at which audio fades </value>

        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }

            foreach (Sound s in sfxs) {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop; 
            }

            foreach (Sound s in bgms) {
                s.source = gameObject.AddComponent<AudioSource>();
                s.loop = true;
                s.pitch = 1;
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop; 
            }
        }

        public void PlaySFX (string soundName) {
            Sound s = Array.Find(sfxs, sound => sound.name == soundName);
            if (s == null) {
                Debug.LogWarning("Sound " + soundName + " does not exist");
            }
            else {
                s.source.volume = sfxVolume;
                s.source.Play();
            }
        }

        /// <summary>
        /// Change the BGM
        /// </summary>
        /// <param name="soundName"></param>
        public void PlayBGM(string soundName) {
            if (shouldChangeBGM == true) {
                shouldChangeBGM = false;
            }
            else {
                if (currentBGM != null && currentBGM.source != null) {
                    fadeOuter = StartCoroutine(FadeCurrentBGMVolume(currentBGM, 0));
                }

                Sound s = Array.Find(bgms, sound => sound.name == soundName);
                if (s == null) {
                    Debug.LogWarning("Sound " + soundName + " does not exist");
                }
                else {
                    s.source.loop = true;
                    s.source.Play();
                    currentBGM = s;
                    fadeInner = StartCoroutine(FadeCurrentBGMVolume(currentBGM, bgmVolume));
                }
            }
        }

        /// <summary>
        /// Change the BGM's volume
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBGMVolume(float volume) {
            bgmVolume = volume;
            if (currentBGM.source != null) {
                currentBGM.source.volume = volume;
            }
        }

        private IEnumerator FadeCurrentBGMVolume(Sound s, float targetVolume) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevVolume = s.source.volume;

            while (s.source.volume != targetVolume) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                s.source.volume = Mathf.Lerp(prevVolume, targetVolume, percentageComplete);

                yield return new WaitForEndOfFrame();
            }

            if (s.source.volume == 0) {
                s.source.Stop();
            }
        }
    }
}