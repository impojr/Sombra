using System;
using Assets.Scripts.Player;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Translocator
{
    public class Translocator : MonoBehaviour
    {
        [Tooltip("The direction in when the object travels when thrown right.")]
        public Vector2 trajectory;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponentInChildren<Rigidbody2D>();
            NullChecker(_rb, "Rigidbody2D is missing. Please attach it to child.");
            
            gameObject.SetActive(false);
        }

        public void Throw(bool facingRight)
        {
            gameObject.SetActive(true);
            transform.position = PlayerMovement.Instance.gameObject.transform.position;

            _rb.AddForce(facingRight ? trajectory : new Vector2(-trajectory.x, trajectory.y));
        }

        public void Translocate()
        {
            var playerHeightHalf = PlayerMovement.Instance.boxCollider.size.y / 2f;
            var targetPos = new Vector2(transform.position.x, transform.position.y + playerHeightHalf);
            PlayerMovement.Instance.gameObject.transform.position = targetPos;
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
        }
    }
}
