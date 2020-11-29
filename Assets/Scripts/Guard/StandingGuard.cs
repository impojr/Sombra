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
    public class StandingGuard : GuardBase
    {
        public Transform startPos;
        public Transform endPos;

        protected override void Start()
        {
            base.Start();
            NullChecker(startPos, "Start pos on child missing. Please add to child.");
            NullChecker(endPos, "End pos on child missing. Please add to child.");
        }
    }
}
