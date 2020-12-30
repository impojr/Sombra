using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class LevelSelectManager : ScreenManager
    {
        public void LoadLevelOne()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(4);
            });
        }

        public void LoadLevelTwo()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(5);
            });
        }

        public void LoadLevelThree()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(6);
            });
        }

        public void LoadLevelFour()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(7);
            });
        }
        public void LoadLevelFive()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(8);
            });
        }
        public void LoadLevelSix()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(9);
            });
        }
        public void LoadLevelSeven()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(10);
            });
        }
        public void LoadLevelEight()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(11);
            });
        }
        public void LoadLevelNine()
        {
            FadeOut(() =>
            {
                SceneManager.LoadScene(12);
            });
        }
    }
}
