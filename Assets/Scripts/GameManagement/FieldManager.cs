using UnityEngine;

/// <summary>
/// Creates the soccer field, goals, and boundaries programmatically
/// </summary>
public class FieldManager : MonoBehaviour
{
    [Header("Field Dimensions")]
    [SerializeField] private float fieldWidth = 20f;
    [SerializeField] private float fieldHeight = 12f;
    [SerializeField] private float goalWidth = 3f;
    [SerializeField] private float goalDepth = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;

    void Start()
    {
        CreateField();
    }

    private void CreateField()
    {
        // Create walls (boundaries)
        CreateWall(new Vector3(0, fieldHeight / 2, 0), new Vector2(fieldWidth, 0.5f), "TopWall");
        CreateWall(new Vector3(0, -fieldHeight / 2, 0), new Vector2(fieldWidth, 0.5f), "BottomWall");

        // Create goals (left and right)
        CreateGoalStructure(new Vector3(-fieldWidth / 2, 0, 0), TeamType.Team1, "LeftGoal");
        CreateGoalStructure(new Vector3(fieldWidth / 2, 0, 0), TeamType.Team2, "RightGoal");

        // Create center line visual
        CreateCenterLine();
    }

    private void CreateWall(Vector3 position, Vector2 size, string name)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(transform);
        wall.transform.position = position;
        wall.tag = "Wall";

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;

        SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
        renderer.color = Color.white;
        renderer.sortingOrder = -1;

        // Create sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);

        wall.transform.localScale = new Vector3(size.x, size.y, 1);
    }

    private void CreateGoalStructure(Vector3 position, TeamType team, string name)
    {
        // Goal trigger area
        GameObject goalTrigger = new GameObject(name + "_Trigger");
        goalTrigger.transform.SetParent(transform);
        goalTrigger.transform.position = position;
        goalTrigger.tag = "Goal";

        BoxCollider2D triggerCollider = goalTrigger.AddComponent<BoxCollider2D>();
        triggerCollider.size = new Vector2(goalDepth, goalWidth);
        triggerCollider.isTrigger = true;

        Goal goalScript = goalTrigger.AddComponent<Goal>();

        SpriteRenderer renderer = goalTrigger.AddComponent<SpriteRenderer>();
        renderer.color = team == TeamType.Team1 ? new Color(1, 0, 0, 0.3f) : new Color(0, 0, 1, 0.3f);

        // Create sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);

        goalTrigger.transform.localScale = new Vector3(goalDepth, goalWidth, 1);

        // Goal posts (top and bottom)
        float postOffset = goalWidth / 2;
        float postX = team == TeamType.Team1 ? -fieldWidth / 2 : fieldWidth / 2;

        CreateWall(new Vector3(postX, postOffset, 0), new Vector2(0.3f, 0.3f), name + "_TopPost");
        CreateWall(new Vector3(postX, -postOffset, 0), new Vector2(0.3f, 0.3f), name + "_BottomPost");
        CreateWall(new Vector3(postX - (team == TeamType.Team1 ? goalDepth / 2 : -goalDepth / 2), 0, 0),
                   new Vector2(0.3f, goalWidth), name + "_BackPost");
    }

    private void CreateCenterLine()
    {
        GameObject centerLine = new GameObject("CenterLine");
        centerLine.transform.SetParent(transform);
        centerLine.transform.position = Vector3.zero;

        LineRenderer line = centerLine.AddComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(0, -fieldHeight / 2, 0));
        line.SetPosition(1, new Vector3(0, fieldHeight / 2, 0));
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
        line.sortingOrder = -1;
    }
}
