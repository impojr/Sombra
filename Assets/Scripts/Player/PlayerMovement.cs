using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Environment;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;
using DG.Tweening;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : Singleton<PlayerMovement>
    {
        private Collision _coll;
        private Rigidbody2D _rb;
        private float _xMovement;
        private float _yMovement;
        private float _oldPosition;
        private Animator _anim;
        private Vector2 _initialPos;
        private SpriteRenderer _sprite;

        public Collider2D boxCollider;
        public bool facingRight;

        [Header("Movement")]
        public float speed = 10;
        [Tooltip("How much faster the player walks when invisible")]
        public float invisSpeedMultiplier = 1.25f;
        public bool canMove;

        [Header("Jump")]
        public float jumpForce = 10;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        void Start()
        {
            _anim = GetComponent<Animator>();
            _sprite = GetComponentInChildren<SpriteRenderer>();

            NullChecker(_anim, "Animator is missing. Please attach it to the object.");
            NullChecker(_sprite, "Sprite Renderer is missing. Please attach it to the child.");
            NullChecker(boxCollider, "Box Collider is missing. Please reference it.");

            _coll = GetComponent<Collision>();
            _rb = GetComponent<Rigidbody2D>();
            facingRight = true;

            _initialPos = transform.position;
            _oldPosition = transform.position.x;

            canMove = false;
        }

        void Update()
        {
            FlipSprite();
            UpdateAnimation();

            if (!canMove) return;
            UpdateCoordinates();
            ProcessWalk();
            ProcessJump();
            BetterJumping();
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaught += RestartLevel;
            PlayerCaught.OnCaughtAnimEnded += ResetPlayer;
            LevelManager.OnLevelStart += Init;
            LevelManager.OnLevelEnd += Terminate;
            Door.OnDoorEntered += DoorEntered;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaught -= RestartLevel;
            PlayerCaught.OnCaughtAnimEnded -= ResetPlayer;
            LevelManager.OnLevelStart -= Init;
            LevelManager.OnLevelEnd -= Terminate;
            Door.OnDoorEntered -= DoorEntered;
        }

        private void Init()
        {
            canMove = true;
        }

        private void DoorEntered()
        {
            canMove = false;
            StopMomentum();
        }

        private void Terminate()
        {
            StopMomentum();
            EnterDoor();
        }

        private void FlipSprite()
        {
            if (Math.Abs(transform.position.x - _oldPosition) < 0.01f) return;

            if (transform.position.x > _oldPosition) // he's looking right
            {
                facingRight = true;
                transform.localScale = Vector3.one;
            }

            if (transform.position.x < _oldPosition) // he's looking left
            {
                facingRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
            }

            _oldPosition = transform.position.x;
        }

        private void UpdateAnimation()
        {
            _anim.SetBool(AnimationParams.HasXVelocity, Mathf.Abs(_xMovement) > 0f);
        }

        private void UpdateCoordinates()
        {
            _xMovement = Input.GetAxis(InputManager.Horizontal);
            _yMovement = Input.GetAxis(InputManager.Vertical);
        }

        private void ProcessWalk()
        {
            var movementSpeed = PlayerInvisibility.Instance.isInvisible ? speed * invisSpeedMultiplier : speed;

            if (!canMove) { return; }
            var dir = new Vector2(_xMovement, _yMovement);
            _rb.velocity = new Vector2(dir.x * movementSpeed, _rb.velocity.y);
        }

        private void ProcessJump()
        {
            if (!canMove) { return; }
            if (Input.GetButtonDown(InputManager.Jump) && _coll.onGround)
            {
                Jump(Vector2.up);
            }
        }

        public void RestartLevel()
        {
            _rb.constraints = RigidbodyConstraints2D.FreezePosition;
            _xMovement = _yMovement = 0;
            canMove = false;
            _anim.SetTrigger(AnimationParams.Caught);
        }

        private void ResetPlayer()
        {
            transform.position = _initialPos;
            _oldPosition = transform.position.x;
            facingRight = true;
            transform.localScale = Vector3.one;
            _rb.constraints = RigidbodyConstraints2D.None;
            canMove = true;
            _anim.SetTrigger(AnimationParams.Reset);
        }

        private void Jump(Vector2 dir)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.velocity += dir * jumpForce;
        }

        private void BetterJumping()
        {
            if (!canMove) { return; };

            //changes the gravity of the player depending on whether or not they are holding the jump buttom
            //this allows for changes in the jump height depending on button press
            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (_rb.velocity.y > 0 && !Input.GetButton(InputManager.Jump))
            {
                _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        public void StopMomentum()
        {
            _xMovement = _yMovement = 0;
            _rb.velocity = Vector2.zero;
        }

        public void EnterDoor()
        {
            _anim.SetTrigger(AnimationParams.Exit);
            _sprite.DOFade(0, 1f).OnComplete(() =>
            {
                //todo trigger next level stuff
                Debug.Log("AYY");
            });
        }
    }
}