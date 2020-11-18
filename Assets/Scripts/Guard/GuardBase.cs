using UnityEngine;

namespace Assets.Scripts.Guard
{
    public abstract class GuardBase : MonoBehaviour
    {
        public bool playerDetected;
        public bool hacked;

        public void Initialize()
        {
            playerDetected = false;
            hacked = false;
        }

        public void Hacked()
        {

        }
    }
}
