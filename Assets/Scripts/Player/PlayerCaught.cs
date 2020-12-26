using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCaught : Singleton<PlayerCaught>
    {
        public delegate void Caught();
        public static event Caught OnCaught;

        public delegate void CaughtAnimEnded();
        public static event CaughtAnimEnded OnCaughtAnimEnded;

        public void Detected()
        {
            AudioManager.Instance.Play(AudioClipName.Caught);
            LevelManager.Instance.timesCaught++;
            OnCaught?.Invoke();
        }

        public void AnimEnded()
        {
            StartCoroutine(InvokeAnimEndedEvent());
        }

        private IEnumerator InvokeAnimEndedEvent()
        {
            yield return new WaitForSeconds(0.5f);
            OnCaughtAnimEnded?.Invoke();
        }
    }
}
