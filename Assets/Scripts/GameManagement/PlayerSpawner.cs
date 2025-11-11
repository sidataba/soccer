using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Spawns players for local multiplayer with proper input device assignment
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

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
        // For now, spawn 2 players per team (4 players total)
        // This can be made configurable through a main menu

        // Team 1 players
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPos = team1SpawnPoints != null && team1SpawnPoints.Length > i
                ? team1SpawnPoints[i].position
                : new Vector3(-5 + i * 2, i * 2 - 1, 0);

            SpawnPlayer(spawnPos, TeamType.Team1, i + 1, team1Color);
        }

        // Team 2 players
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPos = team2SpawnPoints != null && team2SpawnPoints.Length > i
                ? team2SpawnPoints[i].position
                : new Vector3(5 - i * 2, i * 2 - 1, 0);

            SpawnPlayer(spawnPos, TeamType.Team2, i + 1, team2Color);
        }
    }

    private void SpawnPlayer(Vector3 position, TeamType team, int playerNumber, Color color)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.name = $"{team}_Player{playerNumber}";

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetTeam(team);
            controller.SetPlayerColor(color);
        }

        // Setup Input System for multiplayer
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.defaultControlScheme = playerNumber <= 2 ? $"Keyboard{playerNumber}" : "Gamepad";
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
