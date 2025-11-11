using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Main game manager - handles match state, scoring, and game flow
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Match Settings")]
    [SerializeField] private float matchDuration = 300f; // 5 minutes
    [SerializeField] private int scoreToWin = 5;

    [Header("References")]
    [SerializeField] private Ball ball;
    [SerializeField] private Transform centerPoint;

    // Match state
    private int team1Score = 0;
    private int team2Score = 0;
    private float matchTime;
    private bool matchActive = false;
    private bool isHalfTime = false;

    // Events
    public delegate void OnScoreChanged(int team1Score, int team2Score);
    public static event OnScoreChanged ScoreChanged;

    public delegate void OnMatchStateChanged(MatchState state);
    public static event OnMatchStateChanged MatchStateChanged;

    public delegate void OnTimeUpdated(float timeRemaining);
    public static event OnTimeUpdated TimeUpdated;

    void Awake()
    {
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
        StartMatch();
    }

    void Update()
    {
        if (matchActive)
        {
            matchTime -= Time.deltaTime;
            TimeUpdated?.Invoke(matchTime);

            if (matchTime <= 0)
            {
                EndMatch();
            }

            // Check for win condition
            if (team1Score >= scoreToWin || team2Score >= scoreToWin)
            {
                EndMatch();
            }
        }
    }

    public void StartMatch()
    {
        matchTime = matchDuration;
        team1Score = 0;
        team2Score = 0;
        matchActive = true;

        MatchStateChanged?.Invoke(MatchState.Playing);
        ScoreChanged?.Invoke(team1Score, team2Score);

        // Play whistle sound
        SoundManager.Instance?.PlayWhistle();

        if (ball != null)
        {
            ball.ResetBall();
        }
    }

    public void AddGoal(TeamType team)
    {
        if (!matchActive) return;

        if (team == TeamType.Team1)
        {
            team1Score++;
        }
        else
        {
            team2Score++;
        }

        ScoreChanged?.Invoke(team1Score, team2Score);

        // Play goal sound
        SoundManager.Instance?.PlayGoal();

        // Reset after goal
        StartCoroutine(ResetAfterGoal());
    }

    private IEnumerator ResetAfterGoal()
    {
        matchActive = false;
        MatchStateChanged?.Invoke(MatchState.GoalScored);

        yield return new WaitForSeconds(2f);

        // Reset ball and players
        if (ball != null)
        {
            ball.ResetBall();
        }

        ResetPlayerPositions();

        yield return new WaitForSeconds(1f);

        matchActive = true;
        MatchStateChanged?.Invoke(MatchState.Playing);
    }

    private void ResetPlayerPositions()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        // Simple positioning - can be improved
        int team1Count = 0;
        int team2Count = 0;

        foreach (PlayerController player in players)
        {
            if (player.GetTeam() == TeamType.Team1)
            {
                player.transform.position = new Vector3(-5 + team1Count * 2, 0, 0);
                team1Count++;
            }
            else
            {
                player.transform.position = new Vector3(5 - team2Count * 2, 0, 0);
                team2Count++;
            }
        }
    }

    private void EndMatch()
    {
        matchActive = false;
        MatchStateChanged?.Invoke(MatchState.MatchEnded);

        // Play end whistle
        SoundManager.Instance?.PlayWhistle();

        // Determine winner
        if (team1Score > team2Score)
        {
            Debug.Log("Team 1 Wins!");
        }
        else if (team2Score > team1Score)
        {
            Debug.Log("Team 2 Wins!");
        }
        else
        {
            Debug.Log("Draw!");
        }
    }

    public void RestartMatch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // Getters
    public int GetTeam1Score() => team1Score;
    public int GetTeam2Score() => team2Score;
    public float GetMatchTime() => matchTime;
    public bool IsMatchActive() => matchActive;
}

public enum MatchState
{
    PreMatch,
    Playing,
    GoalScored,
    HalfTime,
    MatchEnded
}
