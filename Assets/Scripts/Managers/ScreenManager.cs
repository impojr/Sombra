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
        public float levelTransitionDelay = 0.1f;
        public float levelTransitionTime = 0.2f;

        public void FadeIn(TweenCallback callback)
        {
            var fadeIn = DOTween.Sequence();

            fadeIn.AppendInterval(levelTransitionDelay);
            fadeIn.OnComplete(callback);
        }

        public void FadeOut(TweenCallback callback)
        {
            var fadeOut = DOTween.Sequence().SetUpdate(true);

            fadeOut.AppendInterval(levelTransitionDelay);
            fadeOut.OnComplete(callback);
        }

        public void LoadCreditsScene()
        {
            SceneManager.LoadScene(1);
        }

        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene(0);
        }

        public void LoadLevelSelectScene()
        {
            SceneManager.LoadScene(2);
        }
    }
}
