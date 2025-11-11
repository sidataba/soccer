# Steam Soccer Game - FIFA Style Multiplayer

A Unity-based 2.5D top-down soccer game with local multiplayer support and full Steam integration. Play with up to 4 players locally using keyboards and gamepads!

## Features

- **Local Multiplayer**: 2-4 players on the same computer
- **Multiple Input Support**: Keyboard (WASD/Arrows) and Xbox/PlayStation controllers
- **Steam Integration**:
  - Achievements
  - Leaderboards
  - Steam API integration
- **Core Soccer Mechanics**:
  - Player movement with sprint
  - Ball physics with realistic friction
  - Shooting and passing
  - Goal detection and scoring
  - Match timer (5 minutes default)
  - Win conditions

## Controls

### Player 1 (Keyboard - WASD)
- **Move**: W/A/S/D keys
- **Sprint**: Left Shift
- **Shoot**: Space Bar
- **Pass**: E key

### Player 2 (Keyboard - Arrows)
- **Move**: Arrow Keys
- **Sprint**: Right Shift
- **Shoot**: Numpad 0
- **Pass**: Numpad 1

### Gamepad (Xbox/PlayStation)
- **Move**: Left Stick
- **Sprint**: Left Trigger (LT/L2)
- **Shoot**: A Button (Xbox) / X Button (PS)
- **Pass**: X Button (Xbox) / Square Button (PS)

### General
- **Pause**: ESC key

## Requirements

### Development
- Unity 2022.3.10f1 or later
- Visual Studio 2019/2022 or VS Code
- Steamworks.NET package
- Steam client installed

### Playing
- Steam client installed and running
- Windows/Mac/Linux OS
- Xbox or PlayStation controller (optional)

## Setup Instructions

### 1. Install Unity
1. Download Unity Hub from https://unity.com/download
2. Install Unity 2022.3.10f1 (LTS) or later
3. Add the project folder to Unity Hub

### 2. Install Steamworks.NET

#### Option A: Unity Package Manager
1. Open Unity Package Manager (Window > Package Manager)
2. Click "+" > "Add package from git URL"
3. Enter: `https://github.com/rlabrecque/Steamworks.NET.git`

#### Option B: Manual Installation
1. Download Steamworks.NET from: https://github.com/rlabrecque/Steamworks.NET/releases
2. Extract to `Assets/Plugins/Steamworks.NET/`
3. Copy the appropriate Steam library files:
   - Windows: `steam_api64.dll` to project root
   - Mac: `libsteam_api.dylib` to project root
   - Linux: `libsteam_api.so` to project root

### 3. Configure Steam

1. **Get Steam App ID**:
   - If testing: Use App ID `480` (Spacewar - Valve's test app)
   - For release: Register your game at https://partner.steamgames.com/

2. **Update steam_appid.txt**:
   - Located in project root
   - Replace `480` with your actual App ID

3. **Steam Partner Setup** (For Release):
   - Create achievements in Steamworks Partner dashboard
   - Set up leaderboards
   - Configure store page
   - Upload builds through Steam Partner

## Building the Game

### Build Settings

1. Open Unity Build Settings: `File > Build Settings`
2. Add your main scene to "Scenes in Build"
3. Select target platform (PC, Mac, Linux)
4. Click "Player Settings" and configure:
   - **Company Name**: Your name/studio
   - **Product Name**: Your game name
   - **Version**: 1.0.0
   - **Icon**: Your game icon

### Building for Steam

1. **Build the Game**:
   ```
   File > Build Settings > Build
   ```
   Choose output directory (e.g., `Builds/Windows/`)

2. **Copy Steam Files**:
   - Copy `steam_api64.dll` (Windows) to build folder next to .exe
   - Copy `steam_appid.txt` to build folder
   - Mac/Linux: Copy respective Steam library files

3. **Test Build**:
   - Make sure Steam is running
   - Launch the built executable
   - Verify Steam overlay works (Shift+Tab)

### Uploading to Steam

1. **Install Steamworks SDK**:
   - Download from Steam Partner site
   - Extract `sdk/tools/ContentBuilder/`

2. **Create App Build Scripts**:
   Create `app_build_[appid].vdf`:
   ```vdf
   "AppBuild"
   {
       "AppID" "YOUR_APP_ID"
       "Desc" "Your build description"
       "ContentRoot" "C:\\path\\to\\your\\build\\"
       "BuildOutput" "C:\\output\\"
       "Depots"
       {
           "DepotID" "YOUR_DEPOT_ID"
           {
               "FileMapping"
               {
                   "LocalPath" "*"
                   "DepotPath" "."
                   "Recursive" "1"
               }
           }
       }
   }
   ```

3. **Upload Build**:
   ```bash
   cd ContentBuilder
   steamcmd +login YOUR_USERNAME +run_app_build ../scripts/app_build_[appid].vdf +quit
   ```

4. **Set Build Live**:
   - Go to Steam Partner dashboard
   - Navigate to "Builds" section
   - Set your uploaded build to default/live

## Project Structure

```
soccer/
├── Assets/
│   ├── Scripts/
│   │   ├── Player/
│   │   │   └── PlayerController.cs      # Player movement and controls
│   │   ├── Ball/
│   │   │   └── Ball.cs                  # Ball physics
│   │   ├── GameManagement/
│   │   │   ├── GameManager.cs           # Match logic and scoring
│   │   │   ├── Goal.cs                  # Goal detection
│   │   │   ├── FieldManager.cs          # Field creation
│   │   │   └── PlayerSpawner.cs         # Player spawning
│   │   ├── UI/
│   │   │   └── UIManager.cs             # UI and menus
│   │   └── Steam/
│   │       ├── SteamManager.cs          # Steam initialization
│   │       ├── SteamAchievements.cs     # Achievement handling
│   │       └── SteamLeaderboards.cs     # Leaderboard handling
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Materials/
│   └── PlayerInputActions.inputactions  # Input configuration
├── ProjectSettings/
├── Packages/
└── steam_appid.txt                      # Steam App ID
```

## Creating Your First Scene

1. **Create Main Scene**:
   - `File > New Scene`
   - Add GameObject: "GameManager" with `GameManager.cs`
   - Add GameObject: "FieldManager" with `FieldManager.cs`
   - Add GameObject: "PlayerSpawner" with `PlayerSpawner.cs`
   - Add GameObject: "Ball" with `Ball.cs` and `Rigidbody2D`, `CircleCollider2D`
   - Add GameObject: "UICanvas" with `UIManager.cs`

2. **Setup Camera**:
   - Set Camera to Orthographic
   - Size: 8-10
   - Position: (0, 0, -10)

3. **Create Player Prefab**:
   - Create GameObject with:
     - `PlayerController.cs`
     - `Rigidbody2D` (Freeze Rotation Z)
     - `CircleCollider2D`
     - `PlayerInput` (assign PlayerInputActions asset)
     - `SpriteRenderer` (create simple circle sprite)
   - Save as Prefab in Assets/Prefabs/

4. **Configure Tags**:
   - Create tag "Ball"
   - Create tag "Wall"
   - Create tag "Goal"
   - Assign appropriately

## Customization

### Game Settings
Edit values in `GameManager.cs`:
- `matchDuration`: Match length in seconds (default: 300)
- `scoreToWin`: Goals needed to win (default: 5)

### Player Settings
Edit values in `PlayerController.cs`:
- `moveSpeed`: Normal movement speed (default: 5)
- `sprintSpeed`: Sprint speed (default: 8)
- `shootPower`: Shot power (default: 15)
- `passPower`: Pass power (default: 8)

### Field Size
Edit values in `FieldManager.cs`:
- `fieldWidth`: Field width (default: 20)
- `fieldHeight`: Field height (default: 12)
- `goalWidth`: Goal width (default: 3)

## Troubleshooting

### Steam Not Initializing
- Ensure Steam client is running
- Check `steam_appid.txt` exists in build folder
- Verify Steam library files are in correct location
- For testing, use App ID `480`

### Controllers Not Working
- Install Unity Input System package
- Regenerate Input Actions asset
- Check controller is detected in Unity Input Debugger

### Build Errors
- Verify all Steamworks.NET files are included
- Check platform-specific Steam libraries are copied
- Ensure all scenes are added to Build Settings

### Performance Issues
- Reduce number of players
- Optimize ball physics settings
- Lower screen resolution

## Next Steps & Improvements

This is a foundation. To make it more FIFA-like:

1. **Graphics**:
   - Add 3D models for players and ball
   - Create textured soccer field
   - Add particle effects for kicks
   - Add animated player sprites

2. **Gameplay**:
   - AI opponents
   - Team formations
   - Player stamina system
   - Different player roles (striker, defender, goalkeeper)
   - Fouls and penalties
   - Corner kicks and throw-ins

3. **Audio**:
   - Crowd sounds
   - Kick sounds
   - Goal celebration sounds
   - Commentator voice lines

4. **Online Multiplayer**:
   - Steam P2P networking
   - Matchmaking
   - Friend invites

5. **Modes**:
   - Tournament mode
   - Career mode
   - Practice mode
   - Replay system

6. **Steam Features**:
   - Trading cards
   - Cloud saves
   - Rich presence
   - Steam Workshop support

## License

This is a starter project. Feel free to use and modify for your own games.

## Credits

- Built with Unity
- Steam integration via Steamworks.NET
- Input handling via Unity Input System

## Support

For issues or questions:
1. Check Unity documentation: https://docs.unity3d.com/
2. Steamworks.NET docs: https://steamworks.github.io/
3. Unity forums: https://forum.unity.com/

---

**Important Note**: Creating a full FIFA-quality game requires a professional team and significant resources. This project provides a solid foundation to build upon and learn game development and Steam integration.
