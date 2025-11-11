# Quick Setup Guide - Steam Soccer Game

This guide will help you get the game running in 15 minutes!

## Prerequisites Checklist

- [ ] Unity Hub installed
- [ ] Unity 2022.3.10f1 or later installed
- [ ] Steam client installed and running
- [ ] Internet connection (for downloading packages)

## Step-by-Step Setup

### Step 1: Open Project in Unity (2 minutes)

1. Launch Unity Hub
2. Click "Add" → "Add project from disk"
3. Navigate to this project folder
4. Click "Select Folder"
5. Unity will open and import the project (may take a few minutes)

### Step 2: Install Required Packages (3 minutes)

#### Install Steamworks.NET:

1. Download latest Steamworks.NET from GitHub:
   ```
   https://github.com/rlabrecque/Steamworks.NET/releases
   ```

2. Extract the downloaded zip file

3. In your project folder:
   - Create `Assets/Plugins/Steamworks.NET/` folder
   - Copy all contents from extracted zip to this folder

4. Download Steam SDK from:
   ```
   https://partner.steamgames.com/downloads/steamworks_sdk.zip
   ```

5. Extract and locate these files:
   - Windows: `sdk/redistributable_bin/win64/steam_api64.dll`
   - Mac: `sdk/redistributable_bin/osx/libsteam_api.dylib`
   - Linux: `sdk/redistributable_bin/linux64/libsteam_api.so`

6. Copy the appropriate file for your OS to your project root folder (where `steam_appid.txt` is located)

#### Verify Input System:

1. In Unity, go to `Window > Package Manager`
2. Search for "Input System"
3. If not installed, click "Install"
4. If prompted to restart Unity, click "Yes"

### Step 3: Create the Game Scene (5 minutes)

#### A. Create New Scene

1. `File > New Scene` → "2D"
2. Save as `Assets/Scenes/MainGame.unity`

#### B. Setup Camera

1. Select "Main Camera" in Hierarchy
2. In Inspector:
   - Position: (0, 0, -10)
   - Size: 10
   - Background: Dark green (#0F5132)

#### C. Create Game Objects

**Create GameManager:**
1. Right-click in Hierarchy → Create Empty
2. Rename to "GameManager"
3. Add Component → Search "Game Manager"
4. In Inspector, assign reference to Ball (will create next)

**Create FieldManager:**
1. Right-click in Hierarchy → Create Empty
2. Rename to "FieldManager"
3. Add Component → Search "Field Manager"

**Create Ball:**
1. Right-click in Hierarchy → 2D Object → Sprites → Circle
2. Rename to "Ball"
3. Tag: "Ball" (create if needed)
4. Add Component → "Rigidbody 2D"
   - Gravity Scale: 0
   - Linear Drag: 0.5
   - Mass: 1
5. Add Component → "Circle Collider 2D"
6. Add Component → Search "Ball" script
7. Set scale to (0.3, 0.3, 1)
8. Set color to white

**Create Player Prefab:**
1. Right-click in Hierarchy → 2D Object → Sprites → Circle
2. Rename to "Player"
3. Add Component → "Rigidbody 2D"
   - Gravity Scale: 0
   - Freeze Rotation: Z checked
4. Add Component → "Circle Collider 2D"
5. Add Component → "Player Controller"
6. Add Component → "Player Input"
   - In Actions, assign `Assets/PlayerInputActions`
   - Behavior: "Invoke Unity Events"
7. Set scale to (0.5, 0.5, 1)
8. Drag to `Assets/Prefabs/` folder to create prefab
9. Delete from Hierarchy

**Create PlayerSpawner:**
1. Right-click in Hierarchy → Create Empty
2. Rename to "PlayerSpawner"
3. Add Component → "Player Spawner"
4. In Inspector:
   - Drag Player prefab from Prefabs folder to "Player Prefab" field
   - Click "Create Spawn Points" (if available) or manually set team colors

**Create UI:**
1. Right-click in Hierarchy → UI → Canvas
2. Rename to "GameUI"
3. Canvas Scaler → UI Scale Mode: "Scale With Screen Size"
4. Add Component → "UI Manager"

**Create Score Display:**
1. Right-click GameUI → UI → TextMeshPro - Text (if prompted, import TMP Essentials)
2. Rename to "Team1Score"
3. Set position: (-200, 300, 0)
4. Text: "0"
5. Font Size: 48
6. Alignment: Center
7. Color: Red

8. Duplicate (Ctrl+D) and rename to "Team2Score"
9. Set position: (200, 300, 0)
10. Color: Blue

**Create Timer:**
1. Right-click GameUI → UI → TextMeshPro - Text
2. Rename to "Timer"
3. Set position: (0, 300, 0)
4. Text: "05:00"
5. Font Size: 36

**Link UI to UIManager:**
1. Select GameUI
2. In UI Manager component:
   - Drag Team1Score to "Team 1 Score Text"
   - Drag Team2Score to "Team 2 Score Text"
   - Drag Timer to "Match Time Text"

#### D. Link GameManager References

1. Select GameManager
2. In Game Manager component:
   - Drag Ball to "Ball" field
   - Center Point: Create empty GameObject at (0,0,0) and drag here

### Step 4: Configure Build Settings (2 minutes)

1. `File > Build Settings`
2. Click "Add Open Scenes" to add MainGame scene
3. Platform: PC, Mac & Linux Standalone
4. Architecture: x86_64
5. Click "Player Settings":
   - Company Name: Your name
   - Product Name: "Soccer Game"
   - Version: 1.0.0

### Step 5: Test the Game (2 minutes)

1. Click Play button in Unity
2. You should see:
   - Green field with goals
   - Ball in center
   - 4 players (2 red, 2 blue)
   - Score display showing "0 - 0"
   - Timer showing "05:00"

**Test Controls:**
- WASD: Move Player 1
- Space: Shoot (when near ball)
- Arrow Keys: Move Player 2

3. Try scoring a goal!

### Step 6: Build Executable (3 minutes)

1. `File > Build Settings`
2. Click "Build"
3. Choose location (e.g., `Builds/SoccerGame/`)
4. Wait for build to complete

**IMPORTANT**: After build completes:
1. Copy `steam_api64.dll` (Windows) or equivalent to build folder
2. Copy `steam_appid.txt` to build folder next to .exe
3. Make sure Steam is running
4. Launch your game!

## Testing Checklist

- [ ] Game launches without errors
- [ ] Players can move with WASD and Arrow keys
- [ ] Ball moves when kicked
- [ ] Goals are detected and score updates
- [ ] Timer counts down
- [ ] Match ends when timer reaches 0
- [ ] Can pause with ESC

## Common Issues

### "Steam not initialized" warning
- ✅ Make sure Steam client is running
- ✅ Check `steam_appid.txt` is in project root
- ✅ Verify steam_api64.dll is in project root

### Controllers don't work
- ✅ Regenerate Input Actions: Select `PlayerInputActions.inputactions` → Click "Generate C# Class"
- ✅ Restart Unity

### Can't see players/ball
- ✅ Check camera is at Z = -10
- ✅ Verify sprites have color (not black)
- ✅ Check objects are at Z = 0

### Build fails
- ✅ Check all scenes are added to Build Settings
- ✅ Verify Steamworks.NET is properly installed
- ✅ Try cleaning build folder and rebuilding

## Next Steps

Once you have the game running:

1. **Customize Settings**:
   - Adjust match duration in GameManager
   - Change player speeds in PlayerController
   - Modify field size in FieldManager

2. **Add Graphics**:
   - Replace circle sprites with proper player/ball sprites
   - Add field textures
   - Create animated player sprites

3. **Add Audio**:
   - Import sound effects
   - Add AudioSource components
   - Play sounds on kick, goal, etc.

4. **Prepare for Steam Release**:
   - Register at https://partner.steamgames.com/
   - Get your App ID
   - Replace 480 in steam_appid.txt with your App ID
   - Set up achievements and leaderboards
   - Follow upload instructions in main README

## Need Help?

- Unity Documentation: https://docs.unity3d.com/
- Steamworks.NET: https://steamworks.github.io/
- Unity Forums: https://forum.unity.com/

---

**Estimated Total Time**: 15-20 minutes

**Difficulty**: Beginner-Friendly

Good luck with your soccer game! ⚽🎮
