using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data structure for team information
/// Stores team name, colors, formation, player names, and stats
/// </summary>
[System.Serializable]
public class TeamData
{
    [Header("Team Identity")]
    public string teamName = "My Team";
    public string teamAbbreviation = "MT";
    public Color primaryColor = Color.red;
    public Color secondaryColor = Color.white;
    public Color goalKeeperColor = Color.yellow;

    [Header("Formation")]
    public FormationType formation = FormationType.Formation_4_4_2;
    public float offensiveness = 0.5f;  // 0 = defensive, 1 = offensive

    [Header("Players")]
    public List<PlayerData> players = new List<PlayerData>();

    [Header("Team Stats")]
    public int gamesPlayed = 0;
    public int gamesWon = 0;
    public int gamesDrawn = 0;
    public int gamesLost = 0;
    public int goalsScored = 0;
    public int goalsConceded = 0;

    // Constructor
    public TeamData()
    {
        InitializeDefaultPlayers();
    }

    public TeamData(string name, Color color)
    {
        teamName = name;
        primaryColor = color;
        teamAbbreviation = GetAbbreviation(name);
        InitializeDefaultPlayers();
    }

    private void InitializeDefaultPlayers()
    {
        if (players.Count == 0)
        {
            // Create default squad of 11 players
            players = new List<PlayerData>
            {
                new PlayerData("Goalkeeper", PlayerPosition.Goalkeeper, 1),
                new PlayerData("Defender 1", PlayerPosition.Defender, 2),
                new PlayerData("Defender 2", PlayerPosition.Defender, 3),
                new PlayerData("Defender 3", PlayerPosition.Defender, 4),
                new PlayerData("Defender 4", PlayerPosition.Defender, 5),
                new PlayerData("Midfielder 1", PlayerPosition.Midfielder, 6),
                new PlayerData("Midfielder 2", PlayerPosition.Midfielder, 7),
                new PlayerData("Midfielder 3", PlayerPosition.Midfielder, 8),
                new PlayerData("Midfielder 4", PlayerPosition.Midfielder, 9),
                new PlayerData("Forward 1", PlayerPosition.Forward, 10),
                new PlayerData("Forward 2", PlayerPosition.Forward, 11)
            };
        }
    }

    private string GetAbbreviation(string name)
    {
        if (string.IsNullOrEmpty(name)) return "XX";

        string[] words = name.Split(' ');
        if (words.Length >= 2)
        {
            return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
        }
        else if (name.Length >= 2)
        {
            return name.Substring(0, 2).ToUpper();
        }

        return name.ToUpper();
    }

    public int GetTotalPoints()
    {
        return (gamesWon * 3) + gamesDrawn;
    }

    public float GetWinPercentage()
    {
        if (gamesPlayed == 0) return 0f;
        return ((float)gamesWon / gamesPlayed) * 100f;
    }

    public int GetGoalDifference()
    {
        return goalsScored - goalsConceded;
    }

    public void RecordMatchResult(bool won, bool draw, int goalsFor, int goalsAgainst)
    {
        gamesPlayed++;
        if (won)
            gamesWon++;
        else if (draw)
            gamesDrawn++;
        else
            gamesLost++;

        goalsScored += goalsFor;
        goalsConceded += goalsAgainst;
    }

    public List<PlayerData> GetPlayersByPosition(PlayerPosition position)
    {
        return players.FindAll(p => p.position == position);
    }

    public PlayerData GetPlayerByNumber(int number)
    {
        return players.Find(p => p.jerseyNumber == number);
    }
}

/// <summary>
/// Individual player data
/// </summary>
[System.Serializable]
public class PlayerData
{
    public string playerName = "Player";
    public int jerseyNumber = 0;
    public PlayerPosition position = PlayerPosition.Midfielder;

    [Header("Attributes")]
    [Range(1, 99)] public int speed = 70;
    [Range(1, 99)] public int shooting = 70;
    [Range(1, 99)] public int passing = 70;
    [Range(1, 99)] public int dribbling = 70;
    [Range(1, 99)] public int defending = 70;
    [Range(1, 99)] public int physical = 70;

    [Header("Stats")]
    public int gamesPlayed = 0;
    public int goals = 0;
    public int assists = 0;
    public int yellowCards = 0;
    public int redCards = 0;

    // Constructor
    public PlayerData(string name, PlayerPosition pos, int number)
    {
        playerName = name;
        position = pos;
        jerseyNumber = number;
        GenerateAttributesForPosition();
    }

    private void GenerateAttributesForPosition()
    {
        // Generate realistic attributes based on position
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                speed = Random.Range(50, 70);
                shooting = Random.Range(30, 50);
                passing = Random.Range(50, 70);
                dribbling = Random.Range(30, 50);
                defending = Random.Range(60, 80);
                physical = Random.Range(70, 90);
                break;

            case PlayerPosition.Defender:
                speed = Random.Range(60, 80);
                shooting = Random.Range(40, 60);
                passing = Random.Range(60, 80);
                dribbling = Random.Range(50, 70);
                defending = Random.Range(70, 90);
                physical = Random.Range(70, 90);
                break;

            case PlayerPosition.Midfielder:
                speed = Random.Range(65, 85);
                shooting = Random.Range(60, 80);
                passing = Random.Range(70, 90);
                dribbling = Random.Range(70, 90);
                defending = Random.Range(60, 80);
                physical = Random.Range(60, 80);
                break;

            case PlayerPosition.Forward:
                speed = Random.Range(70, 95);
                shooting = Random.Range(75, 95);
                passing = Random.Range(60, 80);
                dribbling = Random.Range(75, 95);
                defending = Random.Range(30, 50);
                physical = Random.Range(60, 80);
                break;
        }
    }

    public int GetOverallRating()
    {
        return (speed + shooting + passing + dribbling + defending + physical) / 6;
    }

    public void RecordGoal()
    {
        goals++;
    }

    public void RecordAssist()
    {
        assists++;
    }

    public string GetPositionAbbreviation()
    {
        switch (position)
        {
            case PlayerPosition.Goalkeeper: return "GK";
            case PlayerPosition.Defender: return "DEF";
            case PlayerPosition.Midfielder: return "MID";
            case PlayerPosition.Forward: return "FWD";
            default: return "???";
        }
    }
}

public enum PlayerPosition
{
    Goalkeeper,
    Defender,
    Midfielder,
    Forward
}

public enum FormationType
{
    Formation_4_4_2,    // Balanced
    Formation_4_3_3,    // Attacking
    Formation_3_5_2,    // Midfield control
    Formation_5_3_2,    // Defensive
    Formation_4_2_3_1,  // Modern
    Formation_3_4_3     // Ultra attacking
}
