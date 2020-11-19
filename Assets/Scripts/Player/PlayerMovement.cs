using System;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : Singleton<PlayerMovement>
    {
        private Collision coll;
        private Rigidbody2D rb;
        private float xMovement;
        private float yMovement;
        public bool facingRight;
        private float oldPosition;

        public Collider2D boxCollider;

        [Space]
        public float speed = 10;

        [Header("Jump")]
        public float jumpForce = 10;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        [Space]
        public bool canMove;

        void Start()
        {
            if (boxCollider == null)
                throw new NullReferenceException("Box Collider is missing. Please reference it.");

            coll = GetComponent<Collision>();
            rb = GetComponent<Rigidbody2D>();
            facingRight = true;

            oldPosition = transform.position.x;
        }

        void Update()
        {
            FlipSprite();
            UpdateCoordinates();
            ProcessWalk();
            ProcessJump();
            BetterJumping();
        }

        private void FlipSprite()
        {
            if (Math.Abs(transform.position.x - oldPosition) < 0.01f) return;

            if (transform.position.x > oldPosition) // he's looking right
            {
                facingRight = true;
                transform.localScale = Vector3.one;
            }

            if (transform.position.x < oldPosition) // he's looking left
            {
                facingRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
            }

            oldPosition = transform.position.x;
        }

        private void UpdateCoordinates()
        {
            xMovement = Input.GetAxis(InputManager.Horizontal);
            yMovement = Input.GetAxis(InputManager.Vertical);
        }

        private void ProcessWalk()
        {
            if (!canMove) { return; }
            Vector2 dir = new Vector2(xMovement, yMovement);
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }

        private void ProcessJump()
        {
            if (!canMove) { return; }
            if (Input.GetButtonDown(InputManager.Jump) && coll.onGround)
            {
                Jump(Vector2.up);
            }
        }

        private void Jump(Vector2 dir)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += dir * jumpForce;
        }

        private void BetterJumping()
        {
            if (!canMove) { return; };

            //changes the gravity of the player depending on whether or not they are holding the jump buttom
            //this allows for changes in the jump height depending on button press
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton(InputManager.Jump))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }
}