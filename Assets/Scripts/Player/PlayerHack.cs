using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Environment;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
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
        private Animator _anim;

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
            _anim = GetComponent<Animator>();
            _layerMask = 1 << LayerMask.NameToLayer(Layer.Hackable);
            _objectHacking = null;
            _currentHackingTime = 0f;
            isHacking = false;
            canHack = false;

            _line = GetComponentInChildren<LineRenderer>();
            NullChecker(_anim, "Animator is missing. Please attach it to the object.");
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
                _anim.SetBool(AnimationParams.HackingStance, true);
                PlayerMovement.Instance.canMove = false;
                PlayerMovement.Instance.StopMomentum();
                var playerFacingRight = PlayerMovement.Instance.facingRight;

                RaycastHit2D hit = playerFacingRight ?
                    Physics2D.Raycast(transform.position, Vector2.right, hackDistance, _layerMask) :
                    Physics2D.Raycast(transform.position, Vector2.left, hackDistance, _layerMask);

                #region Debug Raydrawing
                if (playerFacingRight)
                {
                    Debug.DrawRay(transform.position, Vector2.right * hackDistance);
                }
                else
                {
                    Debug.DrawRay(transform.position, Vector2.left * hackDistance);
                }
                #endregion

                if (!hit)
                {
                    if (_objectHacking != null)
                    {
                        AudioManager.Instance.Play(AudioClipName.HackInterrupted);
                        StartCoroutine(HackStopped());
                    }

                    return;
                }

                var hackableObject = hit.collider.gameObject.GetComponentInParent<IHackable>();
                if (hackableObject == null || hackableObject.IsHacked()) return;
                if (hackableObject != _objectHacking)
                {
                    AudioManager.Instance.Play(AudioClipName.Hacking);
                    _objectHacking = hackableObject;
                    _currentHackingTime = 0f;
                }

                _anim.SetBool(AnimationParams.Hacking, true);
                isHacking = true;
                _currentHackingTime += Time.deltaTime;

                _line.enabled = true;
                _line.SetPosition(0, hackLineStartPos.position);
                _line.SetPosition(1, hackableObject.GetHackPos().position);
                //Debug.DrawRay(hackLineStartPos.position, hit.collider.transform.position - hackLineStartPos.position);

                if (Math.Abs(_currentHackingTime - hackTimeInSeconds) < 0.01f)
                {
                    LevelManager.Instance.successfulHacks++;
                    _objectHacking.Hacked();
                    StartCoroutine(HackStopped());
                }
            }

            if (Input.GetButtonUp(InputManager.Hack))
            {
                _anim.SetBool(AnimationParams.HackingStance, false);
                PlayerMovement.Instance.canMove = true;
                if (!isHacking) return;
                AudioManager.Instance.Play(AudioClipName.HackInterrupted);
                StartCoroutine(HackStopped());
            }
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaught += ResetHack;
            PlayerCaught.OnCaughtAnimEnded += EnableHack;
            LevelManager.OnLevelStart += Init;
            Door.OnDoorEntered += EnteredDoor;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaught -= ResetHack;
            PlayerCaught.OnCaughtAnimEnded -= EnableHack;
            LevelManager.OnLevelStart -= Init;
            Door.OnDoorEntered -= EnteredDoor;
        }

        private void Init()
        {
            canHack = true;
        }

        private void EnteredDoor()
        {
            canHack = false;
        }

        private void ResetHack()
        {
            StartCoroutine(HackStopped());
            StopAllCoroutines();
        }

        private void EnableHack()
        {
            canHack = true;
        }

        private IEnumerator HackStopped()
        {
            _anim.SetBool(AnimationParams.HackingStance, false);
            _anim.SetBool(AnimationParams.Hacking, false);
            AudioManager.Instance.Stop(AudioClipName.Hacking);
            PlayerMovement.Instance.canMove = true;
            _line.enabled = false;
            canHack = false;
            isHacking = false;
            _objectHacking = null;

            yield return new WaitForSeconds(timeBetweenHacks);
            canHack = true;
        }
    }
}
