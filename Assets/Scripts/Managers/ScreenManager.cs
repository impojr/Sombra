using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Scripts.Helpers.Helpers;

namespace Assets.Scripts.Managers
{
    public abstract class ScreenManager : MonoBehaviour
    {
        private readonly float _minBlockCount = 64.0f;
        private readonly float _maxBlockCount = 512.0f;
        private Pixelation.Scripts.Pixelation _pixelation;

        public float levelTransitionDelay = 0.1f;
        public float levelTransitionTime = 0.2f;

        private void Awake()
        {
            _pixelation = GetComponent<Pixelation.Scripts.Pixelation>();
            NullChecker(_pixelation, "Pixelation script is missing from object. Please attach.");

            _pixelation.enabled = true;
            _pixelation.BlockCount = _minBlockCount;
        }

        void Start()
        {
            FadeIn(ShowScreen);
        }

        public void FadeIn(TweenCallback callback)
        {
            var fadeIn = DOTween.Sequence();

            fadeIn.AppendInterval(levelTransitionDelay);
            fadeIn.Insert(levelTransitionDelay,
                DOTween.To(() => _pixelation.BlockCount, x => _pixelation.BlockCount = x, _maxBlockCount,
                    levelTransitionTime));
            fadeIn.OnComplete(callback);
        }

        public void FadeOut(TweenCallback callback)
        {
            _pixelation.enabled = true;
            var fadeOut = DOTween.Sequence().SetUpdate(true);

            fadeOut.AppendInterval(levelTransitionDelay);
            fadeOut.Insert(0,
                DOTween.To(() => _pixelation.BlockCount, x => _pixelation.BlockCount = x, _minBlockCount,
                    levelTransitionTime));
            fadeOut.OnComplete(callback);
        }

        public void ShowScreen()
        {
            _pixelation.enabled = false;
        }

        public void LoadCreditsScene()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(1);
            });
        }

        public void LoadMainMenuScene()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(0);
            });
        }

        public void LoadLevelSelectScene()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(2);
            });
        }
    }
}
