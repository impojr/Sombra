﻿using System;
using Assets.Scripts.Constants;
using Assets.Scripts.Guard;
using Assets.Scripts.Helpers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Helpers.Helpers;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        private bool _paused = false;
        private string _level;

        public TextMeshProUGUI levelText;
        public Image fadeImage;
        public float levelTransitionDelay = 0.5f;
        public float levelTransitionTime = 1f;
        
        public RectTransform pauseMenu;
        public RectTransform pauseMenuPanel;
        public RectTransform pauseMenuBackground;

        public RectTransform endLevelMenu;
        public RectTransform endLevelMenuPanel;
        public RectTransform endLevelMenuBackground;
        public Button nextLevelButton;
        public TextMeshProUGUI levelCompleteTitle;
        public TextMeshProUGUI timesCaughtText;
        public TextMeshProUGUI successfulHacksText;

        public int timesCaught;
        public int successfulHacks;

        public delegate void LevelStart();
        public static event LevelStart OnLevelStart;

        public delegate void LevelEnd();
        public static event LevelEnd OnLevelEnd;

        private void Awake()
        {
            Time.timeScale = 1;
            NullChecker(levelText, "Please attach levelText to script");
            NullChecker(fadeImage, "Please attach fadeImage to script");
            NullChecker(pauseMenu, "Please attach pauseMenu to script");
            NullChecker(endLevelMenu, "Please attach pauseMenu to script");
            NullChecker(timesCaughtText, "Please attach timesCaughtText to script");
            NullChecker(successfulHacksText, "Please attach successfulHacksText to script");
            NullChecker(levelCompleteTitle, "Please attach levelCompleteTitle to script");

            _level = SceneManager.GetActiveScene().name;
            levelText.text = $"Level {_level}";
            endLevelMenu.transform.localScale = Vector3.zero;
            pauseMenu.transform.localScale = Vector3.zero;

            fadeImage.color = Color.black;
        }

        private void Start()
        {
            OrderGuardSpritesAndVisors();
            timesCaught = 0;
            successfulHacks = 0;

        var fadeIn = DOTween.Sequence();

            fadeIn.AppendInterval(levelTransitionDelay);
            fadeIn.Append(fadeImage.DOFade(0, levelTransitionTime));
            fadeIn.Insert(levelTransitionDelay, levelText.DOFade(0, levelTransitionTime));
            fadeIn.OnComplete(StartLevel);
        }

        private void Update()
        {
            if (Input.GetButtonDown(Constants.InputManager.Pause) && !_paused)
            {
                Pause();
            }
            else if (Input.GetButtonDown(Constants.InputManager.Pause) && _paused)
            {
                Resume();
            }
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
            OnLevelStart?.Invoke();
        }

        public void EndLevel()
        {
            AudioManager.Instance.Play(AudioClipName.Win);
            OnLevelEnd?.Invoke();
        }

        public void ShowEndLevelMenu()
        {
            timesCaughtText.text = timesCaught.ToString();
            successfulHacksText.text = successfulHacks.ToString();
            levelCompleteTitle.text = $"Level {_level} Complete";

            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
                Destroy(nextLevelButton.gameObject);

            endLevelMenu.localScale = Vector3.one;
            endLevelMenuBackground.localScale = Vector3.one;
            endLevelMenuPanel.localScale = Vector3.zero;
            endLevelMenuPanel.DOScale(1, Constants.Menu.TransitionDuration);
        }

        private void SceneChangeTransition(TweenCallback callback)
        {
            var fadeOut = DOTween.Sequence().SetUpdate(true);

            fadeOut.AppendInterval(levelTransitionDelay);
            fadeOut.AppendCallback(() =>
            {
                pauseMenuPanel.gameObject.SetActive(false);
                endLevelMenuPanel.gameObject.SetActive(false);
            });
            fadeOut.Append(fadeImage.DOFade(1, levelTransitionTime));
            fadeOut.OnComplete(callback);
        }

        public void Pause()
        {
            _paused = true;
            Time.timeScale = 0;
            pauseMenu.localScale = Vector3.one;
            pauseMenuBackground.localScale = Vector3.one;
            pauseMenuPanel.localScale = Vector3.zero;
            pauseMenuPanel.DOScale(1, Constants.Menu.TransitionDuration).SetUpdate(true);
        }

        public void Resume()
        {
            pauseMenuBackground.localScale = Vector3.zero;
            pauseMenu.DOScale(0, Constants.Menu.TransitionDuration).SetUpdate(true).OnComplete(() =>
            {
                _paused = false;
                Time.timeScale = 1;
            });
        }

        public void NextLevel()
        {
            SceneChangeTransition(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            });
        }

        public void ToMainMenu()
        {
            SceneChangeTransition(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}
