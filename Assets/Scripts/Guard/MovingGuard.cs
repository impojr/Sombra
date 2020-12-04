﻿using System;
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

        protected override void Start()
        {
            base.Start();
            NullChecker(startPos, "Start pos on child missing. Please add to child.");
            NullChecker(endPos, "End pos on child missing. Please add to child.");

            if (endPos.position.x <= startPos.position.x)
                Debug.LogWarning("StartPos x value needs to be left of the EndPos x value");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LevelManager.OnLevelStart += Move;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LevelManager.OnLevelStart -= Move;
        }

        private void Move()
        {
            PatrolRoute = DOTween.Sequence();

            PatrolRoute.AppendInterval(timeStoppedAtEachEnd);
            PatrolRoute.Append(transform.DOScaleX(-1, 0));
            PatrolRoute.Append(transform.DOMoveX(endPos.position.x, timeToMoveBetweenPos));
            PatrolRoute.AppendInterval(timeStoppedAtEachEnd);
            PatrolRoute.Append(transform.DOScaleX(1, 0));
            PatrolRoute.Append(transform.DOMoveX(startPos.position.x, timeToMoveBetweenPos));
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
            PatrolRoute.Play();
            visor.color = Color.white;
            hacked = false;
            ReactionImage.enabled = false;

            OnPlayerVisible();
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
