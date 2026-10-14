using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Hearts")]
    public int maxHearts = 3;
    [Tooltip("Drag the heart images here (first heart = first one lost).")]
    public GameObject[] heartIcons;

    [Header("UI")]
    public TMP_Text scoreText;
    [Tooltip("Start menu panel with the Play and Quit buttons (optional).")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText; // optional

    // Fired every time the score changes (the spawner listens to this)
    public static event System.Action<int> OnScoreChanged;

    // Fired once when all hearts are lost (the player listens to this)
    public static event System.Action OnGameOver;

    // Stays true after a restart so the start menu isn't shown again
    private static bool skipStartMenu = false;

    private int score = 0;
    private int hearts;
    private bool isGameOver = false;

    void OnEnable()
    {
        Candy.OnCandyCaught += HandleCandyCaught;
        Candy.OnCandyMissed += HandleCandyMissed;
        DangerousCandy.OnDangerousCandyCaught += HandleDangerousCaught;
    }

    void OnDisable()
    {
        Candy.OnCandyCaught -= HandleCandyCaught;
        Candy.OnCandyMissed -= HandleCandyMissed;
        DangerousCandy.OnDangerousCandyCaught -= HandleDangerousCaught;
    }

    void Start()
    {
        // Show the start menu (game frozen) unless we just restarted
        if (startPanel != null && !skipStartMenu)
        {
            startPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            if (startPanel != null) startPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        skipStartMenu = false;

        hearts = maxHearts;
        score = 0;
        isGameOver = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateScoreUI();
        UpdateHeartsUI();
    }

    private void HandleCandyCaught(int points)
    {
        if (isGameOver) return;
        score += points;
        UpdateScoreUI();
    }

    private void HandleCandyMissed(int points)
    {
        if (isGameOver) return;
        score = Mathf.Max(0, score - points); // score can't go below 0
        UpdateScoreUI();
    }

    private void HandleDangerousCaught()
    {
        if (isGameOver) return;
        hearts--;
        UpdateHeartsUI();

        if (hearts <= 0) GameOver();
    }

    private void GameOver()
    {
        isGameOver = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "Score: " + score;
        OnGameOver?.Invoke();
        Time.timeScale = 0f; // freezes spawner, candies and player
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "" + score;
        OnScoreChanged?.Invoke(score);
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < hearts);
        }
    }

    // Hook this to the Play button's OnClick
    public void StartGame()
    {
        if (startPanel != null) startPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Hook this to the Quit button's OnClick
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops Play mode in the editor
#else
        Application.Quit();
#endif
    }

    // Hook this to the Restart button's OnClick
    public void RestartGame()
    {
        skipStartMenu = true; // go straight into the game after reloading
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
