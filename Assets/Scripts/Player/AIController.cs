using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// AI Controller for computer-controlled players
/// Simple but effective AI that can play against human players
/// </summary>
public class AIController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float reactionTime = 0.3f;
    [SerializeField] private float decisionInterval = 0.5f;
    [SerializeField] private float shootDistance = 8f;
    [SerializeField] private float passDistance = 5f;

    [Header("AI Difficulty")]
    [SerializeField] private AIDifficulty difficulty = AIDifficulty.Medium;
    [SerializeField] [Range(0f, 1f)] private float accuracyMultiplier = 0.8f;

    // Components
    private Rigidbody2D rb;
    private PlayerController playerController;
    private Ball ball;
    private Transform goalTarget;

    // AI State
    private Vector2 targetPosition;
    private float decisionTimer;
    private AIState currentState = AIState.Chase;
    private float reactionTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        // Disable PlayerInput component if exists (AI doesn't use input)
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
        }

        SetDifficulty(difficulty);
    }

    void Start()
    {
        ball = FindObjectOfType<Ball>();
        FindGoalTarget();
    }

    void Update()
    {
        if (ball == null) return;

        reactionTimer -= Time.deltaTime;
        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0)
        {
            MakeDecision();
            decisionTimer = decisionInterval;
        }

        ExecuteCurrentState();
    }

    void FixedUpdate()
    {
        if (targetPosition != Vector2.zero)
        {
            MoveTowardsTarget();
        }
    }

    private void MakeDecision()
    {
        if (ball == null) return;

        float distanceToBall = Vector2.Distance(transform.position, ball.transform.position);
        float distanceToGoal = goalTarget != null ? Vector2.Distance(transform.position, goalTarget.position) : 100f;

        // Decision tree
        if (distanceToBall < 1.5f && playerController.HasBall())
        {
            // Have the ball - decide to shoot or pass
            if (distanceToGoal < shootDistance)
            {
                currentState = AIState.Shoot;
            }
            else
            {
                // Look for teammate to pass
                if (Random.value > 0.7f) // 30% chance to pass
                {
                    currentState = AIState.Pass;
                }
                else
                {
                    currentState = AIState.Dribble;
                }
            }
        }
        else if (distanceToBall < 10f)
        {
            // Ball nearby - chase it
            currentState = AIState.Chase;
        }
        else
        {
            // Ball far away - go to defensive position
            currentState = AIState.Position;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.Chase:
                ChaseBall();
                break;
            case AIState.Shoot:
                Shoot();
                break;
            case AIState.Pass:
                Pass();
                break;
            case AIState.Dribble:
                DribbleToGoal();
                break;
            case AIState.Position:
                MoveToDefensivePosition();
                break;
        }
    }

    private void ChaseBall()
    {
        if (ball != null)
        {
            targetPosition = ball.transform.position;
        }
    }

    private void Shoot()
    {
        if (reactionTimer > 0) return;

        if (goalTarget != null && playerController.HasBall())
        {
            // Add some inaccuracy based on difficulty
            Vector2 goalDirection = (goalTarget.position - transform.position).normalized;
            float inaccuracy = (1f - accuracyMultiplier) * 2f;
            goalDirection += new Vector2(
                Random.Range(-inaccuracy, inaccuracy),
                Random.Range(-inaccuracy, inaccuracy)
            );

            // Simulate shoot action
            SimulateShoot(goalDirection.normalized);

            reactionTimer = reactionTime;
            currentState = AIState.Chase;
        }
    }

    private void Pass()
    {
        if (reactionTimer > 0) return;

        PlayerController nearestTeammate = FindNearestTeammate();
        if (nearestTeammate != null)
        {
            Vector2 passDirection = (nearestTeammate.transform.position - transform.position).normalized;
            SimulatePass(passDirection);
            reactionTimer = reactionTime;
        }
        currentState = AIState.Chase;
    }

    private void DribbleToGoal()
    {
        if (goalTarget != null)
        {
            targetPosition = Vector2.Lerp(transform.position, goalTarget.position, 0.3f);
        }
    }

    private void MoveToDefensivePosition()
    {
        // Simple defensive positioning - stay between ball and own goal
        if (goalTarget != null && ball != null)
        {
            Vector2 ownGoalPos = -goalTarget.position; // Opposite side
            targetPosition = Vector2.Lerp(ownGoalPos, ball.transform.position, 0.3f);
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Rotate towards movement
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void FindGoalTarget()
    {
        Goal[] goals = FindObjectsOfType<Goal>();
        foreach (Goal goal in goals)
        {
            // Find opponent's goal (where we want to score)
            if (playerController != null)
            {
                goalTarget = goal.transform;
                break;
            }
        }

        if (goalTarget == null)
        {
            // Default to right side
            GameObject target = new GameObject("AIGoalTarget");
            target.transform.position = new Vector3(10, 0, 0);
            goalTarget = target.transform;
        }
    }

    private PlayerController FindNearestTeammate()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        PlayerController nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (PlayerController player in allPlayers)
        {
            if (player == playerController) continue;
            if (player.GetTeam() != playerController.GetTeam()) continue;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < nearestDistance && distance < passDistance)
            {
                nearestDistance = distance;
                nearest = player;
            }
        }

        return nearest;
    }

    private void SimulateShoot(Vector2 direction)
    {
        // Call the player controller's shooting method
        if (playerController != null)
        {
            playerController.AIShoot(direction);
        }
    }

    private void SimulatePass(Vector2 direction)
    {
        // Call the player controller's passing method
        if (playerController != null)
        {
            playerController.AIPass(direction);
        }
    }

    public void SetDifficulty(AIDifficulty newDifficulty)
    {
        difficulty = newDifficulty;

        switch (difficulty)
        {
            case AIDifficulty.Easy:
                moveSpeed = 3f;
                reactionTime = 0.5f;
                decisionInterval = 0.8f;
                accuracyMultiplier = 0.5f;
                break;
            case AIDifficulty.Medium:
                moveSpeed = 4f;
                reactionTime = 0.3f;
                decisionInterval = 0.5f;
                accuracyMultiplier = 0.75f;
                break;
            case AIDifficulty.Hard:
                moveSpeed = 5f;
                reactionTime = 0.2f;
                decisionInterval = 0.3f;
                accuracyMultiplier = 0.9f;
                break;
        }
    }
}

public enum AIState
{
    Chase,
    Shoot,
    Pass,
    Dribble,
    Position
}

public enum AIDifficulty
{
    Easy,
    Medium,
    Hard
}
