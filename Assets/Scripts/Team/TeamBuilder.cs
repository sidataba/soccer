using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Team builder and management system
/// Handles team creation, customization, saving/loading
/// </summary>
public class TeamBuilder : MonoBehaviour
{
    public static TeamBuilder Instance { get; private set; }

    [Header("Current Teams")]
    [SerializeField] private TeamData team1Data;
    [SerializeField] private TeamData team2Data;

    [Header("Preset Teams")]
    [SerializeField] private List<TeamData> presetTeams = new List<TeamData>();

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "teams.json";

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeDefaultTeams();
    }

    void Start()
    {
        LoadTeams();
    }

    #region Team Management

    private void InitializeDefaultTeams()
    {
        if (team1Data == null)
        {
            team1Data = CreatePresetTeam("Red Devils", Color.red, Color.white);
        }

        if (team2Data == null)
        {
            team2Data = CreatePresetTeam("Blue Eagles", Color.blue, Color.yellow);
        }

        // Create some preset teams
        if (presetTeams.Count == 0)
        {
            presetTeams.Add(CreatePresetTeam("Red Devils", Color.red, Color.white));
            presetTeams.Add(CreatePresetTeam("Blue Eagles", Color.blue, Color.yellow));
            presetTeams.Add(CreatePresetTeam("Green Warriors", Color.green, Color.black));
            presetTeams.Add(CreatePresetTeam("Yellow Lightning", Color.yellow, Color.red));
            presetTeams.Add(CreatePresetTeam("Purple Titans", new Color(0.5f, 0f, 0.5f), Color.white));
            presetTeams.Add(CreatePresetTeam("Orange Flames", new Color(1f, 0.5f, 0f), Color.black));
            presetTeams.Add(CreatePresetTeam("Black Panthers", Color.black, Color.white));
            presetTeams.Add(CreatePresetTeam("White Knights", Color.white, Color.blue));
        }
    }

    private TeamData CreatePresetTeam(string name, Color primary, Color secondary)
    {
        TeamData team = new TeamData(name, primary);
        team.secondaryColor = secondary;

        // Generate player names based on team name
        GeneratePlayerNames(team);

        return team;
    }

    private void GeneratePlayerNames(TeamData team)
    {
        string[] firstNames = { "John", "Mike", "Alex", "Chris", "David", "James", "Robert", "Daniel", "Ryan", "Kevin", "Tom" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Wilson" };

        for (int i = 0; i < team.players.Count; i++)
        {
            string firstName = firstNames[Random.Range(0, firstNames.Length)];
            string lastName = lastNames[Random.Range(0, lastNames.Length)];
            team.players[i].playerName = $"{firstName} {lastName}";
        }
    }

    public TeamData CreateCustomTeam(string teamName, Color primaryColor, Color secondaryColor)
    {
        TeamData newTeam = new TeamData(teamName, primaryColor);
        newTeam.secondaryColor = secondaryColor;
        GeneratePlayerNames(newTeam);
        return newTeam;
    }

    #endregion

    #region Getters & Setters

    public TeamData GetTeam1Data() => team1Data;
    public TeamData GetTeam2Data() => team2Data;

    public void SetTeam1Data(TeamData data)
    {
        team1Data = data;
    }

    public void SetTeam2Data(TeamData data)
    {
        team2Data = data;
    }

    public List<TeamData> GetPresetTeams() => presetTeams;

    public TeamData GetPresetTeam(int index)
    {
        if (index >= 0 && index < presetTeams.Count)
        {
            return presetTeams[index];
        }
        return null;
    }

    #endregion

    #region Team Customization

    public void CustomizeTeamName(TeamData team, string newName)
    {
        team.teamName = newName;
        team.teamAbbreviation = GetAbbreviation(newName);
    }

    public void CustomizeTeamColors(TeamData team, Color primary, Color secondary)
    {
        team.primaryColor = primary;
        team.secondaryColor = secondary;
    }

    public void CustomizePlayer(TeamData team, int playerIndex, string newName, PlayerPosition position, int number)
    {
        if (playerIndex >= 0 && playerIndex < team.players.Count)
        {
            team.players[playerIndex].playerName = newName;
            team.players[playerIndex].position = position;
            team.players[playerIndex].jerseyNumber = number;
        }
    }

    public void SetFormation(TeamData team, FormationType formation)
    {
        team.formation = formation;
    }

    public void RandomizeTeam(TeamData team)
    {
        // Randomize colors
        team.primaryColor = new Color(Random.value, Random.value, Random.value);
        team.secondaryColor = new Color(Random.value, Random.value, Random.value);

        // Randomize player names
        GeneratePlayerNames(team);

        // Randomize formation
        System.Array formations = System.Enum.GetValues(typeof(FormationType));
        team.formation = (FormationType)formations.GetValue(Random.Range(0, formations.Length));
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

    #endregion

    #region Save/Load System

    public void SaveTeams()
    {
        TeamSaveData saveData = new TeamSaveData
        {
            team1 = team1Data,
            team2 = team2Data,
            customTeams = new List<TeamData>(presetTeams)
        };

        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(SavePath, json);
            Debug.Log($"Teams saved to: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save teams: {e.Message}");
        }
    }

    public void LoadTeams()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No saved teams found. Using defaults.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            TeamSaveData saveData = JsonUtility.FromJson<TeamSaveData>(json);

            if (saveData != null)
            {
                team1Data = saveData.team1 ?? team1Data;
                team2Data = saveData.team2 ?? team2Data;

                if (saveData.customTeams != null && saveData.customTeams.Count > 0)
                {
                    presetTeams = saveData.customTeams;
                }

                Debug.Log("Teams loaded successfully!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load teams: {e.Message}");
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save data deleted.");
            InitializeDefaultTeams();
        }
    }

    #endregion

    #region Formation Positions

    public Vector3[] GetFormationPositions(FormationType formation, bool isTeam1)
    {
        float xMultiplier = isTeam1 ? -1f : 1f;
        Vector3[] positions;

        switch (formation)
        {
            case FormationType.Formation_4_4_2:
                positions = new Vector3[]
                {
                    new Vector3(8f * xMultiplier, 0f, 0f),      // GK
                    new Vector3(5f * xMultiplier, 3f, 0f),      // DEF
                    new Vector3(5f * xMultiplier, 1f, 0f),
                    new Vector3(5f * xMultiplier, -1f, 0f),
                    new Vector3(5f * xMultiplier, -3f, 0f),
                    new Vector3(2f * xMultiplier, 2f, 0f),      // MID
                    new Vector3(2f * xMultiplier, 0.7f, 0f),
                    new Vector3(2f * xMultiplier, -0.7f, 0f),
                    new Vector3(2f * xMultiplier, -2f, 0f),
                    new Vector3(-1f * xMultiplier, 1.5f, 0f),   // FWD
                    new Vector3(-1f * xMultiplier, -1.5f, 0f)
                };
                break;

            case FormationType.Formation_4_3_3:
                positions = new Vector3[]
                {
                    new Vector3(8f * xMultiplier, 0f, 0f),      // GK
                    new Vector3(5f * xMultiplier, 3f, 0f),      // DEF
                    new Vector3(5f * xMultiplier, 1f, 0f),
                    new Vector3(5f * xMultiplier, -1f, 0f),
                    new Vector3(5f * xMultiplier, -3f, 0f),
                    new Vector3(3f * xMultiplier, 1.5f, 0f),    // MID
                    new Vector3(3f * xMultiplier, 0f, 0f),
                    new Vector3(3f * xMultiplier, -1.5f, 0f),
                    new Vector3(-1f * xMultiplier, 2.5f, 0f),   // FWD
                    new Vector3(-1f * xMultiplier, 0f, 0f),
                    new Vector3(-1f * xMultiplier, -2.5f, 0f)
                };
                break;

            case FormationType.Formation_3_5_2:
                positions = new Vector3[]
                {
                    new Vector3(8f * xMultiplier, 0f, 0f),      // GK
                    new Vector3(6f * xMultiplier, 2f, 0f),      // DEF
                    new Vector3(6f * xMultiplier, 0f, 0f),
                    new Vector3(6f * xMultiplier, -2f, 0f),
                    new Vector3(3f * xMultiplier, 3f, 0f),      // MID
                    new Vector3(3f * xMultiplier, 1.5f, 0f),
                    new Vector3(3f * xMultiplier, 0f, 0f),
                    new Vector3(3f * xMultiplier, -1.5f, 0f),
                    new Vector3(3f * xMultiplier, -3f, 0f),
                    new Vector3(-1f * xMultiplier, 1.5f, 0f),   // FWD
                    new Vector3(-1f * xMultiplier, -1.5f, 0f)
                };
                break;

            default:
                positions = new Vector3[]
                {
                    new Vector3(8f * xMultiplier, 0f, 0f),      // Default positions
                    new Vector3(5f * xMultiplier, 2f, 0f),
                    new Vector3(5f * xMultiplier, -2f, 0f),
                    new Vector3(3f * xMultiplier, 3f, 0f),
                    new Vector3(3f * xMultiplier, 1f, 0f),
                    new Vector3(3f * xMultiplier, -1f, 0f),
                    new Vector3(3f * xMultiplier, -3f, 0f),
                    new Vector3(0f * xMultiplier, 2f, 0f),
                    new Vector3(0f * xMultiplier, 0f, 0f),
                    new Vector3(0f * xMultiplier, -2f, 0f),
                    new Vector3(-2f * xMultiplier, 0f, 0f)
                };
                break;
        }

        return positions;
    }

    #endregion
}

[System.Serializable]
public class TeamSaveData
{
    public TeamData team1;
    public TeamData team2;
    public List<TeamData> customTeams;
}
