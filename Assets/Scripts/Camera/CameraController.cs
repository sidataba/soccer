using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Advanced camera system with multiple view modes
/// Supports: Top-Down, Isometric, Side View, Follow Ball, Split Screen
/// </summary>
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraMode currentMode = CameraMode.TopDown;
    [SerializeField] private float transitionSpeed = 5f;

    [Header("View Mode Settings")]
    [SerializeField] private float topDownHeight = 15f;
    [SerializeField] private float isometricHeight = 12f;
    [SerializeField] private float isometricAngle = 45f;
    [SerializeField] private float sideViewHeight = 8f;
    [SerializeField] private float followDistance = 10f;

    [Header("Follow Settings")]
    [SerializeField] private Transform ballTransform;
    [SerializeField] private float followSmoothness = 2f;
    [SerializeField] private Vector3 followOffset = new Vector3(0, 10, -5);

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSpeed = 2f;

    [Header("Pan Settings")]
    [SerializeField] private bool allowPanning = true;
    [SerializeField] private float panSpeed = 10f;
    [SerializeField] private Vector2 panLimits = new Vector2(15f, 10f);

    // Camera state
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float targetOrthographicSize;
    private Vector3 panOffset = Vector3.zero;

    // Input
    private Vector2 panInput;
    private float zoomInput;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
    }

    void Start()
    {
        // Find ball if not assigned
        if (ballTransform == null)
        {
            Ball ball = FindObjectOfType<Ball>();
            if (ball != null)
            {
                ballTransform = ball.transform;
            }
        }

        // Initialize camera to current mode
        SetCameraMode(currentMode, immediate: true);
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
        HandleZoom();
        HandlePanning();
    }

    #region Camera Modes

    public void SetCameraMode(CameraMode mode, bool immediate = false)
    {
        currentMode = mode;

        switch (mode)
        {
            case CameraMode.TopDown:
                SetTopDownView();
                break;
            case CameraMode.Isometric:
                SetIsometricView();
                break;
            case CameraMode.SideView:
                SetSideView();
                break;
            case CameraMode.FollowBall:
                SetFollowBallView();
                break;
            case CameraMode.Broadcast:
                SetBroadcastView();
                break;
            case CameraMode.ProStadium:
                SetProStadiumView();
                break;
        }

        if (immediate)
        {
            mainCamera.transform.position = targetPosition;
            mainCamera.transform.rotation = targetRotation;
            mainCamera.orthographicSize = targetOrthographicSize;
        }
    }

    private void SetTopDownView()
    {
        targetPosition = new Vector3(0, topDownHeight, 0) + panOffset;
        targetRotation = Quaternion.Euler(90f, 0f, 0f);
        targetOrthographicSize = topDownHeight * 0.6f;
    }

    private void SetIsometricView()
    {
        float angle = isometricAngle;
        float height = isometricHeight;
        float distance = height / Mathf.Tan(angle * Mathf.Deg2Rad);

        targetPosition = new Vector3(0, height, -distance) + panOffset;
        targetRotation = Quaternion.Euler(angle, 0f, 0f);
        targetOrthographicSize = height * 0.7f;
    }

    private void SetSideView()
    {
        targetPosition = new Vector3(0, sideViewHeight, -15) + panOffset;
        targetRotation = Quaternion.Euler(30f, 0f, 0f);
        targetOrthographicSize = sideViewHeight;
    }

    private void SetFollowBallView()
    {
        if (ballTransform != null)
        {
            targetPosition = ballTransform.position + followOffset;
            targetRotation = Quaternion.LookRotation(ballTransform.position - targetPosition, Vector3.up);
            targetOrthographicSize = followDistance * 0.5f;
        }
        else
        {
            SetTopDownView();
        }
    }

    private void SetBroadcastView()
    {
        // FIFA-style broadcast camera
        targetPosition = new Vector3(0, 10f, -12f) + panOffset;
        targetRotation = Quaternion.Euler(35f, 0f, 0f);
        targetOrthographicSize = 8f;
    }

    private void SetProStadiumView()
    {
        // Dynamic stadium camera that follows action
        if (ballTransform != null)
        {
            Vector3 ballPos = ballTransform.position;
            targetPosition = new Vector3(ballPos.x * 0.3f, 12f, -10f);
            targetRotation = Quaternion.Euler(40f, 0f, 0f);
            targetOrthographicSize = 9f;
        }
        else
        {
            SetBroadcastView();
        }
    }

    #endregion

    #region Camera Movement

    private void UpdateCameraPosition()
    {
        // Update target if following ball
        if (currentMode == CameraMode.FollowBall || currentMode == CameraMode.ProStadium)
        {
            SetCameraMode(currentMode);
        }

        // Smooth transition
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            Time.deltaTime * transitionSpeed
        );

        mainCamera.transform.rotation = Quaternion.Slerp(
            mainCamera.transform.rotation,
            targetRotation,
            Time.deltaTime * transitionSpeed
        );

        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetOrthographicSize,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    private void HandleZoom()
    {
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetOrthographicSize -= zoomInput * zoomSpeed * Time.deltaTime;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoom, maxZoom);
        }

        // Mouse wheel zoom
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetOrthographicSize -= scroll * 0.5f;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoom, maxZoom);
        }
    }

    private void HandlePanning()
    {
        if (allowPanning && panInput.magnitude > 0.01f)
        {
            Vector3 pan = new Vector3(panInput.x, 0f, panInput.y) * panSpeed * Time.deltaTime;

            panOffset += pan;
            panOffset.x = Mathf.Clamp(panOffset.x, -panLimits.x, panLimits.x);
            panOffset.z = Mathf.Clamp(panOffset.z, -panLimits.y, panLimits.y);

            SetCameraMode(currentMode);
        }
    }

    #endregion

    #region Input Handlers

    public void OnCameraSwitch(InputValue value)
    {
        if (value.isPressed)
        {
            CycleCamera();
        }
    }

    public void OnCameraPan(InputValue value)
    {
        panInput = value.Get<Vector2>();
    }

    public void OnCameraZoom(InputValue value)
    {
        zoomInput = value.Get<float>();
    }

    public void OnResetCamera(InputValue value)
    {
        if (value.isPressed)
        {
            ResetCamera();
        }
    }

    #endregion

    #region Public Methods

    public void CycleCamera()
    {
        int modeCount = System.Enum.GetValues(typeof(CameraMode)).Length;
        int nextMode = ((int)currentMode + 1) % modeCount;
        SetCameraMode((CameraMode)nextMode);

        // Play sound
        SoundManager.Instance?.PlayWhistle();

        Debug.Log($"Camera Mode: {currentMode}");
    }

    public void ResetCamera()
    {
        panOffset = Vector3.zero;
        SetCameraMode(currentMode);
    }

    public void SetCameraTarget(Transform target)
    {
        ballTransform = target;
    }

    public void EnablePanning(bool enable)
    {
        allowPanning = enable;
    }

    public CameraMode GetCurrentMode() => currentMode;

    #endregion

    #region Utility Methods

    public void ShakeCamera(float intensity, float duration)
    {
        StartCoroutine(CameraShakeCoroutine(intensity, duration));
    }

    private IEnumerator CameraShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            mainCamera.transform.position = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPosition;
    }

    public void ZoomToFitPlayers(Transform[] players, float padding = 2f)
    {
        if (players == null || players.Length == 0) return;

        // Calculate bounds
        Bounds bounds = new Bounds(players[0].position, Vector3.zero);
        foreach (Transform player in players)
        {
            if (player != null)
            {
                bounds.Encapsulate(player.position);
            }
        }

        // Set camera to fit bounds
        float maxExtent = Mathf.Max(bounds.size.x, bounds.size.z);
        targetOrthographicSize = maxExtent * 0.5f + padding;
        targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoom, maxZoom);

        targetPosition = new Vector3(bounds.center.x, targetPosition.y, bounds.center.z - 5f);
    }

    #endregion
}

public enum CameraMode
{
    TopDown,        // Classic overhead view
    Isometric,      // Angled top-down (FIFA classic)
    SideView,       // Side perspective
    FollowBall,     // Camera follows ball
    Broadcast,      // FIFA broadcast style
    ProStadium      // Dynamic professional camera
}
