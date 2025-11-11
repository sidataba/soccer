using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI controls for camera system
/// Shows current camera mode and allows switching
/// </summary>
public class CameraControlUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject cameraPanel;
    [SerializeField] private TextMeshProUGUI cameraModeTex;
    [SerializeField] private Button previousCameraButton;
    [SerializeField] private Button nextCameraButton;
    [SerializeField] private Button resetCameraButton;
    [SerializeField] private Toggle panningToggle;

    [Header("Camera Mode Descriptions")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Settings")]
    [SerializeField] private bool showPanel = true;
    [SerializeField] private float autoHideDelay = 3f;

    private CameraController cameraController;
    private float lastInteractionTime;

    void Start()
    {
        cameraController = CameraController.Instance;
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }

        SetupUI();
        UpdateCameraModeDisplay();
    }

    void Update()
    {
        // Auto-hide panel
        if (showPanel && autoHideDelay > 0)
        {
            if (Time.time - lastInteractionTime > autoHideDelay)
            {
                if (cameraPanel != null)
                {
                    cameraPanel.SetActive(false);
                }
            }
        }

        // Show panel on input
        if (Input.anyKeyDown && cameraPanel != null)
        {
            cameraPanel.SetActive(true);
            lastInteractionTime = Time.time;
        }
    }

    void SetupUI()
    {
        // Setup buttons
        if (previousCameraButton != null)
        {
            previousCameraButton.onClick.AddListener(PreviousCamera);
        }

        if (nextCameraButton != null)
        {
            nextCameraButton.onClick.AddListener(NextCamera);
        }

        if (resetCameraButton != null)
        {
            resetCameraButton.onClick.AddListener(ResetCamera);
        }

        if (panningToggle != null)
        {
            panningToggle.onValueChanged.AddListener(TogglePanning);
            panningToggle.isOn = true;
        }

        lastInteractionTime = Time.time;
    }

    public void NextCamera()
    {
        if (cameraController != null)
        {
            cameraController.CycleCamera();
            UpdateCameraModeDisplay();
            lastInteractionTime = Time.time;
        }
    }

    public void PreviousCamera()
    {
        if (cameraController == null) return;

        CameraMode currentMode = cameraController.GetCurrentMode();
        int modeCount = System.Enum.GetValues(typeof(CameraMode)).Length;
        int previousMode = ((int)currentMode - 1 + modeCount) % modeCount;

        cameraController.SetCameraMode((CameraMode)previousMode);
        UpdateCameraModeDisplay();
        lastInteractionTime = Time.time;
    }

    public void ResetCamera()
    {
        if (cameraController != null)
        {
            cameraController.ResetCamera();
            lastInteractionTime = Time.time;
        }
    }

    public void TogglePanning(bool enabled)
    {
        if (cameraController != null)
        {
            cameraController.EnablePanning(enabled);
        }
    }

    void UpdateCameraModeDisplay()
    {
        if (cameraController == null) return;

        CameraMode mode = cameraController.GetCurrentMode();

        // Update mode text
        if (cameraModeText != null)
        {
            cameraModeText.text = $"Camera: {GetCameraModeName(mode)}";
        }

        // Update description
        if (descriptionText != null)
        {
            descriptionText.text = GetCameraModeDescription(mode);
        }
    }

    string GetCameraModeName(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.TopDown: return "Top-Down";
            case CameraMode.Isometric: return "Isometric";
            case CameraMode.SideView: return "Side View";
            case CameraMode.FollowBall: return "Follow Ball";
            case CameraMode.Broadcast: return "Broadcast";
            case CameraMode.ProStadium: return "Pro Stadium";
            default: return mode.ToString();
        }
    }

    string GetCameraModeDescription(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.TopDown:
                return "Classic overhead view - Best for strategic play";
            case CameraMode.Isometric:
                return "Angled view - FIFA classic style";
            case CameraMode.SideView:
                return "Side perspective - Retro arcade feel";
            case CameraMode.FollowBall:
                return "Dynamic camera following the ball";
            case CameraMode.Broadcast:
                return "FIFA broadcast style - TV view";
            case CameraMode.ProStadium:
                return "Professional stadium camera with dynamic tracking";
            default:
                return "";
        }
    }

    public void ToggleCameraPanel()
    {
        if (cameraPanel != null)
        {
            cameraPanel.SetActive(!cameraPanel.activeSelf);
            lastInteractionTime = Time.time;
        }
    }

    public void SetCameraMode(int modeIndex)
    {
        if (cameraController != null && modeIndex >= 0)
        {
            int modeCount = System.Enum.GetValues(typeof(CameraMode)).Length;
            if (modeIndex < modeCount)
            {
                cameraController.SetCameraMode((CameraMode)modeIndex);
                UpdateCameraModeDisplay();
                lastInteractionTime = Time.time;
            }
        }
    }
}
