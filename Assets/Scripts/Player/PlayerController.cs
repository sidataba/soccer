using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement, shooting, and passing
/// Supports local multiplayer with different input devices
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Ball Control")]
    [SerializeField] private float shootPower = 15f;
    [SerializeField] private float passPower = 8f;
    [SerializeField] private float ballControlRadius = 1.5f;
    [SerializeField] private Transform kickPoint;

    [Header("Player Info")]
    [SerializeField] private int playerNumber;
    [SerializeField] private TeamType team;
    [SerializeField] private Color playerColor = Color.white;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Ball currentBall;
    private PlayerAnimator playerAnimator;

    // Input
    private Vector2 moveInput;
    private bool isSprinting;
    private bool hasBall;

    // State
    private float lastKickTime;
    private const float kickCooldown = 0.3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<PlayerAnimator>();

        if (kickPoint == null)
        {
            GameObject kp = new GameObject("KickPoint");
            kp.transform.SetParent(transform);
            kp.transform.localPosition = new Vector3(0, 0.5f, 0);
            kickPoint = kp.transform;
        }

        // Set player color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = playerColor;
        }
    }

    void Update()
    {
        CheckBallProximity();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Calculate movement
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector2 movement = moveInput.normalized * currentSpeed;
            rb.velocity = movement;

            // Rotate towards movement direction
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.95f);
        }
    }

    private void CheckBallProximity()
    {
        if (currentBall == null)
        {
            currentBall = FindObjectOfType<Ball>();
            if (currentBall == null) return;
        }

        float distance = Vector2.Distance(transform.position, currentBall.transform.position);
        hasBall = distance < ballControlRadius && currentBall.CanBeControlled();

        if (hasBall)
        {
            currentBall.SetController(this);
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnShoot(InputValue value)
    {
        if (value.isPressed && hasBall && Time.time > lastKickTime + kickCooldown)
        {
            Shoot();
        }
    }

    public void OnPass(InputValue value)
    {
        if (value.isPressed && hasBall && Time.time > lastKickTime + kickCooldown)
        {
            Pass();
        }
    }

    private void Shoot()
    {
        if (currentBall != null)
        {
            Vector2 shootDirection = (kickPoint.position - currentBall.transform.position).normalized;
            currentBall.Kick(shootDirection, shootPower);
            lastKickTime = Time.time;
            hasBall = false;

            // Play kick animation
            if (playerAnimator != null)
            {
                playerAnimator.PlayKickAnimation();
            }

            // Play kick sound
            SoundManager.Instance?.PlayKick();
        }
    }

    private void Pass()
    {
        if (currentBall != null)
        {
            // Find nearest teammate
            PlayerController nearestTeammate = FindNearestTeammate();

            Vector2 passDirection;
            if (nearestTeammate != null)
            {
                passDirection = (nearestTeammate.transform.position - transform.position).normalized;
            }
            else
            {
                passDirection = (kickPoint.position - currentBall.transform.position).normalized;
            }

            currentBall.Kick(passDirection, passPower);
            lastKickTime = Time.time;
            hasBall = false;

            // Play kick animation
            if (playerAnimator != null)
            {
                playerAnimator.PlayKickAnimation();
            }

            // Play pass sound
            SoundManager.Instance?.PlayPass();
        }
    }

    private PlayerController FindNearestTeammate()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        PlayerController nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (PlayerController player in allPlayers)
        {
            if (player != this && player.team == this.team)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = player;
                }
            }
        }

        return nearest;
    }

    public bool HasBall() => hasBall;
    public TeamType GetTeam() => team;
    public int GetPlayerNumber() => playerNumber;
    public bool IsMoving() => rb != null && rb.velocity.magnitude > 0.1f;

    public void SetTeam(TeamType newTeam)
    {
        team = newTeam;
    }

    public void SetPlayerColor(Color color)
    {
        playerColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTeamColor(color);
        }
    }

    // AI Control Methods
    public void AIShoot(Vector2 direction)
    {
        if (currentBall != null && hasBall && Time.time > lastKickTime + kickCooldown)
        {
            currentBall.Kick(direction, shootPower);
            lastKickTime = Time.time;
            hasBall = false;

            if (playerAnimator != null)
            {
                playerAnimator.PlayKickAnimation();
            }

            SoundManager.Instance?.PlayKick();
        }
    }

    public void AIPass(Vector2 direction)
    {
        if (currentBall != null && hasBall && Time.time > lastKickTime + kickCooldown)
        {
            currentBall.Kick(direction, passPower);
            lastKickTime = Time.time;
            hasBall = false;

            if (playerAnimator != null)
            {
                playerAnimator.PlayKickAnimation();
            }

            SoundManager.Instance?.PlayPass();
        }
    }
}

public enum TeamType
{
    Team1,
    Team2
}
