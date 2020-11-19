using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player
{
    public class PlayerHack : Singleton<PlayerHack>
    {
        private LayerMask _layermask;
        private IHackable _objectHacking;
        private float _currentHackingTime;

        public float hackDistance = 1f;
        public float hackTimeInSeconds = 2f;
        public bool isHacking = false;
        public bool canHack = true;
        public float timeBetweenHacks = 1.5f;

        private void Awake()
        {
            _layermask = 1 << LayerMask.NameToLayer(Layer.Hackable);
            _objectHacking = null;
            _currentHackingTime = 0f;
            isHacking = false;
        }

        void Update()
        {
            if (!canHack) return;

            if (Input.GetButton(InputManager.Hack))
            {
                var playerFacingRight = PlayerMovement.Instance.facingRight;

                RaycastHit2D hit = playerFacingRight ?
                    Physics2D.Raycast(transform.position, Vector2.right, hackDistance, _layermask) :
                    Physics2D.Raycast(transform.position, Vector2.left, hackDistance, _layermask);

                if (playerFacingRight)
                {
                    Debug.DrawRay(transform.position, Vector2.right * 1f);
                }
                else
                {
                    Debug.DrawRay(transform.position, Vector2.left * 1f);
                }

                if (!hit) return;

                var hackableObject = hit.collider.gameObject.GetComponentInParent<IHackable>();

                if (hackableObject == null || hackableObject.IsHacked()) return;

                Debug.Log("HACKING ");

                if (hackableObject != _objectHacking)
                {
                    _objectHacking = hackableObject;
                    _currentHackingTime = 0f;
                }

                isHacking = true;
                _currentHackingTime += Time.deltaTime;

                if (Math.Abs(_currentHackingTime - hackTimeInSeconds) < 0.01f)
                {
                    Debug.Log("HACKED ");
                    _objectHacking.Hacked();
                    StartCoroutine(HackStopped());
                }
            }

            if (Input.GetButtonUp(InputManager.Hack))
            {
                if (!isHacking) return;

                StartCoroutine(HackStopped());
            }
        }

        private IEnumerator HackStopped()
        {
            canHack = false;
            isHacking = false;
            _objectHacking = null;
            yield return new WaitForSeconds(timeBetweenHacks);
            canHack = true;
        }
    }
}
