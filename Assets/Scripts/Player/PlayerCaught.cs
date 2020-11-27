using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCaught : MonoBehaviour
    {
        public delegate void Caught();
        public static event Caught OnCaught;

        private Vector2 _initialPos;

        private void Start()
        {
            _initialPos = transform.position;
        }

        public void Detected()
        {
            transform.position = _initialPos;
            OnCaught?.Invoke();
        }
    }
}
