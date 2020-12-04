using System;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using DG.Tweening;
using UnityEngine;
using Collision = Assets.Scripts.Player.Collision;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Environment
{
    public class Door : Singleton<Door>
    {
        private Animator anim;

        public bool isUnlocked;

        public delegate void DoorEntered();
        public static event DoorEntered OnDoorEntered;

        private void Start()
        {
            isUnlocked = false;

            anim = GetComponent<Animator>();
            NullChecker(anim, "Animator is missing. Please attach it to the object.");
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaughtAnimEnded += Lock;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaughtAnimEnded -= Lock;
        }

        public void Unlock()
        {
            isUnlocked = true;
            anim.SetTrigger(AnimationParams.Open);
        }

        public void Lock()
        {
            isUnlocked = false;
            anim.SetTrigger(AnimationParams.Close);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player) && isUnlocked && Collision.Instance.onGround)
            {
                OnDoorEntered?.Invoke();
                other.transform.DOMoveX(transform.position.x, 0.5f).OnComplete(() =>
                {
                    LevelManager.Instance.EndLevel();
                });
            }
        }
    }
}
