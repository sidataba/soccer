using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to quickly set up the game scene
/// Attach to an empty GameObject and click "Setup Scene" in the Inspector
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject playerPrefab;

    [Header("Configuration")]
    public Color team1Color = Color.red;
    public Color team2Color = Color.blue;
    public float fieldWidth = 20f;
    public float fieldHeight = 12f;

    #if UNITY_EDITOR
    [ContextMenu("Setup Complete Scene")]
    public void SetupCompleteScene()
    {
        Debug.Log("Setting up soccer game scene...");

        // Create Ball
        CreateBall();

        // Create Field Manager
        CreateFieldManager();

        // Create Game Manager
        CreateGameManager();

        // Create Player Spawner
        CreatePlayerSpawner();

        // Create UI
        CreateUI();

        // Setup Camera
        SetupCamera();

        Debug.Log("Scene setup complete! Press Play to start the game.");
    }

    private void CreateBall()
    {
        if (GameObject.Find("Ball") != null)
        {
            Debug.Log("Ball already exists, skipping...");
            return;
        }

        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        ball.tag = "Ball";
        ball.transform.position = Vector3.zero;
        ball.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Remove 3D components
        DestroyImmediate(ball.GetComponent<SphereCollider>());

        // Add 2D components
        Rigidbody2D rb = ball.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearDamping = 0.5f;

        CircleCollider2D col = ball.AddComponent<CircleCollider2D>();

        ball.AddComponent<Ball>();

        // Make it white
        ball.GetComponent<Renderer>().material.color = Color.white;

        Debug.Log("✓ Ball created");
    }

    private void CreateFieldManager()
    {
        if (GameObject.Find("FieldManager") != null)
        {
            Debug.Log("FieldManager already exists, skipping...");
            return;
        }

        GameObject fieldManager = new GameObject("FieldManager");
        FieldManager fm = fieldManager.AddComponent<FieldManager>();

        Debug.Log("✓ FieldManager created");
    }

    private void CreateGameManager()
    {
        if (GameObject.Find("GameManager") != null)
        {
            Debug.Log("GameManager already exists, skipping...");
            return;
        }

        GameObject gameManager = new GameObject("GameManager");
        GameManager gm = gameManager.AddComponent<GameManager>();

        // Find and assign ball reference
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            SerializedObject so = new SerializedObject(gm);
            so.FindProperty("ball").objectReferenceValue = ball;
            so.ApplyModifiedProperties();
        }

        Debug.Log("✓ GameManager created");
    }

    private void CreatePlayerSpawner()
    {
        if (GameObject.Find("PlayerSpawner") != null)
        {
            Debug.Log("PlayerSpawner already exists, skipping...");
            return;
        }

        GameObject spawner = new GameObject("PlayerSpawner");
        PlayerSpawner ps = spawner.AddComponent<PlayerSpawner>();

        if (playerPrefab != null)
        {
            SerializedObject so = new SerializedObject(ps);
            so.FindProperty("playerPrefab").objectReferenceValue = playerPrefab;
            so.ApplyModifiedProperties();
        }

        Debug.Log("✓ PlayerSpawner created");
        if (playerPrefab == null)
        {
            Debug.LogWarning("⚠ Player prefab not assigned! Please assign it manually or create the player prefab first.");
        }
    }

    private void CreateUI()
    {
        if (GameObject.Find("GameUI") != null)
        {
            Debug.Log("GameUI already exists, skipping...");
            return;
        }

        // This would need UnityEngine.UI and TMPro to be fully implemented
        // For now, just create the basic structure
        GameObject canvas = new GameObject("GameUI");
        canvas.AddComponent<Canvas>();
        canvas.AddComponent<UIManager>();

        Debug.Log("✓ GameUI created (requires manual TextMeshPro setup)");
        Debug.LogWarning("⚠ Please add Score and Timer UI elements manually following the SETUP_GUIDE.md");
    }

    private void SetupCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.orthographic = true;
            mainCam.orthographicSize = 10f;
            mainCam.transform.position = new Vector3(0, 0, -10);
            mainCam.backgroundColor = new Color(0.059f, 0.318f, 0.196f); // Dark green

            Debug.Log("✓ Camera configured");
        }
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneSetupHelper))]
public class SceneSetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneSetupHelper helper = (SceneSetupHelper)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Click the button below to automatically set up the game scene.", MessageType.Info);

        if (GUILayout.Button("Setup Complete Scene", GUILayout.Height(40)))
        {
            helper.SetupCompleteScene();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Note: You'll still need to create the Player prefab manually and assign it above, then click the button again. See SETUP_GUIDE.md for details.", MessageType.Warning);
    }
}
#endif
