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
        private bool _levelComplete;

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
            AudioManager.Instance.Play(AudioClipName.DoorOpened);
            isUnlocked = true;
            anim.SetBool(AnimationParams.IsOpen, true);
        }

        public void Lock()
        {
            if (!isUnlocked) return;

            AudioManager.Instance.Play(AudioClipName.DoorClosed);
            isUnlocked = false;
            anim.SetBool(AnimationParams.IsOpen, false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            DetectPlayerGoingThroughDoor(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            DetectPlayerGoingThroughDoor(other);
        }

        private void DetectPlayerGoingThroughDoor(Collider2D other)
        {
            if (!_levelComplete && other.CompareTag(Tags.Player) && isUnlocked && Collision.Instance.onGround)
            {
                _levelComplete = true;
                OnDoorEntered?.Invoke();
                other.transform.DOMoveX(transform.position.x, 0.5f).OnComplete(() =>
                {
                    LevelManager.Instance.EndLevel();
                });
            }
        }
    }
}
