using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Spawns players for local multiplayer with proper input device assignment
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Game Mode")]
    [SerializeField] private GameMode gameMode = GameMode.MultiplayerLocal;
    [SerializeField] private AIDifficulty aiDifficulty = AIDifficulty.Medium;

    [Header("Team Colors")]
    [SerializeField] private Color team1Color = Color.red;
    [SerializeField] private Color team2Color = Color.blue;

    [Header("Spawn Positions")]
    [SerializeField] private Transform[] team1SpawnPoints;
    [SerializeField] private Transform[] team2SpawnPoints;

    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
            return;
        }

        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        switch (gameMode)
        {
            case GameMode.SinglePlayerVsAI:
                // Player controls Team 1, AI controls Team 2
                SpawnPlayer(GetSpawnPosition(TeamType.Team1, 0), TeamType.Team1, 1, team1Color, false);
                SpawnPlayer(GetSpawnPosition(TeamType.Team1, 1), TeamType.Team1, 2, team1Color, false);
                SpawnPlayer(GetSpawnPosition(TeamType.Team2, 0), TeamType.Team2, 1, team2Color, true);
                SpawnPlayer(GetSpawnPosition(TeamType.Team2, 1), TeamType.Team2, 2, team2Color, true);
                break;

            case GameMode.MultiplayerLocal:
                // 2 human players per team (4 total)
                SpawnPlayer(GetSpawnPosition(TeamType.Team1, 0), TeamType.Team1, 1, team1Color, false);
                SpawnPlayer(GetSpawnPosition(TeamType.Team1, 1), TeamType.Team1, 2, team1Color, false);
                SpawnPlayer(GetSpawnPosition(TeamType.Team2, 0), TeamType.Team2, 1, team2Color, false);
                SpawnPlayer(GetSpawnPosition(TeamType.Team2, 1), TeamType.Team2, 2, team2Color, false);
                break;

            case GameMode.AIvsAI:
                // Watch AI play against AI
                SpawnPlayer(GetSpawnPosition(TeamType.Team1, 0), TeamType.Team1, 1, team1Color, true);
                SpawnPlayer(GetSpawnPosition(TeamType.Team1, 1), TeamType.Team1, 2, team1Color, true);
                SpawnPlayer(GetSpawnPosition(TeamType.Team2, 0), TeamType.Team2, 1, team2Color, true);
                SpawnPlayer(GetSpawnPosition(TeamType.Team2, 1), TeamType.Team2, 2, team2Color, true);
                break;
        }
    }

    private Vector3 GetSpawnPosition(TeamType team, int index)
    {
        Transform[] spawnPoints = team == TeamType.Team1 ? team1SpawnPoints : team2SpawnPoints;

        if (spawnPoints != null && spawnPoints.Length > index)
        {
            return spawnPoints[index].position;
        }

        // Default positions
        float xBase = team == TeamType.Team1 ? -5 : 5;
        float xOffset = team == TeamType.Team1 ? index * 2 : -index * 2;
        return new Vector3(xBase + xOffset, index * 2 - 1, 0);
    }

    private void SpawnPlayer(Vector3 position, TeamType team, int playerNumber, Color color, bool isAI)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.name = $"{team}_Player{playerNumber}" + (isAI ? "_AI" : "");

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetTeam(team);
            controller.SetPlayerColor(color);
        }

        if (isAI)
        {
            // Add AI controller and disable human input
            AIController aiController = player.AddComponent<AIController>();
            aiController.SetDifficulty(aiDifficulty);

            // Disable PlayerInput for AI
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
        }
        else
        {
            // Setup Input System for human players
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.defaultControlScheme = playerNumber <= 2 ? $"Keyboard{playerNumber}" : "Gamepad";
            }
        }

        // Add animator component
        if (player.GetComponent<PlayerAnimator>() == null)
        {
            player.AddComponent<PlayerAnimator>();
        }
    }

    public void CreateSpawnPoints()
    {
        // Helper method to create spawn point transforms
        GameObject team1Parent = new GameObject("Team1SpawnPoints");
        GameObject team2Parent = new GameObject("Team2SpawnPoints");
        team1Parent.transform.SetParent(transform);
        team2Parent.transform.SetParent(transform);

        team1SpawnPoints = new Transform[2];
        team2SpawnPoints = new Transform[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject sp1 = new GameObject($"SpawnPoint{i + 1}");
            sp1.transform.SetParent(team1Parent.transform);
            sp1.transform.position = new Vector3(-5 + i * 2, i * 2 - 1, 0);
            team1SpawnPoints[i] = sp1.transform;

            GameObject sp2 = new GameObject($"SpawnPoint{i + 1}");
            sp2.transform.SetParent(team2Parent.transform);
            sp2.transform.position = new Vector3(5 - i * 2, i * 2 - 1, 0);
            team2SpawnPoints[i] = sp2.transform;
        }
    }
}

public enum GameMode
{
    SinglePlayerVsAI,
    MultiplayerLocal,
    AIvsAI
}
