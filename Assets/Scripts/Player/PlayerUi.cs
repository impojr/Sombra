using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Helpers.Helpers;
using DG.Tweening;

namespace Assets.Scripts.Player
{
    public class PlayerUi : Singleton<PlayerUi>
    {
        private Canvas _canvas;
        public RectTransform invisibilityPanel;
        public Image invisibilitySlider; 
        public RectTransform hackPanel;
        public Image hackSlider;
        public RectTransform translocatorPanel;
        public Image translocatorSlider;

        // Start is called before the first frame update
        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
            NullChecker(_canvas, "Please attach canvas to player");
        }

        private void OnEnable()
        {
            PlayerCaught.OnCaught += Reset;
            LevelManager.OnLevelEnd += Reset;
        }

        private void OnDisable()
        {
            PlayerCaught.OnCaught -= Reset;
            LevelManager.OnLevelEnd -= Reset;
        }

        private void Reset()
        {
            invisibilitySlider.fillAmount = 1;
            hackSlider.fillAmount = 1;
            translocatorSlider.fillAmount = 1;
            hackPanel.gameObject.SetActive(false);
            invisibilityPanel.gameObject.SetActive(false);
            translocatorPanel.gameObject.SetActive(false);
        }

        public void Flip(bool facingRight)
        {
            _canvas.transform.localScale = facingRight ? Vector3.one : new Vector3(-1, 1, 1);
        }

        public void StartInvisibility(float maxTimeInvisible)
        {
            invisibilityPanel.gameObject.SetActive(true);
            invisibilitySlider.DOFillAmount(0, maxTimeInvisible);
        }

        public void RestoreInvisibility(float recoveryTime)
        {
            invisibilitySlider.DOKill();
            invisibilitySlider.fillAmount = 0;
            invisibilitySlider.DOFillAmount(1, recoveryTime).OnComplete(() =>
            {
                invisibilityPanel.gameObject.SetActive(false);
            });
        }

        public void StartHack(float hackTime)
        {
            hackPanel.gameObject.SetActive(true);
            hackSlider.DOFillAmount(0, hackTime);
        }

        public void RestoreHack(float recoveryTime)
        {
            hackSlider.fillAmount = 0;
            hackSlider.DOKill();
            hackSlider.DOFillAmount(1, recoveryTime).OnComplete(() =>
            {
                hackPanel.gameObject.SetActive(false);
            });
        }

        public void RestoreTranslocator(float recoveryTime)
        {
            translocatorPanel.gameObject.SetActive(true);
            translocatorSlider.DOKill();
            translocatorSlider.fillAmount = 0;
            translocatorSlider.DOFillAmount(1, recoveryTime).OnComplete(() =>
            {
                translocatorPanel.gameObject.SetActive(false);
            });
        }
    }
}
