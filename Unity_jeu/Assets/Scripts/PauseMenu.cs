using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel UI du menu pause")]
    public GameObject pauseMenuPanel;

    [Header("Settings")]
    [Tooltip("Nom de la scène du menu principal")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        // S'assurer que le menu est caché au démarrage
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // S'assurer que le jeu n'est pas en pause
        ResumeGame();
    }

    void Update()
    {
        // Détection de la touche echap pour pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// Met le jeu en pause et affiche le menu
    public void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Arrêter le temps du jeu
        Time.timeScale = 0f;

        // Afficher et déverrouiller le curseur
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isPaused = true;
    }

    /// Reprend le jeu et cache le menu
    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Reprendre le temps du jeu
        Time.timeScale = 1f;

        // Cacher et verrouiller le curseur
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
    }

    /// Retourne au menu principal
    public void LoadMainMenu()
    {
        // IMPORTANT : Remettre le temps à la normale avant de changer de scène
        Time.timeScale = 1f;

        // Afficher le curseur pour le menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Charger le menu principal
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// Recommence la scène actuelle depuis le début
    public void RestartLevel()
    {
        // IMPORTANT : Remettre le temps à la normale avant de recharger
        Time.timeScale = 1f;

        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// Fonction pour charger n'importe quelle scène
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    /// Fonction appelée quand le script est désactivé
    /// Pour s'assurer que le temps revient à la normale
    void OnDisable()
    {
        Time.timeScale = 1f;
    }

    /// Fonction appelée quand l'application est mise en pause (ALT+TAB sur PC, etc.)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !isPaused)
        {
            // Si l'application est mise en pause et le jeu n'est pas déjà en pause
            PauseGame();
        }
    }
}