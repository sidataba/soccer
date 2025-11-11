using UnityEngine;
using Steamworks;

/// <summary>
/// Handles Steam achievements
/// </summary>
public class SteamAchievements : MonoBehaviour
{
    // Achievement identifiers (these need to be set up in Steamworks Partner site)
    private enum Achievement
    {
        FIRST_GOAL,
        WIN_MATCH,
        SCORE_5_GOALS,
        WIN_10_MATCHES
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam not initialized. Achievements will not work.");
            return;
        }

        // Request current stats and achievements from Steam servers
        SteamUserStats.RequestCurrentStats();
    }

    /// <summary>
    /// Unlocks a specific achievement
    /// </summary>
    public static void UnlockAchievement(string achievementName)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        SteamUserStats.SetAchievement(achievementName);
        SteamUserStats.StoreStats();

        Debug.Log($"Achievement Unlocked: {achievementName}");
    }

    /// <summary>
    /// Example: Unlock first goal achievement
    /// </summary>
    public static void OnFirstGoalScored()
    {
        UnlockAchievement("FIRST_GOAL");
    }

    /// <summary>
    /// Example: Unlock match win achievement
    /// </summary>
    public static void OnMatchWon()
    {
        UnlockAchievement("WIN_MATCH");
    }

    /// <summary>
    /// Updates a stat-based achievement
    /// </summary>
    public static void UpdateStatAchievement(string statName, int value)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        SteamUserStats.SetStat(statName, value);
        SteamUserStats.StoreStats();
    }
}
