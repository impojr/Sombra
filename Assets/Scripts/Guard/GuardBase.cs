using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Guard
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class GuardBase : MonoBehaviour, IHackable
    {
        public SpriteRenderer baseSprite;
        [Tooltip("The visor of the robot. This is needed to change its colour.")]
        public SpriteRenderer visor;

        public float visorLightIntensity = 0.5f;

        [Header("Player Detection")]
        public bool playerDetected;
        public float timeBeforeDetected = 0.2f;

        [Header("Hacking")]
        public float timeDisabledWhileHacked = 5f;
        public bool hacked;
        public Transform hackLineEndPos;

        [Header("Reaction Images")]
        public Sprite playerNoticedSprite;
        public Sprite playerDetectedSprite;
        public Sprite hackedSprite;

        private Collider2D _collider2D;
        private Light2D _pointLight;
        private Coroutine _detectPlayerCoroutine;
        protected Image ReactionImage;

        protected virtual void Start()
        {
            _collider2D = GetComponent<Collider2D>();
            _pointLight = GetComponentInChildren<Light2D>();
            ReactionImage = GetComponentInChildren<Image>();

            NullChecker(_collider2D, "Collider2D is missing. Please add to game object.");
            NullChecker(_pointLight, "Point Light is missing on Guard. Please add to child.");
            NullChecker(visor, "Visor is missing. Please add the visor as a child to the object and reference it.");
            NullChecker(ReactionImage, "Image is missing on Guard canvas. Please add to child.");
            NullChecker(hackLineEndPos, "hackLineEndPos is missing. Please add as child.");

            ReactionImage.enabled = false;
            playerDetected = false;
            UpdateVisor(Color.black, 0);

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
            UpdateVisor(Color.white);
            AudioManager.Instance.Play(AudioClipName.GuardRestored);
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

        protected void UpdateVisor(Color color, float lightIntensity = 0.5f)
        {
            visor.color = color;
            _pointLight.color = color;
            _pointLight.intensity = lightIntensity;
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
            AudioManager.Instance.Play(AudioClipName.Seen);
            UpdateVisor(Color.yellow);
            ReactionImage.enabled = true;
            ReactionImage.sprite = playerNoticedSprite;

            yield return new WaitForSeconds(timeBeforeDetected);
            UpdateVisor(Color.red);
            ReactionImage.sprite = playerDetectedSprite;

            PlayerCaught.Instance.Detected();
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (!other.tag.Equals(Tags.Player)) return;
            if (PlayerInvisibility.Instance.isInvisible) return;

            UndetectPlayer();
        }

        protected virtual void UndetectPlayer()
        {
            ReactionImage.enabled = false;
            playerDetected = false;

            if (!IsHacked())
            {
                UpdateVisor(Color.white);
            }

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
            AudioManager.Instance.Play(AudioClipName.GuardHacked);
            UpdateVisor(Color.black, 0);
            UndetectPlayer();
            ReactionImage.enabled = true;
            ReactionImage.sprite = hackedSprite;
            StartCoroutine(Restore());
        }

        public Transform GetHackPos()
        {
            return hackLineEndPos;
        }

        protected virtual IEnumerator Restore()
        {
            yield return new WaitForSeconds(timeDisabledWhileHacked);
            AudioManager.Instance.Play(AudioClipName.GuardRestored);
            UpdateVisor(Color.white);
            hacked = false;
            ReactionImage.enabled = false;

            OnPlayerVisible();
        }
    }
}
