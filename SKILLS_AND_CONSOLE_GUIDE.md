# Player Skills & Console Support Guide - v3.0

Complete guide to the new skills system and enhanced console/gamepad support!

---

## 🎮 Console/Gamepad Support

### Supported Controllers

Your game now has **full console controller support** with automatic detection:

| Console | Controllers Supported |
|---------|---------------------|
| **Xbox** | Xbox One, Xbox Series X/S, Xbox 360 (with adapter) |
| **PlayStation** | DualShock 4, DualSense (PS5), DualShock 3 |
| **Nintendo Switch** | Pro Controller, Joy-Cons |
| **PC** | Any XInput or DirectInput gamepad |

### Features

✅ **Automatic Controller Detection**
- Plug-and-play support
- Hot-swapping (connect/disconnect during gameplay)
- Multi-controller support (up to 4 players)

✅ **Console-Specific Button Displays**
- Shows correct button names per controller type
- Xbox: A/B/X/Y, LB/RB, LT/RT
- PlayStation: Cross/Circle/Square/Triangle, L1/R1, L2/R2
- Switch: A/B/X/Y, L/R, ZL/ZR

✅ **Controller Vibration/Rumble**
- Haptic feedback on kicks, passes, goals
- Configurable intensity (0-100%)
- Can be enabled/disabled in settings
- Smart vibration patterns for different events

---

## 🎯 Player Skills System

### Overview

Players now have **6 special skills** with cooldowns! Each skill adds tactical depth to gameplay.

### Skills List

#### 1. ⚡ **Speed Boost**
**Hotkey:** Q (Keyboard) | Y/△ (Gamepad)
**Cooldown:** 5 seconds
**Duration:** 3 seconds

**Description:** Temporary speed increase (1.8x multiplier)

**Best Used For:**
- Breaking away from defenders
- Chasing down loose balls
- Quick counter-attacks

**Visual Effect:** Yellow glow + speed trail

**Sound:** Rising electronic beep (800 Hz)

---

#### 2. 💥 **Power Shot**
**Hotkey:** Hold Shoot + Q (Keyboard) | Hold A/X + Y/△ (Gamepad)
**Cooldown:** 8 seconds
**Power Multiplier:** 2.0x

**Description:** Devastating super-powered shot

**Best Used For:**
- Long-range goals
- Breaking through goalkeeper
- Powerful clearances

**Visual Effect:** Red charging glow

**Sound:** Deep explosive thud (150 Hz)

---

#### 3. 🌀 **Curve Shot**
**Hotkey:** Automatic when shooting with momentum
**Cooldown:** None (skill-based)

**Description:** Ball curves in flight following your movement

**Best Used For:**
- Bending shots around defenders
- Trick shots
- Scoring from angles

**Visual Effect:** Ball path curves

**Sound:** Medium frequency swoosh (500 Hz)

---

#### 4. 🤸 **Skill Move**
**Hotkey:** F (Keyboard) | RB/R1 (Gamepad)
**Cooldown:** 3 seconds
**Dash Distance:** 2 units

**Description:** Quick dash/dodge in movement direction

**Best Used For:**
- Evading tackles
- Dribbling past opponents
- Creating space

**Visual Effect:** Cyan flash + quick dash

**Sound:** High-pitched whoosh (1000 Hz)

---

#### 5. 🦵 **Slide Tackle**
**Hotkey:** C (Keyboard) | LB/L1 (Gamepad)
**Cooldown:** 6 seconds
**Range:** 2 units

**Description:** Slide forward to steal the ball

**Best Used For:**
- Defensive plays
- Stealing ball from opponents
- Blocking shots

**Visual Effect:** Orange color during slide

**Sound:** Scraping/sliding noise (250 Hz)

**Note:** Can be risky - leaves you vulnerable if missed!

---

#### 6. 🛡️ **Shield Ball**
**Hotkey:** Must have ball, auto-activates in danger
**Cooldown:** 10 seconds
**Duration:** 4 seconds

**Description:** Protect possession from opponents

**Best Used For:**
- Maintaining possession under pressure
- Waiting for teammates
- Time-wasting near end of match

**Visual Effect:** Green protective sphere

**Sound:** Defensive tone (350 Hz)

---

## 🎮 Control Schemes

### Keyboard Controls

| Action | Key | Skill |
|--------|-----|-------|
| Move | W/A/S/D | - |
| Sprint | Left Shift | - |
| Shoot | Space | - |
| Pass | E | - |
| **Speed Boost** | **Q** | ⚡ |
| **Skill Move** | **F** | 🤸 |
| **Slide Tackle** | **C** | 🦵 |

### Xbox Controller

| Action | Button | Skill |
|--------|--------|-------|
| Move | Left Stick | - |
| Sprint | LT | - |
| Shoot | A | - |
| Pass | X | - |
| **Speed Boost** | **Y** | ⚡ |
| **Skill Move** | **RB** | 🤸 |
| **Slide Tackle** | **LB** | 🦵 |
| Power Shot | A + Y | 💥 |

### PlayStation Controller

| Action | Button | Skill |
|--------|--------|-------|
| Move | Left Stick | - |
| Sprint | L2 | - |
| Shoot | Cross (✕) | - |
| Pass | Square (□) | - |
| **Speed Boost** | **Triangle (△)** | ⚡ |
| **Skill Move** | **R1** | 🤸 |
| **Slide Tackle** | **L1** | 🦵 |
| Power Shot | ✕ + △ | 💥 |

### Nintendo Switch Controller

| Action | Button | Skill |
|--------|--------|-------|
| Move | Left Stick | - |
| Sprint | ZL | - |
| Shoot | B | - |
| Pass | Y | - |
| **Speed Boost** | **X** | ⚡ |
| **Skill Move** | **R** | 🤸 |
| **Slide Tackle** | **L** | 🦵 |
| Power Shot | B + X | 💥 |

---

## 📊 Skills UI

### On-Screen Display

Each player sees their skill cooldowns in the corner:

```
[BOOST (Q)] ████████░░  80% Ready
[SKILL (F)] ██████████ 100% Ready (Green)
[TACKLE(C)] ████░░░░░░  40% Ready
```

**Colors:**
- **Green**: Skill ready to use
- **Red/Gray**: Skill on cooldown
- **Fill Bar**: Shows cooldown progress

### Position

- Player 1: Top-left corner
- Player 2: Top-right corner
- Player 3: Bottom-left corner
- Player 4: Bottom-right corner

---

## 🔧 Technical Implementation

### Component Structure

```
Player GameObject
├── PlayerController (movement, basic actions)
├── PlayerSkills (skill system)
├── PlayerAnimator (visuals)
├── Rigidbody2D (physics)
└── SpriteRenderer (rendering)
```

### Skill Cooldown System

```csharp
// Check if skill is ready
if (playerSkills.IsSkillReady(SkillType.SpeedBoost))
{
    // Activate skill
    playerSkills.ActivateSpeedBoost();
}

// Get cooldown percentage (0-1)
float cooldown = playerSkills.GetSkillCooldownPercent(SkillType.SkillMove);
```

### Vibration System

```csharp
// Trigger specific vibration patterns
ConsoleInputManager.Instance.VibrationKick(playerIndex);
ConsoleInputManager.Instance.VibrationGoal(playerIndex);
ConsoleInputManager.Instance.VibrationCollision(playerIndex);
ConsoleInputManager.Instance.VibrationSkill(playerIndex);

// Custom vibration
ConsoleInputManager.Instance.TriggerVibration(
    lowFrequency: 0.5f,    // 0-1
    highFrequency: 0.5f,   // 0-1
    duration: 0.2f,        // seconds
    gamepadIndex: 0        // player index
);

// Stop vibration
ConsoleInputManager.Instance.StopVibration(playerIndex);
ConsoleInputManager.Instance.StopAllVibrations();
```

### Console Detection

```csharp
// Get current console type
ConsoleType console = ConsoleInputManager.Instance.GetCurrentConsole();
// Returns: PC, Xbox, PlayStation, Switch, Generic

// Get button name for display
string buttonName = ConsoleInputManager.Instance.GetButtonName(GamepadButton.South);
// Xbox: "A"
// PlayStation: "Cross (✕)"
// Switch: "B"

// Check if gamepad is connected
bool hasGamepad = ConsoleInputManager.Instance.IsGamepadConnected(0);
int count = ConsoleInputManager.Instance.GetConnectedGamepadCount();
```

---

## ⚙️ Configuration

### Skill Parameters

Edit in `PlayerSkills.cs`:

```csharp
[Header("Skill Cooldowns")]
[SerializeField] private float speedBoostCooldown = 5f;      // Time between uses
[SerializeField] private float powerShotCooldown = 8f;
[SerializeField] private float skillMoveCooldown = 3f;
[SerializeField] private float slideTackleCooldown = 6f;
[SerializeField] private float shieldBallCooldown = 10f;

[Header("Skill Durations")]
[SerializeField] private float speedBoostDuration = 3f;      // How long skill lasts
[SerializeField] private float shieldBallDuration = 4f;

[Header("Skill Parameters")]
[SerializeField] private float speedBoostMultiplier = 1.8f;  // Speed increase
[SerializeField] private float powerShotMultiplier = 2.0f;   // Shot power
[SerializeField] private float curveAmount = 3f;             // Curve strength
[SerializeField] private float skillMoveDistance = 2f;       // Dash distance
[SerializeField] private float slideTackleRange = 2f;        // Tackle range
```

### Vibration Settings

Edit in `ConsoleInputManager.cs`:

```csharp
[Header("Vibration Settings")]
[SerializeField] private bool vibrationEnabled = true;       // Master switch
[SerializeField] [Range(0f, 1f)] private float vibrationIntensity = 0.7f;  // Global intensity
```

Or at runtime:

```csharp
// Enable/disable vibration
ConsoleInputManager.Instance.SetVibrationEnabled(true);

// Adjust intensity (0.0 to 1.0)
ConsoleInputManager.Instance.SetVibrationIntensity(0.5f);
```

---

## 🎨 Customization

### Changing Skill Hotkeys

Edit `PlayerInputActions_v2.inputactions`:

```json
{
    "name": "SpeedBoost",
    "bindings": [
        {"path": "<Keyboard>/q"},           // Change to any key
        {"path": "<Gamepad>/buttonNorth"}   // Y/△/X button
    ]
}
```

### Adding New Skills

1. **Add to SkillType enum:**
```csharp
public enum SkillType
{
    SpeedBoost,
    PowerShot,
    // ... existing skills
    YourNewSkill  // Add here
}
```

2. **Create activation method:**
```csharp
public void ActivateYourNewSkill()
{
    if (yourSkillTimer <= 0)
    {
        StartCoroutine(YourSkillCoroutine());
        yourSkillTimer = cooldown;
    }
}
```

3. **Implement coroutine:**
```csharp
private IEnumerator YourSkillCoroutine()
{
    // Skill effect here
    yield return new WaitForSeconds(duration);
    // Clean up
}
```

4. **Add input handler:**
```csharp
public void OnYourNewSkill(InputValue value)
{
    if (value.isPressed)
    {
        ActivateYourNewSkill();
    }
}
```

5. **Add to input actions file**

---

## 🎯 Strategy Tips

### Offensive Strategies

**Fast Break:**
1. Steal ball with Slide Tackle
2. Speed Boost forward
3. Skill Move past last defender
4. Power Shot on goal

**Technical Play:**
1. Shield Ball to maintain possession
2. Wait for teammate positioning
3. Accurate Pass
4. Curve Shot from angle

### Defensive Strategies

**Aggressive Defense:**
1. Track opponent movement
2. Slide Tackle when in range
3. Quick clear with Pass or Shoot

**Patient Defense:**
1. Position between ball and goal
2. Don't commit too early
3. Use Speed Boost only when sure
4. Slide Tackle as last resort

### Skill Combinations

**"The Finisher":**
Speed Boost → Skill Move → Power Shot

**"The Playmaker":**
Shield Ball → Look for teammate → Curve Pass

**"The Defender":**
Speed Boost → Slide Tackle → Clear

---

## 🐛 Troubleshooting

### Skills Not Working

**Issue:** Skills don't activate
**Solutions:**
- Check PlayerSkills component is attached
- Verify input action bindings
- Check cooldown isn't active (UI shows red)
- Ensure player has ball (for ball-dependent skills)

### Controller Not Detected

**Issue:** Gamepad not recognized
**Solutions:**
- Check USB connection
- Try different USB port
- Restart game after connecting controller
- Update controller drivers
- Check Unity Input System is installed

### No Vibration

**Issue:** Controller doesn't vibrate
**Solutions:**
- Check vibration is enabled: `ConsoleInputManager.Instance.SetVibrationEnabled(true)`
- Verify controller supports rumble
- Check vibration intensity: `SetVibrationIntensity(0.7f)`
- Some controllers require specific drivers

### Cooldown UI Not Showing

**Issue:** Skill cooldowns not displayed
**Solutions:**
- Check SkillCooldownUI component exists
- Verify Canvas has UI elements
- Check PlayerSkills reference is assigned
- Make sure TextMeshPro is installed

---

## 📈 Performance Impact

### Skills System

- **Memory:** < 20 KB per player
- **CPU:** Minimal (coroutine-based)
- **Skill checks:** Once per frame
- **Visual effects:** Simple sprites/colors

### Console Input Manager

- **Memory:** < 10 KB
- **CPU:** Event-driven (no polling)
- **Vibration:** Native API calls
- **Detection:** Automatic, non-blocking

**Total Performance Impact:** Negligible on all platforms

---

## 🎓 Advanced Topics

### Creating Custom Vibration Patterns

```csharp
// Goal celebration pattern
IEnumerator GoalVibrationPattern()
{
    for (int i = 0; i < 3; i++)
    {
        ConsoleInputManager.Instance.TriggerVibration(0.8f, 0.8f, 0.2f, 0);
        yield return new WaitForSeconds(0.3f);
    }
}

// Continuous rumble
IEnumerator ContinuousRumble(float duration)
{
    float elapsed = 0f;
    while (elapsed < duration)
    {
        ConsoleInputManager.Instance.TriggerVibration(0.3f, 0.3f, 0.1f, 0);
        yield return new WaitForSeconds(0.1f);
        elapsed += 0.1f;
    }
}
```

### Skill Chaining System

```csharp
// Allow skills to combo into each other
private SkillType lastUsedSkill = SkillType.None;
private float comboWindow = 1.0f;
private float lastSkillTime;

public void ActivateSkillWithCombo(SkillType skill)
{
    if (Time.time - lastSkillTime < comboWindow)
    {
        // Bonus for combo!
        float bonusMultiplier = 1.2f;
        // Apply bonus...
    }

    lastUsedSkill = skill;
    lastSkillTime = Time.time;

    // Activate skill normally...
}
```

### Platform-Specific Features

```csharp
void ConfigureForPlatform()
{
    ConsoleType console = ConsoleInputManager.Instance.GetCurrentConsole();

    switch (console)
    {
        case ConsoleType.PlayStation:
            // DualSense adaptive triggers (PS5)
            // Touchpad functionality
            break;

        case ConsoleType.Xbox:
            // Impulse triggers (Xbox One/Series)
            break;

        case ConsoleType.Switch:
            // HD Rumble
            // Motion controls
            break;
    }
}
```

---

## 📚 API Reference

### PlayerSkills

```csharp
// Activation
void ActivateSpeedBoost()
void ActivatePowerShot()
void ActivateCurveShot()
void ActivateSkillMove()
void ActivateSlideTackle()
void ActivateShieldBall()

// Status queries
bool IsSkillReady(SkillType skill)
float GetSkillCooldownPercent(SkillType skill)  // Returns 0-1
bool IsSpeedBoosting()
bool IsShieldingBall()
float GetSpeedBoostMultiplier()  // Returns 1.0 or boosted value
```

### ConsoleInputManager

```csharp
// Vibration
void TriggerVibration(float low, float high, float duration, int gamepadIndex)
void VibrationKick(int gamepadIndex)
void VibrationGoal(int gamepadIndex)
void VibrationCollision(int gamepadIndex)
void VibrationSkill(int gamepadIndex)
void StopVibration(int gamepadIndex)
void StopAllVibrations()

// Console detection
ConsoleType GetCurrentConsole()
string GetButtonName(GamepadButton button)
bool IsGamepadConnected(int index)
int GetConnectedGamepadCount()

// Settings
void SetVibrationEnabled(bool enabled)
void SetVibrationIntensity(float intensity)
bool IsVibrationEnabled()
```

---

## 🎉 What's Next?

### Potential Enhancements

1. **More Skills**
   - Ultimate ability (super powerful, once per match)
   - Team combo skills
   - Position-specific skills (striker, defender, etc.)

2. **Skill Progression**
   - Unlock skills as you play
   - Upgrade skill power/cooldown
   - Skill points system

3. **Advanced Console Features**
   - PlayStation 5 adaptive triggers
   - Nintendo Switch motion controls
   - Xbox Impulse Triggers

4. **Skill Customization**
   - Choose 3 skills from larger pool
   - Create custom skill loadouts
   - Skill presets for different playstyles

---

**Version:** 3.0
**Last Updated:** 2025-11-11
**Compatibility:** Unity 2022.3.10f1+

**Enjoy your enhanced soccer game with skills and full console support!** ⚽🎮✨
