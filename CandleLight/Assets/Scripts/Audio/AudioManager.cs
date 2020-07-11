using System;
using UnityEngine.Audio;
using UnityEngine;

namespace Audio {

    public class AudioManager: MonoBehaviour {

        public static AudioManager instance;    /// <value> Singleton </value>

        public Sound[] sounds;

        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }

            foreach (Sound s in sounds) {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop; 
            }
        }

        public void Play (string soundName) {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null) {
                Debug.LogError("Sound " + soundName + " does not exist");
            }
            else {
                s.source.Play();
            }
        }
    }
}