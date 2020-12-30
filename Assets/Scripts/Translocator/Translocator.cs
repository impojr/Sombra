using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Translocator
{
    public class Translocator : MonoBehaviour
    {
        [Tooltip("The direction in when the object travels when thrown right.")]
        public Vector2 trajectory;
        public SpriteRenderer translocatorColor;
        private Rigidbody2D _rb;
        private bool _stationery;

        private void Awake()
        {
            _rb = GetComponentInChildren<Rigidbody2D>();
            NullChecker(_rb, "Rigidbody2D is missing. Please attach it to child.");
            NullChecker(translocatorColor, "translocatorColor is missing. Please attach it to child.");

            _stationery = true;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            StartCoroutine(ResetPreviousPositionLog());
        }

        private IEnumerator ResetPreviousPositionLog()
        {
            yield return new WaitForSeconds(0.1f);
            _stationery = false;
        }

        private void Update()
        {
            if (!_stationery && _rb.velocity == Vector2.zero)
            {
                AudioManager.Instance.Play(AudioClipName.TranslocatorActive);
                translocatorColor.color = Color.green;
                //todo add lighting
                PlayerTranslocate.Instance.canTranslocate = true;
                _stationery = true;
            }
        }

        public void Throw(bool facingRight)
        {
            gameObject.SetActive(true);
            translocatorColor.color = Color.red;
            transform.position = PlayerMovement.Instance.gameObject.transform.position;
            AudioManager.Instance.Play(AudioClipName.TranslocatorThrown);

            _rb.AddForce(facingRight ? trajectory : new Vector2(-trajectory.x, trajectory.y));
        }

        public void Translocate()
        {
            var playerHeightHalf = PlayerMovement.Instance.boxCollider.size.y / 2f;
            var targetPos = new Vector2(transform.position.x, transform.position.y + playerHeightHalf);
            AudioManager.Instance.Play(AudioClipName.TranslocatorUsed);
            PlayerMovement.Instance.gameObject.transform.position = targetPos;
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            AudioManager.Instance.Play(AudioClipName.TranslocatorCancelled);
            gameObject.SetActive(false);
        }
    }
}
