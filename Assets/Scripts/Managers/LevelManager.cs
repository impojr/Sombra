using System;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts.Guard;
using Assets.Scripts.Helpers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        private Pixelation.Scripts.Pixelation pixelation;
        private readonly float _minBlockCount = 64.0f;
        private readonly float _maxBlockCount = 512.0f;

        public TextMeshProUGUI levelText;
        public Image fadeImage;
        public float levelTransitionDelay = 0.5f;
        public float levelTransitionTime = 1f;

        public delegate void LevelStart();
        public static event LevelStart OnLevelStart;

        public delegate void LevelEnd();
        public static event LevelEnd OnLevelEnd;

        private void Awake()
        {
            pixelation = GetComponent<Pixelation.Scripts.Pixelation>();
            NullChecker(pixelation, "Pixelation script is missing from object. Please attach.");
            NullChecker(levelText, "Please attach levelText to script");
            NullChecker(fadeImage, "Please attach fadeImage to script");

            pixelation.enabled = true;
            pixelation.BlockCount = _minBlockCount;
            fadeImage.color = Color.black;
        }

        private void Start()
        {
            OrderGuardSpritesAndVisors();

            var fadeIn = DOTween.Sequence();

            fadeIn.AppendInterval(levelTransitionDelay);
            fadeIn.Append(fadeImage.DOFade(0, levelTransitionTime));
            fadeIn.Insert(levelTransitionDelay, levelText.DOFade(0, levelTransitionTime));
            fadeIn.Insert(levelTransitionDelay,
                DOTween.To(() => pixelation.BlockCount, x => pixelation.BlockCount = x, _maxBlockCount,
                    levelTransitionTime));
            fadeIn.OnComplete(StartLevel);
        }

        private void OrderGuardSpritesAndVisors()
        {
            var guards = FindObjectsOfType<GuardBase>();
            var orderInLayercount = 0;

            foreach (var guard in guards)
            {
                //sprite greater than visor
                guard.visor.sortingOrder = orderInLayercount++;
                guard.baseSprite.sortingOrder = orderInLayercount++;
            }
        }

        private void StartLevel()
        {
            levelText.enabled = false;
            pixelation.enabled = false;
            OnLevelStart?.Invoke();
        }

        public void EndLevel()
        {
            OnLevelEnd?.Invoke();
            SceneChangeTransition();
            //show end level modal
        }

        private void SceneChangeTransition()
        {
            pixelation.enabled = true;
            var fadeOut = DOTween.Sequence();

            fadeOut.AppendInterval(levelTransitionDelay);
            fadeOut.Append(fadeImage.DOFade(1, levelTransitionTime));
            fadeOut.Insert(0,
                DOTween.To(() => pixelation.BlockCount, x => pixelation.BlockCount = x, _minBlockCount,
                    levelTransitionTime));
            //fadeOut.OnComplete();
        }
    }
}
