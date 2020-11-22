using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Player
{
    public class PlayerHack : Singleton<PlayerHack>
    {
        private LayerMask _layerMask;
        private IHackable _objectHacking;
        private float _currentHackingTime;
        private LineRenderer _line;

        public bool canHack = true;
        public bool isHacking = false;

        [Space]
        public float hackDistance = 1f;

        [Space]
        [Tooltip("Where the hack line starts on the Player.")]
        public Transform hackLineStartPos;

        [Header("Delays")]
        public float hackTimeInSeconds = 2f;
        public float timeBetweenHacks = 1.5f;

        private void Awake()
        {
            _layerMask = 1 << LayerMask.NameToLayer(Layer.Hackable);
            _objectHacking = null;
            _currentHackingTime = 0f;
            isHacking = false;

            _line = GetComponentInChildren<LineRenderer>();
            NullChecker(_line, "Line renderer is missing. Please attach it to child.");
            NullChecker(hackLineStartPos, "hackLineStartPos is missing. Please add reference.");
        }

        void Update()
        {
            if (!canHack) return;

            //stop hacking when the player turns invisible
            if (isHacking && PlayerInvisibility.Instance.isInvisible)
            {
                StartCoroutine(HackStopped());
                return;
            }

            if (Input.GetButton(InputManager.Hack) && !PlayerInvisibility.Instance.isInvisible)
            {
                var playerFacingRight = PlayerMovement.Instance.facingRight;

                RaycastHit2D hit = playerFacingRight ?
                    Physics2D.Raycast(transform.position, Vector2.right, hackDistance, _layerMask) :
                    Physics2D.Raycast(transform.position, Vector2.left, hackDistance, _layerMask);

                #region Debug Raydrawing
                //if (playerFacingRight)
                //{
                //    Debug.DrawRay(transform.position, Vector2.right * 1f);
                //}
                //else
                //{
                //    Debug.DrawRay(transform.position, Vector2.left * 1f);
                //}
                #endregion

                if (!hit)
                {
                    if (_objectHacking != null) 
                        StartCoroutine(HackStopped());

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

                _line.enabled = true;
                _line.SetPosition(0, hackLineStartPos.position);
                _line.SetPosition(1, hit.collider.transform.position);

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
            _line.enabled = false;
            canHack = false;
            isHacking = false;
            _objectHacking = null;

            yield return new WaitForSeconds(timeBetweenHacks);
            canHack = true;
        }
    }
}
