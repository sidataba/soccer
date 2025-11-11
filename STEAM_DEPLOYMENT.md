# Steam Deployment Guide

Complete guide to deploying your soccer game to Steam.

## Prerequisites

- [ ] Steamworks Partner account ($100 USD fee)
- [ ] Completed game build
- [ ] All required assets (icons, screenshots, videos)
- [ ] Legal documents (privacy policy, EULA if applicable)

## Part 1: Steamworks Partner Setup (1-2 hours)

### Step 1: Register Your Game

1. Go to https://partner.steamgames.com/
2. Sign in with your Steam account
3. Pay the $100 USD Steam Direct fee (one-time per developer)
4. Click "Publish" → "Create New App"
5. Fill out initial details:
   - App Type: "Game"
   - Release Type: "Full Release" or "Early Access"
   - Base Price
6. Note your **App ID** (you'll need this!)

### Step 2: Update Your Project

1. Open `steam_appid.txt` in your project root
2. Replace `480` with your actual App ID
3. Save and rebuild your game

### Step 3: Store Page Setup

Complete all required fields in Steamworks Partner:

#### Basic Info Tab:
- **Game Name**: Your soccer game name
- **Developer**: Your name/studio
- **Publisher**: Your name/studio (or leave same as developer)
- **Release Date**: Target launch date
- **Supported Languages**: List all languages
- **Categories**: Check applicable:
  - Local Multi-Player
  - Steam Achievements
  - Steam Leaderboards
  - Full Controller Support

#### Description:
```
[Suggested template - customize as needed]

Kick off the ultimate soccer experience!

⚽ FEATURES ⚽
• Local multiplayer for 2-4 players
• Fast-paced arcade soccer action
• Multiple control schemes (Keyboard + Gamepad)
• Steam Achievements and Leaderboards
• Competitive match timer gameplay

🎮 GAMEPLAY 🎮
Control your team, pass the ball, and score amazing goals!
Perfect for playing with friends on the same computer.

🏆 COMPETE 🏆
Climb the Steam leaderboards and unlock achievements!

Whether you're a casual player or competitive gamer,
[Your Game Name] delivers hours of soccer fun!
```

#### Media:
Upload at least:
- 1 Header image (460x215)
- 5 Screenshots (1920x1080 recommended)
- 1 Trailer video (optional but recommended)
- Library assets (600x900, 300x450)
- Small capsule (231x87)
- Main capsule (616x353)
- Header capsule (460x215)

### Step 4: Configure Achievements

1. Go to "Steamworks Settings" → "Achievement Configuration"
2. Create achievements:

**Suggested Achievements:**
```
ID: FIRST_GOAL
Name: "First Goal"
Description: "Score your first goal"
Hidden: No

ID: WIN_MATCH
Name: "Victory!"
Description: "Win your first match"
Hidden: No

ID: SCORE_5_GOALS
Name: "Hat Trick Master"
Description: "Score 5 goals in a single match"
Hidden: No

ID: WIN_10_MATCHES
Name: "Champion"
Description: "Win 10 matches"
Hidden: No

ID: PERFECT_MATCH
Name: "Clean Sheet"
Description: "Win a match without conceding a goal"
Hidden: No
```

3. Upload achievement icons (64x64 PNG for each)
4. Click "Save"
5. Click "Publish"

### Step 5: Configure Leaderboards

1. Go to "Steamworks Settings" → "Leaderboards"
2. Create leaderboard:

**Main Leaderboard:**
- Name: "Most_Goals_Scored"
- Display Name: "Career Goals"
- Sort Method: Descending (highest first)
- Display Type: Numeric
- Uploads: Trusted

3. Click "Save"
4. Click "Publish"

### Step 6: Pricing

1. Go to "Pricing & Packaging"
2. Set your base price (suggest $4.99 - $9.99 for indie soccer games)
3. Configure regional pricing (Steam suggests automatically)
4. Set up launch discount if desired (10-25% common)

### Step 7: Tax & Banking

Complete tax interview and banking information for revenue payments.

## Part 2: Building for Steam (30 minutes)

### Step 1: Build Your Game

In Unity:
1. `File > Build Settings`
2. Ensure your scene is added
3. Platform settings:
   - **PC**: x86_64
   - **Mac**: Universal (Intel + Apple Silicon)
   - **Linux**: x86_64
4. Click "Build"
5. Choose output folder: `Builds/Steam/Windows/` (or Mac/Linux)

### Step 2: Prepare Build Folders

Create this structure:
```
MyGame/
├── Windows/
│   ├── [YourGame].exe
│   ├── steam_api64.dll
│   ├── steam_appid.txt
│   └── [Other game files]
├── Mac/
│   ├── [YourGame].app/
│   ├── libsteam_api.dylib
│   └── steam_appid.txt
└── Linux/
    ├── [YourGame].x86_64
    ├── libsteam_api.so
    └── steam_appid.txt
```

**Critical**: Each platform needs:
- The game executable
- Platform-specific Steam library
- steam_appid.txt with your App ID

### Step 3: Test Local Build

1. Launch Steam
2. Run your .exe (Windows), .app (Mac), or binary (Linux)
3. Verify:
   - Game launches
   - Steam overlay works (Shift+Tab)
   - No errors in console
4. Test all gameplay features

## Part 3: Uploading to Steam (1 hour)

### Step 1: Download Steamworks SDK

1. Go to https://partner.steamgames.com/
2. Navigate to "Downloads" → "Steamworks SDK"
3. Download and extract
4. Locate `sdk/tools/ContentBuilder/`

### Step 2: Create Build Configuration

Create file: `scripts/app_build_[YOUR_APP_ID].vdf`

Replace YOUR_APP_ID, YOUR_DEPOT_ID, and paths:

```vdf
"AppBuild"
{
    "AppID" "YOUR_APP_ID"    // Your App ID
    "Desc" "Version 1.0.0"   // Build description

    // Base content folder (all platforms)
    "ContentRoot" "C:\\MyGame\\Builds\\"

    // Output folder for build logs
    "BuildOutput" "C:\\MyGame\\BuildOutput\\"

    // Depots (one per platform)
    "Depots"
    {
        // Windows Depot
        "YOUR_WINDOWS_DEPOT_ID"
        {
            "FileMapping"
            {
                "LocalPath" "Windows\\*"
                "DepotPath" "."
                "Recursive" "1"
            }
        }

        // Mac Depot (if applicable)
        "YOUR_MAC_DEPOT_ID"
        {
            "FileMapping"
            {
                "LocalPath" "Mac\\*"
                "DepotPath" "."
                "Recursive" "1"
            }
        }

        // Linux Depot (if applicable)
        "YOUR_LINUX_DEPOT_ID"
        {
            "FileMapping"
            {
                "LocalPath" "Linux\\*"
                "DepotPath" "."
                "Recursive" "1"
            }
        }
    }
}
```

Find your Depot IDs:
1. Go to Steamworks Partner
2. "App Admin" → "Depots"
3. Each platform has a unique Depot ID

### Step 3: Create Depot Configurations

For each depot, create `depot_build_[DEPOT_ID].vdf`:

**Example for Windows Depot:**
```vdf
"DepotBuild"
{
    "DepotID" "YOUR_WINDOWS_DEPOT_ID"
    "ContentRoot" "C:\\MyGame\\Builds\\Windows\\"
    "FileMapping"
    {
        "LocalPath" "*"
        "DepotPath" "."
        "Recursive" "1"
    }

    "FileExclusion" "*.pdb"    // Exclude debug files
    "FileExclusion" "*.log"    // Exclude log files
}
```

### Step 4: Upload Build

**Windows:**
```batch
cd ContentBuilder
builder\steamcmd.exe +login YOUR_STEAM_USERNAME +run_app_build ..\scripts\app_build_YOUR_APP_ID.vdf +quit
```

**Mac/Linux:**
```bash
cd ContentBuilder
./builder_osx/steamcmd.sh +login YOUR_STEAM_USERNAME +run_app_build ../scripts/app_build_YOUR_APP_ID.vdf +quit
# Or use builder_linux for Linux
```

**Note**: You'll be prompted for:
1. Your Steam password
2. Steam Guard code (email or mobile authenticator)

Upload time depends on file size and connection speed (typically 5-30 minutes).

### Step 5: Set Build Live

1. Go to Steamworks Partner → "Builds"
2. You'll see your uploaded build
3. Select build → "Set build live now" for default branch
4. Confirm

For beta testing:
- Create beta branch
- Set build to beta branch instead
- Share beta access key with testers

## Part 4: Pre-Launch Testing (1-2 days)

### Internal Testing

1. **Use Steam Keys**:
   - Go to "Generate Steam Keys"
   - Create 5-10 keys for testing
   - Distribute to testers

2. **Test Checklist**:
   - [ ] Game downloads and installs correctly
   - [ ] All features work as expected
   - [ ] Achievements unlock properly
   - [ ] Leaderboards update correctly
   - [ ] Steam overlay functions (Shift+Tab)
   - [ ] Controller support works
   - [ ] No crashes or major bugs
   - [ ] Performance is acceptable

### Beta Testing (Optional)

1. Create beta branch in Steamworks
2. Upload build to beta branch
3. Share access code with beta testers
4. Gather feedback
5. Fix issues
6. Upload final build

## Part 5: Launch (Launch Day!)

### Pre-Launch (1 week before)

1. **Complete Store Page**:
   - All descriptions finalized
   - All media uploaded
   - Pricing confirmed
   - Release date set

2. **Marketing** (if applicable):
   - Social media announcements
   - Press kit prepared
   - Trailer uploaded to YouTube
   - Contact gaming press/influencers

3. **Final Build**:
   - Upload final, tested build
   - Set as default branch
   - Do final smoke test

### Launch Day

1. **Morning of Launch**:
   - Verify build is live
   - Check store page is visible
   - Test purchase flow (use test account)

2. **Release**:
   - Go to App Admin
   - Click "Publish to Steam"
   - Confirm release

3. **Monitor**:
   - Watch for bug reports
   - Monitor Steam forums
   - Check reviews
   - Be ready to push hotfix if needed

### Post-Launch (First Week)

- Respond to reviews and feedback
- Fix critical bugs immediately
- Update store page if needed
- Thank players for support!

## Part 6: Ongoing Maintenance

### Updates

To push updates:
1. Build new version
2. Update version number
3. Create new build description in vdf
4. Upload using ContentBuilder
5. Set new build live

### Marketing

- Regular sales (Steam seasonal sales)
- Content updates
- Community engagement
- Stream key giveaways

### Analytics

Monitor in Steamworks:
- Sales data
- Player count
- Regional performance
- Revenue reports

## Cost Breakdown

**Minimum Costs:**
- Steam Direct Fee: $100 USD (one-time)
- Total: $100 USD

**Optional Costs:**
- Art assets: $0 - $500+
- Music/SFX: $0 - $300+
- Marketing: $0 - $1000+
- Beta testing services: $0 - $500+

## Timeline Summary

- **Week 1**: Steamworks setup, store page
- **Week 2**: Build preparation, achievements/leaderboards
- **Week 3**: Testing and iteration
- **Week 4**: Final testing, marketing prep
- **Week 5**: LAUNCH! 🚀

## Support Resources

- **Steamworks Documentation**: https://partner.steamgames.com/doc/home
- **Steamworks SDK**: https://partner.steamgames.com/downloads
- **Steamworks Developer Forums**: https://steamcommunity.com/groups/steamworks
- **Steamworks Support**: Through partner site

## Legal Requirements

- Privacy Policy (if collecting any data)
- Terms of Service
- Age rating (ESRB, PEGI, etc.)
- Content warnings if applicable

## Pro Tips

1. **Price competitively**: Research similar games
2. **Launch discount**: 10-15% attracts early buyers
3. **Wishlist campaign**: Build wishlists before launch
4. **Regular updates**: Keep players engaged
5. **Community**: Respond to forums and reviews
6. **Sales**: Participate in Steam seasonal sales
7. **Bundle deals**: Partner with other developers
8. **Trading cards**: Add later for engagement boost

## Common Issues

### Build fails to upload
- Check file paths in .vdf files
- Verify depot IDs are correct
- Ensure SteamCmd has write permissions

### Game won't launch after download
- Verify Steam library files included
- Check steam_appid.txt is present
- Test locally first before uploading

### Achievements not working
- Verify achievement IDs match code
- Check achievements are published in Steamworks
- Ensure SteamUserStats.RequestCurrentStats() is called

---

**Congratulations on preparing to launch on Steam!** 🎉

This is a significant achievement. Take your time with each step, test thoroughly, and don't hesitate to reach out to the Steamworks support team if you encounter issues.

Good luck with your launch! ⚽🎮🚀
