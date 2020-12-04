using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Environment;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Player
{
    public class PlayerInvisibility : Singleton<PlayerInvisibility>
    {
        public bool isInvisible;
        public bool canBeInvisible;

        [Space]
        public float maxTimeInvisible = 1.5f;
        public float delayBeforeCanBeInvisibleAgain = 3f;

        [Space]
        public SpriteRenderer playerSprite;

        public delegate void Invisible();
        public static event Invisible OnInvisible;

        public delegate void Visible();
        public static event Visible OnVisible;

        private Coroutine _invisibilityCoroutine;

        private void Start()
        {
            NullChecker(playerSprite, "Player sprite is missing. Please reference it.");

            playerSprite.color = Color.white;
            isInvisible = false;
            canBeInvisible = false;
        }

        private void Update()
        {
            if (Input.GetButtonDown(InputManager.Invisibility) && isInvisible)
            {
                TurnVisible();
            } else if (Input.GetButtonDown(InputManager.Invisibility) && canBeInvisible)
            {
                TurnInvisible();
            }
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaught += RestartInvisibility;
            PlayerCaught.OnCaughtAnimEnded += ResetPlayer;
            LevelManager.OnLevelStart += Init;
            Door.OnDoorEntered += EnteredDoor;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaught -= RestartInvisibility;
            PlayerCaught.OnCaughtAnimEnded -= ResetPlayer;
            LevelManager.OnLevelStart -= Init;
            Door.OnDoorEntered -= EnteredDoor;
        }

        private void Init()
        {
            canBeInvisible = true;
        }

        private void EnteredDoor()
        {
            canBeInvisible = false;
            isInvisible = false;
            playerSprite.color = Color.white;
        }

        public void TurnVisible()
        {
            if (!isInvisible)
                return;

            StopCoroutine(_invisibilityCoroutine);

            isInvisible = false;
            playerSprite.color = Color.white;
            OnVisible?.Invoke();
            StartCoroutine(EnableInvisibility());
        }

        private IEnumerator EnableInvisibility()
        {
            canBeInvisible = false;
            yield return new WaitForSeconds(delayBeforeCanBeInvisibleAgain);
            canBeInvisible = true;
        }

        private void TurnInvisible()
        {
            if (isInvisible)
                return;

            isInvisible = true;
            playerSprite.color = Color.black;
            OnInvisible?.Invoke();

            _invisibilityCoroutine = StartCoroutine(TurnVisibleCoroutine());
        }

        private IEnumerator TurnVisibleCoroutine()
        {
            yield return new WaitForSeconds(maxTimeInvisible);
            TurnVisible();
        }

        private void RestartInvisibility()
        {
            StopAllCoroutines();
            canBeInvisible = false;
        }

        private void ResetPlayer()
        {
            canBeInvisible = true;
        }
    }
}
