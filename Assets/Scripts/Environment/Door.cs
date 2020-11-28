using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using UnityEngine;
using Collision = Assets.Scripts.Player.Collision;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Environment
{
    public class Door : Singleton<Door>
    {
        private Animator anim;

        public bool isUnlocked;

        private void Start()
        {
            isUnlocked = false;

            anim = GetComponent<Animator>();
            NullChecker(anim, "Animator is missing. Please attach it to the object.");
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaught += Lock;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaught -= Lock;
        }

        public void Unlock()
        {
            Debug.Log("DOOR UNLCOKED");
            isUnlocked = true;
            anim.SetTrigger(AnimationParams.Open);
        }

        public void Lock()
        {
            Debug.Log("DOOR LCOKED");
            isUnlocked = false;
            anim.SetTrigger(AnimationParams.Close);
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
