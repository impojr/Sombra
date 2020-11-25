using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using UnityEngine;
using Collision = Assets.Scripts.Player.Collision;

namespace Assets.Scripts.Environment
{
    public class Door : Singleton<Door>
    {
        public bool isUnlocked;

        private void Start()
        {
            isUnlocked = false;
        }

        public void Unlock()
        {
            isUnlocked = true;
            //todo play animation
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player) && isUnlocked && Collision.Instance.onGround)
            {
                //todo start end of level sequence
            }
        }
    }
}
