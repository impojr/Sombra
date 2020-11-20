using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Player;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Guard
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StandingGuard : MonoBehaviour, IHackable
    {
        public bool playerDetected;
        public bool hacked;
        public SpriteRenderer visor;

        public float timeDisabledWhileHacked = 5f;
        public float timeBeforeDetected = 0.2f;

        private Collider2D collider2D;

        private Coroutine _detectPlayerCoroutine;

        private void Start()
        {
            if (visor == null)
                throw new NullReferenceException("Visor is missing. Please add the visor as a child to the object and reference it.");

            playerDetected = false;
            visor.color = Color.white;
            hacked = false;
            collider2D = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            PlayerInvisibility.OnInvisible += OnPlayerInvisible;
            PlayerInvisibility.OnVisible += OnPlayerVisible;
        }

        private void OnDisable()
        {
            PlayerInvisibility.OnInvisible -= OnPlayerInvisible;
            PlayerInvisibility.OnVisible -= OnPlayerVisible;
        }

        private void OnTriggerEnter2D([NotNull] Collider2D other)
        {
            if (hacked) return;
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            playerDetected = true;
            _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }

        private void OnTriggerExit2D([NotNull] Collider2D other)
        {
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            UndetectPlayer();
        }

        private IEnumerator DetectPlayer()
        {
            visor.color = Color.yellow;
            yield return new WaitForSeconds(timeBeforeDetected);
            visor.color = Color.red;
        }

        private void UndetectPlayer()
        {
            playerDetected = false;

            if (!hacked)
                visor.color = Color.white;

            if (_detectPlayerCoroutine != null)
                StopCoroutine(_detectPlayerCoroutine);
        }

        public bool IsHacked()
        {
            return hacked;
        }

        public void Hacked()
        {
            hacked = true;
            visor.color = Color.black;
            UndetectPlayer();
            StartCoroutine(Restore());
        }

        private IEnumerator Restore()
        {
            yield return new WaitForSeconds(timeDisabledWhileHacked);
            visor.color = Color.white;
            hacked = false;

            OnPlayerVisible();
        }

        private void OnPlayerInvisible()
        {
            if (!playerDetected) return;

            UndetectPlayer();
        }

        private void OnPlayerVisible()
        {
            if (collider2D.IsTouching(PlayerMovement.Instance.boxCollider))
            {
                _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
            }
        }
    }
}
