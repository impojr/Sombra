using System.Linq;
using System.Security.Cryptography;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;

        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (FindObjectsOfType<AudioManager>().Length > 1)
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);

            foreach (var sound in sounds)
            {
                sound.SetAudioSource(gameObject.AddComponent<AudioSource>());
                var source = sound.GetAudioSource();
                source.clip = sound.clip;
                source.volume = sound.volume;
                source.pitch = sound.pitch;
                source.loop = sound.loop;
                source.playOnAwake = false;
            }
        }

        void Start()
        {
            Play(AudioClipName.Theme);
        }

        public void Play(AudioClipName clipName)
        {
            sounds.First(x => x.name == clipName)?.GetAudioSource().Play();
        }

        public void Stop(AudioClipName clipName)
        {
            sounds.First(x => x.name == clipName)?.GetAudioSource().Stop();
        }
    }
}
