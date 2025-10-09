using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Panel principal avec PLAY/RULES/QUIT")]
    public GameObject mainMenuPanel;
    
    [Tooltip("Panel contenant les règles du jeu")]
    public GameObject rulesPanel;

    void Start()
    {
        // Afficher le menu principal au démarrage
        ShowMainMenu();
        
        // Rendre le curseur visible dans le menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Affiche le menu principal
    /// </summary>
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        
        if (rulesPanel != null)
            rulesPanel.SetActive(false);
    }

    /// <summary>
    /// Affiche le panneau des règles
    /// </summary>
    public void ShowRules()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (rulesPanel != null)
            rulesPanel.SetActive(true);
    }

    /// <summary>
    /// Lance la scène finale Scene_Liam_2
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene_Liam_2");
    }

    /// <summary>
    /// Quitte l'application
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }
}