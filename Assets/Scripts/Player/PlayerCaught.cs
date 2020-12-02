using System.Collections;
using Assets.Scripts.Helpers;
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
            OnCaught?.Invoke();
        }

        public void AnimEnded()
        {
            Debug.Log("ANIM ENDED CALLED");
            StartCoroutine(InvokeAnimEndedEvent());
        }

        private IEnumerator InvokeAnimEndedEvent()
        {
            yield return new WaitForSeconds(0.5f);
            OnCaughtAnimEnded?.Invoke();
        }
    }
}
