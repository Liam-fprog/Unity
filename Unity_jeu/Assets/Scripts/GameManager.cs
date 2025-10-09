using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton

    [Header("UI Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Cacher les panels au démarrage
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    /// <summary>
    /// Appelé quand le joueur gagne
    /// </summary>
    public void TriggerVictory()
    {
        Debug.Log("🎉 VICTOIRE !");
        
        // Afficher le panel de victoire
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
        
        // Arrêter le temps
        Time.timeScale = 0f;
        
        // Montrer le curseur
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Appelé quand le joueur perd
    /// </summary>
    public void TriggerDefeat()
    {
        Debug.Log("💀 DÉFAITE !");
        
        // Afficher le panel de défaite
        if (defeatPanel != null)
            defeatPanel.SetActive(true);
        
        // Arrêter le temps
        Time.timeScale = 0f;
        
        // Montrer le curseur
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Rejouer le niveau actuel
    /// </summary>
    public void RestartLevel()
    {
        // Remettre le temps à la normale
        Time.timeScale = 1f;
        
        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Retourner au menu principal
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Remettre le temps à la normale
        Time.timeScale = 1f;
        
        // Charger le menu principal
        SceneManager.LoadScene("MainMenu");
    }
}