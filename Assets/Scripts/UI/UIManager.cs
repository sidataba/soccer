using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages all UI elements including score, timer, and match state displays
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI team1ScoreText;
    [SerializeField] private TextMeshProUGUI team2ScoreText;
    [SerializeField] private TextMeshProUGUI matchTimeText;

    [Header("Match State UI")]
    [SerializeField] private GameObject matchEndPanel;
    [SerializeField] private TextMeshProUGUI matchResultText;
    [SerializeField] private GameObject goalScoredPanel;
    [SerializeField] private TextMeshProUGUI goalScoredText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;

    private bool isPaused = false;

    void OnEnable()
    {
        GameManager.ScoreChanged += UpdateScore;
        GameManager.MatchStateChanged += UpdateMatchState;
        GameManager.TimeUpdated += UpdateTimer;
    }

    void OnDisable()
    {
        GameManager.ScoreChanged -= UpdateScore;
        GameManager.MatchStateChanged -= UpdateMatchState;
        GameManager.TimeUpdated -= UpdateTimer;
    }

    void Start()
    {
        if (matchEndPanel != null)
            matchEndPanel.SetActive(false);

        if (goalScoredPanel != null)
            goalScoredPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Pause/Resume with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void UpdateScore(int team1Score, int team2Score)
    {
        if (team1ScoreText != null)
            team1ScoreText.text = team1Score.ToString();

        if (team2ScoreText != null)
            team2ScoreText.text = team2Score.ToString();
    }

    private void UpdateTimer(float timeRemaining)
    {
        if (matchTimeText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            matchTimeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void UpdateMatchState(MatchState state)
    {
        switch (state)
        {
            case MatchState.Playing:
                if (goalScoredPanel != null)
                    goalScoredPanel.SetActive(false);
                break;

            case MatchState.GoalScored:
                ShowGoalScored();
                break;

            case MatchState.MatchEnded:
                ShowMatchEnd();
                break;
        }
    }

    private void ShowGoalScored()
    {
        if (goalScoredPanel != null && goalScoredText != null)
        {
            goalScoredPanel.SetActive(true);
            goalScoredText.text = "GOAL!";
            Invoke(nameof(HideGoalScored), 2f);
        }
    }

    private void HideGoalScored()
    {
        if (goalScoredPanel != null)
            goalScoredPanel.SetActive(false);
    }

    private void ShowMatchEnd()
    {
        if (matchEndPanel != null && matchResultText != null && GameManager.Instance != null)
        {
            matchEndPanel.SetActive(true);

            int team1Score = GameManager.Instance.GetTeam1Score();
            int team2Score = GameManager.Instance.GetTeam2Score();

            if (team1Score > team2Score)
            {
                matchResultText.text = $"TEAM 1 WINS!\n{team1Score} - {team2Score}";
            }
            else if (team2Score > team1Score)
            {
                matchResultText.text = $"TEAM 2 WINS!\n{team1Score} - {team2Score}";
            }
            else
            {
                matchResultText.text = $"DRAW!\n{team1Score} - {team2Score}";
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartMatch();
        }
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }

    public void OnResumeButton()
    {
        TogglePause();
    }
}
