using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Environment;
using Assets.Scripts.Managers;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Player
{
    public class PlayerTranslocate : MonoBehaviour
    {
        public Translocator.Translocator translocator;

        [Space]
        public float minTimeBetweenThrowAndAct = 0.5f;
        public float delayBetweenThrows = 2f;

        private bool _translocatorDeployed;
        private bool _canTranslocate;
        private bool _canThrow;

        private void Start()
        {
            _canThrow = true;
            _translocatorDeployed = false;
            _canTranslocate = true;

            NullChecker(translocator, "Translocator is missing. Please add reference.");
        }

        private void Update()
        {
            if (CanThrow() && !_translocatorDeployed && Input.GetButtonDown(InputManager.Translocate))
            {
                translocator.Throw(PlayerMovement.Instance.facingRight);
                _translocatorDeployed = true;
                StartCoroutine(AllowTranslocation());
            } else if (_translocatorDeployed && Input.GetButtonDown(InputManager.Translocate) && _canTranslocate)
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
            _canTranslocate = false;
        }

        private void ResetTranslocator()
        {
            StopAllCoroutines();
            translocator.Cancel();
            _canThrow = false;
            _canTranslocate = false;
            _translocatorDeployed = false;
        }

        private void EnableTranslocator()
        {
            _canThrow = true;
            _canTranslocate = true;
        }

        private bool CanThrow()
        {
            return _canThrow && !PlayerInvisibility.Instance.isInvisible && !PlayerHack.Instance.isHacking;
        }

        private IEnumerator AllowTranslocation()
        {
            yield return new WaitForSeconds(minTimeBetweenThrowAndAct);
            _canTranslocate = true;
        }

        private void TranslocatorUsed()
        {
            _canTranslocate = false;
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
