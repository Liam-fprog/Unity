using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [Tooltip("Panel contenant les boutons PLAY et QUIT")]
    public GameObject mainMenuPanel;
    
    [Tooltip("Panel contenant les boutons LAB1 et LAB2")]
    public GameObject mapSelectionPanel;

    void Start()
    {
        // Au démarrage, afficher le menu principal et cacher la sélection de map
        ShowMainMenu();
        
        // Rendre le curseur visible dans le menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // Raccourci clavier : Échap pour quitter
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si on est dans la sélection de map, revenir au menu principal
            if (mapSelectionPanel.activeSelf)
            {
                ShowMainMenu();
            }
            else
            {
                QuitGame();
            }
        }
    }

    // ========== NAVIGATION ENTRE MENUS ==========
    
    /// Affiche le menu principal (PLAY / QUIT)
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        mapSelectionPanel.SetActive(false);
    }

    /// Affiche le menu de sélection de map (LAB1 / LAB2)
    /// Appelé quand on clique sur PLAY
    public void ShowMapSelection()
    {
        mainMenuPanel.SetActive(false);
        mapSelectionPanel.SetActive(true);
    }

    // ========== CHARGEMENT DES SCÈNES ==========

    /// Charge la scène Lab1
    public void LoadLab1()
    {
        SceneManager.LoadScene("lab1");
        // Ou par index : SceneManager.LoadScene(1);
    }

    /// Charge la scène Lab2
    public void LoadLab2()
    {
        SceneManager.LoadScene("lab2");
        // Ou par index : SceneManager.LoadScene(2);
    }

    /// Quitte l'application
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }

    // ========== FONCTIONS LoadScene ==========

    /// Fonction générique pour charger n'importe quelle scène par nom
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// Fonction générique pour charger n'importe quelle scène par index
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}