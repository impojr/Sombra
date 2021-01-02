using System;
using System.Collections;
using Assets.Scripts.Constants;
using Assets.Scripts.Environment;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Guard
{
    public class MovingGuard : GuardBase
    {
        [Header("Moving Parameters")]
        public Transform startPos;
        public Transform endPos;

        public float timeToMoveBetweenPos = 6f;
        public float timeStoppedAtEachEnd = 3f;

        public Sequence PatrolRoute;

        private float _directionScale;

        protected override void Start()
        {
            base.Start();
            NullChecker(startPos, "Start pos on child missing. Please add to child.");
            NullChecker(endPos, "End pos on child missing. Please add to child.");

            _directionScale = startPos.position.x >= endPos.position.x ? 1f : -1f;
            SetStartPosition();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LevelManager.OnLevelStart += Move;
            PlayerCaught.OnCaughtAnimEnded += ResetGuard;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LevelManager.OnLevelStart -= Move;
            PlayerCaught.OnCaughtAnimEnded -= ResetGuard;
        }

        protected override void RestartLevel()
        {
            base.RestartLevel();
            PatrolRoute.Pause();
        }

        private void SetStartPosition()
        {
            transform.position = startPos.position;
            transform.localScale = new Vector3(_directionScale, 1, 1);
        }

        private void ResetGuard()
        {
            SetStartPosition();
            PatrolRoute.Restart();
        }

        private void Move()
        {
            PatrolRoute = DOTween.Sequence();

            PatrolRoute.Append(transform.DOScaleX(_directionScale * 1, 0));
            PatrolRoute.Append(transform.DOMoveX(endPos.position.x, timeToMoveBetweenPos));
            PatrolRoute.AppendInterval(timeStoppedAtEachEnd);
            PatrolRoute.Append(transform.DOScaleX(_directionScale * -1, 0));
            PatrolRoute.Append(transform.DOMoveX(startPos.position.x, timeToMoveBetweenPos));
            PatrolRoute.AppendInterval(timeStoppedAtEachEnd);

            PatrolRoute.SetLoops(-1);
        }

        protected override IEnumerator DetectPlayer()
        {
            PatrolRoute.Pause();
            return base.DetectPlayer();
        }

        public override void Hacked()
        {
            PatrolRoute.Pause();
            base.Hacked();
        }

        protected override IEnumerator Restore()
        {
            yield return new WaitForSeconds(timeDisabledWhileHacked);
            AudioManager.Instance.Play(AudioClipName.GuardRestored);
            PatrolRoute.Play();
            UpdateVisor(Color.white);
            hacked = false;
            ReactionImage.enabled = false;

            OnPlayerVisible();
        }

        protected override void UndetectPlayer()
        {
            base.UndetectPlayer();
            PatrolRoute.Play();
        }

        protected override void UnhackOnReset()
        {
            PatrolRoute.Play();
            visor.color = Color.white;
            hacked = false;
            ReactionImage.enabled = false;
        }
    }
}
