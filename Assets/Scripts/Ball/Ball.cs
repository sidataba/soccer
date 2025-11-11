using UnityEngine;

/// <summary>
/// Controls ball physics, bouncing, and interaction with players
/// </summary>
public class Ball : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float friction = 0.98f;
    [SerializeField] private float minVelocity = 0.1f;
    [SerializeField] private float bounceMultiplier = 0.7f;

    [Header("Control Settings")]
    [SerializeField] private float controlCooldown = 0.2f;

    private Rigidbody2D rb;
    private PlayerController currentController;
    private float lastKickTime;
    private Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    void Start()
    {
        ResetBall();
    }

    void FixedUpdate()
    {
        ApplyFriction();
    }

    private void ApplyFriction()
    {
        if (rb.velocity.magnitude < minVelocity)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity *= friction;
        }
    }

    public void Kick(Vector2 direction, float power)
    {
        rb.velocity = direction.normalized * power;
        lastKickTime = Time.time;
        currentController = null;
    }

    public bool CanBeControlled()
    {
        return Time.time > lastKickTime + controlCooldown;
    }

    public void SetController(PlayerController player)
    {
        if (CanBeControlled())
        {
            currentController = player;
        }
    }

    public PlayerController GetController()
    {
        return currentController;
    }

    public void ResetBall()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        currentController = null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Bounce off walls
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 velocity = rb.velocity;
            velocity *= bounceMultiplier;
            rb.velocity = velocity;
        }
    }
}
