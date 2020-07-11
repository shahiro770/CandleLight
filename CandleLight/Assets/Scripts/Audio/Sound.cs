using UnityEngine;
using UnityEngine.Audio;

namespace Audio {
    
    [System.Serializable]
    public class Sound {

        public string name;
        
        /* external component references */
        public AudioClip clip;
        [HideInInspector]
        public AudioSource source;

        [Range (0f, 1f)]
        public float volume;
        [Range (0.1f, 3f)]
        public float pitch;
        public bool loop;
    }
}
