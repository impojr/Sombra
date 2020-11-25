using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Player;
using JetBrains.Annotations;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts.Guard
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StandingGuard : MonoBehaviour, IHackable
    {
        [Tooltip("The visor of the robot. This is needed to change its colour.")]
        public SpriteRenderer visor;

        [Header("Player Detection")]
        public bool playerDetected;
        public float timeBeforeDetected = 0.2f;

        [Header("Hacking")]
        public float timeDisabledWhileHacked = 5f;
        public bool hacked;

        [Header("Reaction Images")] 
        public Sprite playerNoticedSprite;
        public Sprite playerDetectedSprite;
        public Sprite hackedSprite;

        private Collider2D _collider2D;
        private Coroutine _detectPlayerCoroutine;
        private Image _reactionImage;

        private void Start()
        {
            _collider2D = GetComponent<Collider2D>();
            _reactionImage = GetComponentInChildren<Image>();

            NullChecker(_collider2D, "Collider2D is missing. Please add to game object.");
            NullChecker(visor, "Visor is missing. Please add the visor as a child to the object and reference it.");
            NullChecker(_reactionImage, "Image is missing on Guard canvas. Please add to child.");

            _reactionImage.enabled = false;
            playerDetected = false;
            visor.color = Color.white;
            hacked = false;
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

        private void OnPlayerInvisible()
        {
            if (playerDetected) 
                UndetectPlayer();
        }

        private void OnPlayerVisible()
        {
            if (_collider2D.IsTouching(PlayerMovement.Instance.boxCollider))
                _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }

        private void OnTriggerEnter2D([NotNull] Collider2D other)
        {
            if (IsHacked()) return;
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            playerDetected = true;
            _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }

        private IEnumerator DetectPlayer()
        {
            visor.color = Color.yellow;
            _reactionImage.enabled = true;
            _reactionImage.sprite = playerNoticedSprite;

            yield return new WaitForSeconds(timeBeforeDetected);
            visor.color = Color.red;
            _reactionImage.sprite = playerDetectedSprite;
        }

        private void OnTriggerExit2D([NotNull] Collider2D other)
        {
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            UndetectPlayer();
        }

        private void UndetectPlayer()
        {
            _reactionImage.enabled = false;
            playerDetected = false;

            if (!IsHacked())
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
            _reactionImage.enabled = true;
            _reactionImage.sprite = hackedSprite;
            StartCoroutine(Restore());
        }

        private IEnumerator Restore()
        {
            yield return new WaitForSeconds(timeDisabledWhileHacked);
            visor.color = Color.white;
            hacked = false;
            _reactionImage.enabled = false;

            OnPlayerVisible();
        }
    }
}
