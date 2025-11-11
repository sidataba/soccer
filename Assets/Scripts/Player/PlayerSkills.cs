using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Player skills and special abilities system
/// Includes: Speed Boost, Power Shot, Curve Shot, Slide Tackle, Skill Move, Shield Ball
/// </summary>
public class PlayerSkills : MonoBehaviour
{
    [Header("Skill Cooldowns")]
    [SerializeField] private float speedBoostCooldown = 5f;
    [SerializeField] private float powerShotCooldown = 8f;
    [SerializeField] private float skillMoveCooldown = 3f;
    [SerializeField] private float slideTackleCooldown = 6f;
    [SerializeField] private float shieldBallCooldown = 10f;

    [Header("Skill Durations")]
    [SerializeField] private float speedBoostDuration = 3f;
    [SerializeField] private float shieldBallDuration = 4f;

    [Header("Skill Parameters")]
    [SerializeField] private float speedBoostMultiplier = 1.8f;
    [SerializeField] private float powerShotMultiplier = 2.0f;
    [SerializeField] private float curveAmount = 3f;
    [SerializeField] private float skillMoveDistance = 2f;
    [SerializeField] private float slideTackleRange = 2f;

    // Components
    private PlayerController playerController;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Skill states
    private float speedBoostTimer = 0f;
    private float powerShotTimer = 0f;
    private float skillMoveTimer = 0f;
    private float slideTackleTimer = 0f;
    private float shieldBallTimer = 0f;

    private bool isSpeedBoosting = false;
    private bool isShieldingBall = false;

    // Visual effects
    private GameObject trailEffect;
    private Color originalColor;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        UpdateCooldowns();
    }

    private void UpdateCooldowns()
    {
        speedBoostTimer -= Time.deltaTime;
        powerShotTimer -= Time.deltaTime;
        skillMoveTimer -= Time.deltaTime;
        slideTackleTimer -= Time.deltaTime;
        shieldBallTimer -= Time.deltaTime;
    }

    #region Skill Activation Methods

    /// <summary>
    /// Speed Boost - Temporary speed increase
    /// </summary>
    public void ActivateSpeedBoost()
    {
        if (speedBoostTimer <= 0 && !isSpeedBoosting)
        {
            StartCoroutine(SpeedBoostCoroutine());
            speedBoostTimer = speedBoostCooldown;

            // Play sound and visual effect
            SoundManager.Instance?.PlaySkillSound(SkillType.SpeedBoost);
        }
    }

    /// <summary>
    /// Power Shot - Extra powerful kick
    /// </summary>
    public void ActivatePowerShot()
    {
        if (powerShotTimer <= 0 && playerController.HasBall())
        {
            // Enhanced shoot with power multiplier
            StartCoroutine(PowerShotCoroutine());
            powerShotTimer = powerShotCooldown;

            SoundManager.Instance?.PlaySkillSound(SkillType.PowerShot);
        }
    }

    /// <summary>
    /// Curve Shot - Ball curves in flight
    /// </summary>
    public void ActivateCurveShot()
    {
        if (playerController.HasBall())
        {
            StartCoroutine(CurveShotCoroutine());

            SoundManager.Instance?.PlaySkillSound(SkillType.CurveShot);
        }
    }

    /// <summary>
    /// Skill Move - Quick dodge/dribble
    /// </summary>
    public void ActivateSkillMove()
    {
        if (skillMoveTimer <= 0)
        {
            StartCoroutine(SkillMoveCoroutine());
            skillMoveTimer = skillMoveCooldown;

            SoundManager.Instance?.PlaySkillSound(SkillType.SkillMove);
        }
    }

    /// <summary>
    /// Slide Tackle - Defensive move to steal ball
    /// </summary>
    public void ActivateSlideTackle()
    {
        if (slideTackleTimer <= 0)
        {
            StartCoroutine(SlideTackleCoroutine());
            slideTackleTimer = slideTackleCooldown;

            SoundManager.Instance?.PlaySkillSound(SkillType.SlideTackle);
        }
    }

    /// <summary>
    /// Shield Ball - Protect possession from opponents
    /// </summary>
    public void ActivateShieldBall()
    {
        if (shieldBallTimer <= 0 && playerController.HasBall() && !isShieldingBall)
        {
            StartCoroutine(ShieldBallCoroutine());
            shieldBallTimer = shieldBallCooldown;

            SoundManager.Instance?.PlaySkillSound(SkillType.ShieldBall);
        }
    }

    #endregion

    #region Skill Coroutines

    private IEnumerator SpeedBoostCoroutine()
    {
        isSpeedBoosting = true;

        // Visual feedback - glow effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }

        // Create trail effect
        CreateTrailEffect();

        yield return new WaitForSeconds(speedBoostDuration);

        // Restore normal state
        isSpeedBoosting = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        DestroyTrailEffect();
    }

    private IEnumerator PowerShotCoroutine()
    {
        // Visual charge-up effect
        if (spriteRenderer != null)
        {
            Color chargeColor = Color.red;
            spriteRenderer.color = chargeColor;
        }

        yield return new WaitForSeconds(0.2f);

        // Execute power shot
        Vector2 shootDirection = transform.up;
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            ball.Kick(shootDirection, 15f * powerShotMultiplier);
        }

        // Restore color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator CurveShotCoroutine()
    {
        Vector2 shootDirection = transform.up;
        Ball ball = FindObjectOfType<Ball>();

        if (ball != null)
        {
            // Initial kick
            ball.Kick(shootDirection, 12f);

            // Apply curve over time
            float curveTime = 1.5f;
            float elapsed = 0f;

            while (elapsed < curveTime)
            {
                if (ball != null)
                {
                    Vector2 perpendicular = new Vector2(-shootDirection.y, shootDirection.x);
                    Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
                    if (ballRb != null)
                    {
                        ballRb.AddForce(perpendicular * curveAmount * Time.deltaTime, ForceMode2D.Force);
                    }
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator SkillMoveCoroutine()
    {
        // Quick dash in movement direction
        Vector2 dashDirection = rb.velocity.normalized;
        if (dashDirection.magnitude < 0.1f)
        {
            dashDirection = transform.up;
        }

        // Flash effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
        }

        // Execute dash
        rb.velocity = dashDirection * skillMoveDistance * 5f;

        yield return new WaitForSeconds(0.3f);

        // Restore color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator SlideTackleCoroutine()
    {
        // Slide in current direction
        Vector2 slideDirection = transform.up;

        // Visual - change color during slide
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.5f, 0f); // Orange
        }

        // Execute slide
        rb.velocity = slideDirection * 8f;

        // Check for ball in range
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            float distance = Vector2.Distance(transform.position, ball.transform.position);
            if (distance < slideTackleRange)
            {
                // Steal the ball
                ball.SetController(playerController);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Restore
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ShieldBallCoroutine()
    {
        isShieldingBall = true;

        // Visual - protective shield color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
        }

        // Create shield visual
        GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shield.transform.SetParent(transform);
        shield.transform.localPosition = Vector3.zero;
        shield.transform.localScale = Vector3.one * 1.5f;
        shield.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
        Destroy(shield.GetComponent<Collider>());

        yield return new WaitForSeconds(shieldBallDuration);

        // Remove shield
        isShieldingBall = false;
        if (shield != null)
        {
            Destroy(shield);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    #endregion

    #region Visual Effects

    private void CreateTrailEffect()
    {
        if (trailEffect == null)
        {
            trailEffect = new GameObject("SpeedTrail");
            trailEffect.transform.SetParent(transform);
            trailEffect.transform.localPosition = Vector3.zero;

            TrailRenderer trail = trailEffect.AddComponent<TrailRenderer>();
            trail.time = 0.5f;
            trail.startWidth = 0.3f;
            trail.endWidth = 0.05f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = Color.yellow;
            trail.endColor = new Color(1, 1, 0, 0);
        }
    }

    private void DestroyTrailEffect()
    {
        if (trailEffect != null)
        {
            Destroy(trailEffect, 1f);
            trailEffect = null;
        }
    }

    #endregion

    #region Input Handlers (Called by PlayerController or Input System)

    public void OnSpeedBoost(InputValue value)
    {
        if (value.isPressed)
        {
            ActivateSpeedBoost();
        }
    }

    public void OnPowerShot(InputValue value)
    {
        if (value.isPressed)
        {
            ActivatePowerShot();
        }
    }

    public void OnSkillMove(InputValue value)
    {
        if (value.isPressed)
        {
            ActivateSkillMove();
        }
    }

    public void OnSlideTackle(InputValue value)
    {
        if (value.isPressed)
        {
            ActivateSlideTackle();
        }
    }

    #endregion

    #region Getters

    public bool IsSpeedBoosting() => isSpeedBoosting;
    public bool IsShieldingBall() => isShieldingBall;
    public float GetSpeedBoostMultiplier() => isSpeedBoosting ? speedBoostMultiplier : 1f;

    public bool IsSkillReady(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.SpeedBoost: return speedBoostTimer <= 0;
            case SkillType.PowerShot: return powerShotTimer <= 0;
            case SkillType.SkillMove: return skillMoveTimer <= 0;
            case SkillType.SlideTackle: return slideTackleTimer <= 0;
            case SkillType.ShieldBall: return shieldBallTimer <= 0;
            default: return false;
        }
    }

    public float GetSkillCooldownPercent(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.SpeedBoost:
                return Mathf.Clamp01(1f - (speedBoostTimer / speedBoostCooldown));
            case SkillType.PowerShot:
                return Mathf.Clamp01(1f - (powerShotTimer / powerShotCooldown));
            case SkillType.SkillMove:
                return Mathf.Clamp01(1f - (skillMoveTimer / skillMoveCooldown));
            case SkillType.SlideTackle:
                return Mathf.Clamp01(1f - (slideTackleTimer / slideTackleCooldown));
            case SkillType.ShieldBall:
                return Mathf.Clamp01(1f - (shieldBallTimer / shieldBallCooldown));
            default: return 1f;
        }
    }

    #endregion
}

public enum SkillType
{
    SpeedBoost,
    PowerShot,
    CurveShot,
    SkillMove,
    SlideTackle,
    ShieldBall
}
