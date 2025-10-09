using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text timerText;
    public GameObject winPanel;
    public TMP_Text finalTimeText;
    public TMP_Text bestTimeText;
    private float elapsedTime;
    private bool isGameRunning = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        elapsedTime = 0f;
        isGameRunning = true;

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void WinGame()
    {
        isGameRunning = false;
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (elapsedTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", elapsedTime);
            bestTime = elapsedTime;
        }
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (finalTimeText != null)
                finalTimeText.text = $" Temps final : {FormatTime(elapsedTime)}";
            if (bestTimeText != null)
                bestTimeText.text = $" Meilleur temps : {FormatTime(bestTime)}";
        }

        Time.timeScale = 0f; //Pause
    }

    public void GameOver()
    {
        isGameRunning = false;
        Debug.Log("Perdu");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
