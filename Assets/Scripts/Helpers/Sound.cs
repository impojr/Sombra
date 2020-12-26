using Assets.Scripts.Constants;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [System.Serializable]
    public class Sound
    {
        public AudioClipName name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        [Range(0.1f, 3f)] public float pitch;
        public bool loop;

        private AudioSource _source;

        public void SetAudioSource(AudioSource source)
        {
            _source = source;
        }

        public AudioSource GetAudioSource()
        {
            return _source;
        }
    }
}
