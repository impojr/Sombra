using System;
using System.Collections;
using Assets.Scripts.Constants;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerInvisibility : MonoBehaviour
    {
        public bool isInvisible;
        public bool canBeInvisible;

        public float maxTimeInvisible = 5f;
        public float delayBeforeCanBeInvisibleAgain = 2.5f;

        public SpriteRenderer playerSprite;

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

        private void TurnInvisible()
        {
            if (isInvisible)
                return;

            isInvisible = true;
            playerSprite.color = Color.black;
            Debug.Log("Invisible");
            _invisibilityCoroutine = StartCoroutine(TurnVisibleCoroutine());
        }

        private IEnumerator TurnVisibleCoroutine()
        {
            yield return new WaitForSeconds(maxTimeInvisible);
            Debug.Log("Visible Co-routine");
            TurnVisible();
        }

        private void TurnVisible()
        {
            if (!isInvisible)
                return;

            StopCoroutine(_invisibilityCoroutine);

            isInvisible = false;
            playerSprite.color = Color.white;
            Debug.Log("Visible");
            StartCoroutine(EnableInvisibility());
        }

        private IEnumerator EnableInvisibility()
        {
            canBeInvisible = false;
            yield return new WaitForSeconds(delayBeforeCanBeInvisibleAgain);
            canBeInvisible = true;
        }
    }
}
