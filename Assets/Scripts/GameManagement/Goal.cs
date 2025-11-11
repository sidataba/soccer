using UnityEngine;

/// <summary>
/// Detects when ball enters goal and triggers scoring
/// </summary>
public class Goal : MonoBehaviour
{
    [SerializeField] private TeamType scoringTeam; // Which team scores when ball enters this goal

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // Opposite team scores (own goal protection)
            TeamType teamThatScored = scoringTeam == TeamType.Team1 ? TeamType.Team2 : TeamType.Team1;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddGoal(teamThatScored);
                Debug.Log($"{teamThatScored} scored!");
            }
        }
    }
}
