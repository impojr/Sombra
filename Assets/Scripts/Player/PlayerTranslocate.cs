using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Environment;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Player
{
    public class PlayerTranslocate : Singleton<PlayerTranslocate>
    {
        public Translocator.Translocator translocator;

        public bool canTranslocate;

        [Space]
        public float delayBetweenThrows = 2f;

        private bool _translocatorDeployed;
        private bool _canThrow;

        private void Start()
        {
            _canThrow = true;
            _translocatorDeployed = false;
            canTranslocate = true;

            NullChecker(translocator, "Translocator is missing. Please add reference.");
        }

        private void Update()
        {
            if (CanThrow() && !_translocatorDeployed && Input.GetButtonDown(InputManager.Translocate))
            {
                translocator.Throw(PlayerMovement.Instance.facingRight);
                _translocatorDeployed = true;
                canTranslocate = false;
            } else if (_translocatorDeployed && Input.GetButtonDown(InputManager.Translocate) && canTranslocate)
            {
                translocator.Translocate();
                TranslocatorUsed();
            } else if (_translocatorDeployed && Input.GetButtonDown(InputManager.CancelTranslocator))
            {
                translocator.Cancel();
                TranslocatorUsed();
            }
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaught += ResetTranslocator;
            PlayerCaught.OnCaughtAnimEnded += EnableTranslocator;
            LevelManager.OnLevelStart += Init;
            Door.OnDoorEntered += DoorEntered;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaught -= ResetTranslocator;
            PlayerCaught.OnCaughtAnimEnded -= EnableTranslocator;
            LevelManager.OnLevelStart -= Init;
            Door.OnDoorEntered -= DoorEntered;
        }

        private void Init()
        {
            _canThrow = true;
        }

        private void DoorEntered()
        {
            _canThrow = false;
            canTranslocate = false;
        }

        private void ResetTranslocator()
        {
            StopAllCoroutines();
            translocator.Cancel();
            _canThrow = false;
            canTranslocate = false;
            _translocatorDeployed = false;
        }

        private void EnableTranslocator()
        {
            _canThrow = true;
            canTranslocate = true;
        }

        private bool CanThrow()
        {
            return _canThrow && !PlayerInvisibility.Instance.isInvisible && !PlayerHack.Instance.isHacking;
        }

        private void TranslocatorUsed()
        {
            canTranslocate = false;
            _translocatorDeployed = false;
            StartCoroutine(AllowThrowing());
        }

        private IEnumerator AllowThrowing()
        {
            _canThrow = false;
            yield return new WaitForSeconds(delayBetweenThrows);
            _canThrow = true;
        }
    }
}
