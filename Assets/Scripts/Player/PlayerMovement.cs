using System;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

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

        public Collider2D boxCollider;
        public bool facingRight;

        [Header("Movement")]
        public float speed = 10;
        public bool canMove;

        [Header("Jump")]
        public float jumpForce = 10;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        void Start()
        {
            _anim = GetComponent<Animator>();

            NullChecker(_anim, "Animator is missing. Please attach it to the object.");
            NullChecker(boxCollider, "Box Collider is missing. Please reference it.");

            _coll = GetComponent<Collision>();
            _rb = GetComponent<Rigidbody2D>();
            facingRight = true;

            _oldPosition = transform.position.x;
        }

        void Update()
        {
            FlipSprite();
            UpdateAnimation();
            UpdateCoordinates();
            ProcessWalk();
            ProcessJump();
            BetterJumping();
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
            if (!canMove) { return; }
            var dir = new Vector2(_xMovement, _yMovement);
            _rb.velocity = new Vector2(dir.x * speed, _rb.velocity.y);
        }

        private void ProcessJump()
        {
            if (!canMove) { return; }
            if (Input.GetButtonDown(InputManager.Jump) && _coll.onGround)
            {
                Jump(Vector2.up);
            }
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
    }
}