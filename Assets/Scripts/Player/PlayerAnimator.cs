using UnityEngine;

/// <summary>
/// Handles simple sprite-based animations for player
/// Supports: Idle, Run, Kick animations
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [Header("Animation Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] runSprites = new Sprite[2];  // 2-frame run cycle
    [SerializeField] private Sprite kickSprite;

    [Header("Animation Settings")]
    [SerializeField] private float runAnimationSpeed = 0.15f;
    [SerializeField] private float kickAnimationDuration = 0.2f;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    // Animation state
    private AnimationState currentState = AnimationState.Idle;
    private float animationTimer = 0f;
    private int currentFrame = 0;
    private bool isKicking = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();

        // Create default sprites if not assigned
        if (idleSprite == null)
        {
            idleSprite = CreateCircleSprite(Color.white);
        }
        if (runSprites.Length == 0 || runSprites[0] == null)
        {
            runSprites = new Sprite[2];
            runSprites[0] = CreateCircleSprite(Color.white);
            runSprites[1] = CreateCircleSprite(Color.gray);
        }
        if (kickSprite == null)
        {
            kickSprite = CreateCircleSprite(Color.yellow);
        }
    }

    void Update()
    {
        if (isKicking)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= kickAnimationDuration)
            {
                isKicking = false;
                animationTimer = 0f;
            }
            return;
        }

        // Determine animation state based on movement
        if (playerController != null)
        {
            bool isMoving = playerController.IsMoving();

            if (isMoving)
            {
                PlayRunAnimation();
            }
            else
            {
                PlayIdleAnimation();
            }
        }
    }

    private void PlayIdleAnimation()
    {
        if (currentState != AnimationState.Idle)
        {
            currentState = AnimationState.Idle;
            spriteRenderer.sprite = idleSprite;
        }
    }

    private void PlayRunAnimation()
    {
        if (currentState != AnimationState.Run)
        {
            currentState = AnimationState.Run;
            currentFrame = 0;
            animationTimer = 0f;
        }

        animationTimer += Time.deltaTime;
        if (animationTimer >= runAnimationSpeed)
        {
            currentFrame = (currentFrame + 1) % runSprites.Length;
            spriteRenderer.sprite = runSprites[currentFrame];
            animationTimer = 0f;
        }
    }

    public void PlayKickAnimation()
    {
        isKicking = true;
        animationTimer = 0f;
        spriteRenderer.sprite = kickSprite;
    }

    // Helper method to create simple sprite
    private Sprite CreateCircleSprite(Color color)
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2, size / 2));
                if (distance < size / 2)
                {
                    texture.SetPixel(x, y, color);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    public void SetTeamColor(Color color)
    {
        spriteRenderer.color = color;
    }
}

public enum AnimationState
{
    Idle,
    Run,
    Kick
}
