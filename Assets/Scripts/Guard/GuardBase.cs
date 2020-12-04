using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Guard
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class GuardBase : MonoBehaviour, IHackable
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
        protected Image ReactionImage;

        protected virtual void Start()
        {
            _collider2D = GetComponent<Collider2D>();
            ReactionImage = GetComponentInChildren<Image>();

            NullChecker(_collider2D, "Collider2D is missing. Please add to game object.");
            NullChecker(visor, "Visor is missing. Please add the visor as a child to the object and reference it.");
            NullChecker(ReactionImage, "Image is missing on Guard canvas. Please add to child.");

            ReactionImage.enabled = false;
            playerDetected = false;
            visor.color = Color.black;
            hacked = false;
        }

        protected virtual void OnEnable()
        {
            PlayerInvisibility.OnInvisible += OnPlayerInvisible;
            PlayerInvisibility.OnVisible += OnPlayerVisible;
            PlayerCaught.OnCaught += RestartLevel;
            PlayerCaught.OnCaughtAnimEnded += UnhackOnReset;
            LevelManager.OnLevelStart += Init;
        }

        protected virtual void OnDisable()
        {
            PlayerInvisibility.OnInvisible -= OnPlayerInvisible;
            PlayerInvisibility.OnVisible -= OnPlayerVisible;
            PlayerCaught.OnCaught -= RestartLevel;
            PlayerCaught.OnCaughtAnimEnded -= UnhackOnReset;
            LevelManager.OnLevelStart -= Init;
        }

        protected void Init()
        {
            visor.color = Color.white;
        }

        protected void OnPlayerInvisible()
        {
            if (playerDetected)
                UndetectPlayer();
        }

        protected void OnPlayerVisible()
        {
            if (_collider2D.IsTouching(PlayerMovement.Instance.boxCollider))
                _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }

        protected void RestartLevel()
        {
            StopAllCoroutines();
        }

        protected virtual void UnhackOnReset()
        {
            visor.color = Color.white;
            hacked = false;
            ReactionImage.enabled = false;
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (IsHacked()) return;
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            playerDetected = true;
            _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }

        protected virtual IEnumerator DetectPlayer()
        {
            visor.color = Color.yellow;
            ReactionImage.enabled = true;
            ReactionImage.sprite = playerNoticedSprite;

            yield return new WaitForSeconds(timeBeforeDetected);
            visor.color = Color.red;
            ReactionImage.sprite = playerDetectedSprite;

            PlayerCaught.Instance.Detected();
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            UndetectPlayer();
        }

        protected void UndetectPlayer()
        {
            ReactionImage.enabled = false;
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

        public virtual void Hacked()
        {
            hacked = true;
            visor.color = Color.black;
            UndetectPlayer();
            ReactionImage.enabled = true;
            ReactionImage.sprite = hackedSprite;
            StartCoroutine(Restore());
        }

        protected virtual IEnumerator Restore()
        {
            yield return new WaitForSeconds(timeDisabledWhileHacked);
            visor.color = Color.white;
            hacked = false;
            ReactionImage.enabled = false;

            OnPlayerVisible();
        }
    }
}
