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
        private LineRenderer line;

        public float hackDistance = 1f;
        public float hackTimeInSeconds = 2f;
        public bool isHacking = false;
        public bool canHack = true;
        public float timeBetweenHacks = 1.5f;
        public Transform hackLineStartPos;

        private void Awake()
        {
            _layermask = 1 << LayerMask.NameToLayer(Layer.Hackable);
            _objectHacking = null;
            _currentHackingTime = 0f;
            isHacking = false;

            line = GetComponentInChildren<LineRenderer>();
            if (line == null)
                throw new NullReferenceException("Line renderer is missing. Please attach it to child.");

            if (hackLineStartPos == null)
                throw new NullReferenceException("hackLineStartPos is missing. Please add reference.");
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

                if (!hit)
                {
                    if (_objectHacking != null)
                    {
                        StartCoroutine(HackStopped());
                    }

                    return;
                }

                var hackableObject = hit.collider.gameObject.GetComponentInParent<IHackable>();

                if (hackableObject == null || hackableObject.IsHacked()) return;

                if (hackableObject != _objectHacking)
                {
                    _objectHacking = hackableObject;
                    _currentHackingTime = 0f;
                }

                isHacking = true;
                _currentHackingTime += Time.deltaTime;

                line.enabled = true;
                line.SetPosition(0, hackLineStartPos.position);
                line.SetPosition(1, hit.collider.transform.position);

                if (Math.Abs(_currentHackingTime - hackTimeInSeconds) < 0.01f)
                {
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
            line.enabled = false;
            canHack = false;
            isHacking = false;
            _objectHacking = null;
            yield return new WaitForSeconds(timeBetweenHacks);
            canHack = true;
        }
    }
}
