using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Enhanced console and gamepad support
/// Supports: Xbox, PlayStation, Nintendo Switch controllers
/// Features: Vibration/Rumble, Console-specific button displays, Input detection
/// </summary>
public class ConsoleInputManager : MonoBehaviour
{
    public static ConsoleInputManager Instance { get; private set; }

    [Header("Vibration Settings")]
    [SerializeField] private bool vibrationEnabled = true;
    [SerializeField] [Range(0f, 1f)] private float vibrationIntensity = 0.7f;

    [Header("Console Detection")]
    [SerializeField] private ConsoleType currentConsole = ConsoleType.PC;

    // Connected gamepads
    private Dictionary<int, Gamepad> connectedGamepads = new Dictionary<int, Gamepad>();
    private Dictionary<int, ConsoleType> gamepadConsoleTypes = new Dictionary<int, ConsoleType>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DetectConnectedGamepads();
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    OnGamepadConnected((Gamepad)device);
                    break;
                case InputDeviceChange.Removed:
                    OnGamepadDisconnected((Gamepad)device);
                    break;
            }
        }
    }

    private void DetectConnectedGamepads()
    {
        int index = 0;
        foreach (var gamepad in Gamepad.all)
        {
            connectedGamepads[index] = gamepad;
            gamepadConsoleTypes[index] = DetectConsoleType(gamepad);
            Debug.Log($"Gamepad {index}: {gamepad.displayName} - Type: {gamepadConsoleTypes[index]}");
            index++;
        }

        if (connectedGamepads.Count == 0)
        {
            currentConsole = ConsoleType.PC;
        }
        else
        {
            currentConsole = gamepadConsoleTypes[0];
        }
    }

    private ConsoleType DetectConsoleType(Gamepad gamepad)
    {
        string deviceName = gamepad.displayName.ToLower();

        if (deviceName.Contains("xbox") || deviceName.Contains("xinput"))
        {
            return ConsoleType.Xbox;
        }
        else if (deviceName.Contains("playstation") || deviceName.Contains("dualshock") ||
                 deviceName.Contains("dualsense") || deviceName.Contains("ps4") || deviceName.Contains("ps5"))
        {
            return ConsoleType.PlayStation;
        }
        else if (deviceName.Contains("switch") || deviceName.Contains("joy-con") || deviceName.Contains("pro controller"))
        {
            return ConsoleType.Switch;
        }

        return ConsoleType.Generic;
    }

    private void OnGamepadConnected(Gamepad gamepad)
    {
        int index = connectedGamepads.Count;
        connectedGamepads[index] = gamepad;
        gamepadConsoleTypes[index] = DetectConsoleType(gamepad);

        Debug.Log($"Gamepad connected: {gamepad.displayName} ({gamepadConsoleTypes[index]})");

        // Update current console if first gamepad
        if (index == 0)
        {
            currentConsole = gamepadConsoleTypes[0];
        }

        // Welcome rumble
        TriggerVibration(0.3f, 0.3f, 0.2f, index);
    }

    private void OnGamepadDisconnected(Gamepad gamepad)
    {
        Debug.Log($"Gamepad disconnected: {gamepad.displayName}");

        // Remove from dictionaries
        int removedIndex = -1;
        foreach (var kvp in connectedGamepads)
        {
            if (kvp.Value == gamepad)
            {
                removedIndex = kvp.Key;
                break;
            }
        }

        if (removedIndex >= 0)
        {
            connectedGamepads.Remove(removedIndex);
            gamepadConsoleTypes.Remove(removedIndex);
        }
    }

    #region Vibration/Rumble

    /// <summary>
    /// Trigger controller vibration
    /// </summary>
    public void TriggerVibration(float lowFrequency, float highFrequency, float duration, int gamepadIndex = 0)
    {
        if (!vibrationEnabled) return;

        if (connectedGamepads.TryGetValue(gamepadIndex, out Gamepad gamepad))
        {
            gamepad.SetMotorSpeeds(
                lowFrequency * vibrationIntensity,
                highFrequency * vibrationIntensity
            );

            StartCoroutine(StopVibrationAfterDelay(duration, gamepadIndex));
        }
    }

    /// <summary>
    /// Vibration presets for common game events
    /// </summary>
    public void VibrationKick(int gamepadIndex = 0)
    {
        TriggerVibration(0.4f, 0.2f, 0.1f, gamepadIndex);
    }

    public void VibrationGoal(int gamepadIndex = 0)
    {
        TriggerVibration(0.8f, 0.8f, 0.5f, gamepadIndex);
    }

    public void VibrationCollision(int gamepadIndex = 0)
    {
        TriggerVibration(0.3f, 0.6f, 0.15f, gamepadIndex);
    }

    public void VibrationSkill(int gamepadIndex = 0)
    {
        TriggerVibration(0.5f, 0.5f, 0.2f, gamepadIndex);
    }

    private System.Collections.IEnumerator StopVibrationAfterDelay(float delay, int gamepadIndex)
    {
        yield return new WaitForSeconds(delay);

        if (connectedGamepads.TryGetValue(gamepadIndex, out Gamepad gamepad))
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

    public void StopVibration(int gamepadIndex = 0)
    {
        if (connectedGamepads.TryGetValue(gamepadIndex, out Gamepad gamepad))
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

    public void StopAllVibrations()
    {
        foreach (var gamepad in connectedGamepads.Values)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

    #endregion

    #region Button Display Helpers

    /// <summary>
    /// Get button name for current console (e.g., "A" for Xbox, "Cross" for PlayStation)
    /// </summary>
    public string GetButtonName(GamepadButton button)
    {
        switch (currentConsole)
        {
            case ConsoleType.Xbox:
                return GetXboxButtonName(button);
            case ConsoleType.PlayStation:
                return GetPlayStationButtonName(button);
            case ConsoleType.Switch:
                return GetSwitchButtonName(button);
            default:
                return GetGenericButtonName(button);
        }
    }

    private string GetXboxButtonName(GamepadButton button)
    {
        switch (button)
        {
            case GamepadButton.South: return "A";
            case GamepadButton.East: return "B";
            case GamepadButton.West: return "X";
            case GamepadButton.North: return "Y";
            case GamepadButton.LeftShoulder: return "LB";
            case GamepadButton.RightShoulder: return "RB";
            case GamepadButton.LeftTrigger: return "LT";
            case GamepadButton.RightTrigger: return "RT";
            case GamepadButton.Start: return "Menu";
            case GamepadButton.Select: return "View";
            default: return button.ToString();
        }
    }

    private string GetPlayStationButtonName(GamepadButton button)
    {
        switch (button)
        {
            case GamepadButton.South: return "Cross (✕)";
            case GamepadButton.East: return "Circle (○)";
            case GamepadButton.West: return "Square (□)";
            case GamepadButton.North: return "Triangle (△)";
            case GamepadButton.LeftShoulder: return "L1";
            case GamepadButton.RightShoulder: return "R1";
            case GamepadButton.LeftTrigger: return "L2";
            case GamepadButton.RightTrigger: return "R2";
            case GamepadButton.Start: return "Options";
            case GamepadButton.Select: return "Share/Create";
            default: return button.ToString();
        }
    }

    private string GetSwitchButtonName(GamepadButton button)
    {
        switch (button)
        {
            case GamepadButton.South: return "B";
            case GamepadButton.East: return "A";
            case GamepadButton.West: return "Y";
            case GamepadButton.North: return "X";
            case GamepadButton.LeftShoulder: return "L";
            case GamepadButton.RightShoulder: return "R";
            case GamepadButton.LeftTrigger: return "ZL";
            case GamepadButton.RightTrigger: return "ZR";
            case GamepadButton.Start: return "+";
            case GamepadButton.Select: return "-";
            default: return button.ToString();
        }
    }

    private string GetGenericButtonName(GamepadButton button)
    {
        switch (button)
        {
            case GamepadButton.South: return "Button 1";
            case GamepadButton.East: return "Button 2";
            case GamepadButton.West: return "Button 3";
            case GamepadButton.North: return "Button 4";
            default: return button.ToString();
        }
    }

    #endregion

    #region Getters & Setters

    public ConsoleType GetCurrentConsole() => currentConsole;

    public bool IsGamepadConnected(int index = 0)
    {
        return connectedGamepads.ContainsKey(index);
    }

    public int GetConnectedGamepadCount()
    {
        return connectedGamepads.Count;
    }

    public void SetVibrationEnabled(bool enabled)
    {
        vibrationEnabled = enabled;
        if (!enabled)
        {
            StopAllVibrations();
        }
    }

    public void SetVibrationIntensity(float intensity)
    {
        vibrationIntensity = Mathf.Clamp01(intensity);
    }

    public bool IsVibrationEnabled() => vibrationEnabled;

    #endregion
}

public enum ConsoleType
{
    PC,
    Xbox,
    PlayStation,
    Switch,
    Generic
}

public enum GamepadButton
{
    South,      // A (Xbox), Cross (PS), B (Switch)
    East,       // B (Xbox), Circle (PS), A (Switch)
    West,       // X (Xbox), Square (PS), Y (Switch)
    North,      // Y (Xbox), Triangle (PS), X (Switch)
    LeftShoulder,
    RightShoulder,
    LeftTrigger,
    RightTrigger,
    Start,
    Select,
    LeftStick,
    RightStick,
    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight
}
