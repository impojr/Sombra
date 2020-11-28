using System.Collections;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCaught : Singleton<PlayerCaught>
    {
        public delegate void Caught();
        public static event Caught OnCaught;

        public void Detected()
        {
            OnCaught?.Invoke();
        }
    }
}
