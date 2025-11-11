using UnityEngine;
using Steamworks;

/// <summary>
/// Handles Steam leaderboards for high scores
/// </summary>
public class SteamLeaderboards : MonoBehaviour
{
    private static string LEADERBOARD_NAME = "Most_Goals_Scored";

    // Callbacks
    private CallResult<LeaderboardFindResult_t> m_findLeaderboard;
    private CallResult<LeaderboardScoreUploaded_t> m_uploadScore;

    private SteamLeaderboard_t m_currentLeaderboard;

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam not initialized. Leaderboards will not work.");
            return;
        }

        m_findLeaderboard = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFound);
        m_uploadScore = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);

        FindLeaderboard();
    }

    private void FindLeaderboard()
    {
        SteamAPICall_t handle = SteamUserStats.FindLeaderboard(LEADERBOARD_NAME);
        m_findLeaderboard.Set(handle);
    }

    private void OnLeaderboardFound(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bLeaderboardFound == 1)
        {
            m_currentLeaderboard = pCallback.m_hSteamLeaderboard;
            Debug.Log($"Leaderboard found: {LEADERBOARD_NAME}");
        }
        else
        {
            Debug.LogWarning($"Leaderboard not found: {LEADERBOARD_NAME}");
        }
    }

    public void UploadScore(int score)
    {
        if (!SteamManager.Initialized || m_currentLeaderboard.m_SteamLeaderboard == 0)
        {
            return;
        }

        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(
            m_currentLeaderboard,
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
            score,
            null,
            0
        );

        m_uploadScore.Set(handle);
    }

    private void OnScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess == 1)
        {
            Debug.Log($"Score uploaded successfully: {pCallback.m_nScore}");
        }
        else
        {
            Debug.LogWarning("Score upload failed");
        }
    }
}
