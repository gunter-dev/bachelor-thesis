using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
    public class LobbyMenus : MonoBehaviour
    {
        public static bool isPaused;
        public static bool isPlayerDead;
        public GameObject pauseMenu;
        public GameObject deathScreen;
        public CanvasGroup deathScreenGroup;

        private bool _fade;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !deathScreen.activeSelf)
            {
                if (isPaused) Resume();
                else Pause();
            }

            if (isPlayerDead)
            {
                deathScreen.SetActive(true);
                isPlayerDead = false;
                _fade = true;
            }
            if (_fade) DisplayDeathScreen();
        }
        
        private void DisplayDeathScreen()
        {
            if (deathScreenGroup.alpha < 1) deathScreenGroup.alpha += Time.deltaTime;
            else _fade = false;
        }

        private void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        public void Restart()
        {
            deathScreen.SetActive(false);
            Resume();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(Constants.MainMenu);
            Resume();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}