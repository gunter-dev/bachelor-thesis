using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuScripts
{
    public class LobbyMenus : MonoBehaviour
    {
        public static bool isPaused;
        public static bool isPlayerDead;
        public static bool playerWon;
        
        public GameObject pauseMenu;
        public GameObject deathScreen;
        public CanvasGroup deathScreenGroup;
        public GameObject winScreen;
        public CanvasGroup winScreenGroup;

        private bool _deathScreenFade;
        private bool _winScreenFade;

        void Update()
        {
            if (deathScreen.activeSelf || winScreen.activeSelf)
            {
                if (_deathScreenFade) DisplayDeathScreen();
                if (_winScreenFade) DisplayWinScreen();
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) Resume();
                else Pause();
            }

            if (isPlayerDead)
            {
                deathScreen.SetActive(true);
                isPlayerDead = false;
                _deathScreenFade = true;
            }

            if (playerWon)
            {
                winScreen.SetActive(true);
                playerWon = false;
                _winScreenFade = true;
            }
        }
        
        private void DisplayDeathScreen()
        {
            if (deathScreenGroup.alpha < 1) deathScreenGroup.alpha += Time.deltaTime;
            else _deathScreenFade = false;
        }

        private void DisplayWinScreen()
        {
            if (winScreenGroup.alpha < 1) winScreenGroup.alpha += Time.deltaTime;
            else _winScreenFade = false;
        }
        
        private void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        public void Next()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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