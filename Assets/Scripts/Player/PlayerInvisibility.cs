using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerInvisibility : Singleton<PlayerInvisibility>
    {
        public bool isInvisible;
        public bool canBeInvisible;

        public float maxTimeInvisible = 1.5f;
        public float delayBeforeCanBeInvisibleAgain = 3f;

        public SpriteRenderer playerSprite;

        public delegate void Invisible();
        public static event Invisible OnInvisible;

        public delegate void Visible();
        public static event Visible OnVisible;

        private Coroutine _invisibilityCoroutine;

        private void Start()
        {
            if (playerSprite == null)
                throw new NullReferenceException("Player sprite is missing. Please reference it.");

            playerSprite.color = Color.white;
            isInvisible = false;
            canBeInvisible = true;
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

        private void TurnVisible()
        {
            if (!isInvisible)
                return;

            StopCoroutine(_invisibilityCoroutine);

            isInvisible = false;
            playerSprite.color = Color.white;
            OnVisible?.Invoke();
            StartCoroutine(EnableInvisibility());
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

        private IEnumerator EnableInvisibility()
        {
            canBeInvisible = false;
            yield return new WaitForSeconds(delayBeforeCanBeInvisibleAgain);
            canBeInvisible = true;
        }
    }
}
