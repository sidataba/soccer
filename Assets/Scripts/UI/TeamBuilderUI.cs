using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI for team builder and customization
/// Allows players to create and edit teams before matches
/// </summary>
public class TeamBuilderUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject teamSelectPanel;
    [SerializeField] private GameObject teamCustomizePanel;
    [SerializeField] private GameObject playerEditPanel;

    [Header("Team Selection")]
    [SerializeField] private TMP_Dropdown team1Dropdown;
    [SerializeField] private TMP_Dropdown team2Dropdown;
    [SerializeField] private Button startMatchButton;

    [Header("Team Customization")]
    [SerializeField] private TMP_InputField teamNameInput;
    [SerializeField] private Image primaryColorPreview;
    [SerializeField] private Image secondaryColorPreview;
    [SerializeField] private Slider primaryColorR, primaryColorG, primaryColorB;
    [SerializeField] private Slider secondaryColorR, secondaryColorG, secondaryColorB;
    [SerializeField] private TMP_Dropdown formationDropdown;

    [Header("Player List")]
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerItemPrefab;

    [Header("References")]
    [SerializeField] private TeamBuilder teamBuilder;

    private TeamData currentEditingTeam;
    private bool editingTeam1 = true;

    void Start()
    {
        if (teamBuilder == null)
        {
            teamBuilder = TeamBuilder.Instance;
            if (teamBuilder == null)
            {
                GameObject tb = new GameObject("TeamBuilder");
                teamBuilder = tb.AddComponent<TeamBuilder>();
            }
        }

        SetupUI();
        ShowTeamSelection();
    }

    void SetupUI()
    {
        // Populate team dropdowns
        PopulateTeamDropdowns();

        // Setup formation dropdown
        if (formationDropdown != null)
        {
            formationDropdown.ClearOptions();
            foreach (FormationType formation in System.Enum.GetValues(typeof(FormationType)))
            {
                formationDropdown.options.Add(new TMP_Dropdown.OptionData(formation.ToString().Replace("_", "-")));
            }
            formationDropdown.RefreshShownValue();
        }

        // Setup button listeners
        if (startMatchButton != null)
        {
            startMatchButton.onClick.AddListener(StartMatch);
        }

        // Hide all panels initially
        if (mainPanel != null) mainPanel.SetActive(false);
        if (teamSelectPanel != null) teamSelectPanel.SetActive(false);
        if (teamCustomizePanel != null) teamCustomizePanel.SetActive(false);
        if (playerEditPanel != null) playerEditPanel.SetActive(false);
    }

    void PopulateTeamDropdowns()
    {
        if (team1Dropdown == null || team2Dropdown == null) return;

        team1Dropdown.ClearOptions();
        team2Dropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        options.Add("Custom Team");

        foreach (var team in teamBuilder.GetPresetTeams())
        {
            options.Add(team.teamName);
        }

        team1Dropdown.AddOptions(options);
        team2Dropdown.AddOptions(options);

        team1Dropdown.value = 1; // Red Devils
        team2Dropdown.value = 2; // Blue Eagles
    }

    #region Panel Navigation

    public void ShowTeamSelection()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (teamSelectPanel != null) teamSelectPanel.SetActive(true);
        if (teamCustomizePanel != null) teamCustomizePanel.SetActive(false);
    }

    public void ShowTeamCustomization(bool isTeam1)
    {
        editingTeam1 = isTeam1;
        currentEditingTeam = isTeam1 ? teamBuilder.GetTeam1Data() : teamBuilder.GetTeam2Data();

        if (teamSelectPanel != null) teamSelectPanel.SetActive(false);
        if (teamCustomizePanel != null) teamCustomizePanel.SetActive(true);

        LoadTeamDataToUI(currentEditingTeam);
    }

    public void BackToTeamSelection()
    {
        ShowTeamSelection();
        SaveCurrentTeam();
    }

    #endregion

    #region Team Customization

    void LoadTeamDataToUI(TeamData team)
    {
        if (team == null) return;

        // Load team name
        if (teamNameInput != null)
        {
            teamNameInput.text = team.teamName;
        }

        // Load colors
        if (primaryColorPreview != null)
        {
            primaryColorPreview.color = team.primaryColor;
        }
        if (secondaryColorPreview != null)
        {
            secondaryColorPreview.color = team.secondaryColor;
        }

        // Load color sliders
        if (primaryColorR != null) primaryColorR.value = team.primaryColor.r;
        if (primaryColorG != null) primaryColorG.value = team.primaryColor.g;
        if (primaryColorB != null) primaryColorB.value = team.primaryColor.b;

        if (secondaryColorR != null) secondaryColorR.value = team.secondaryColor.r;
        if (secondaryColorG != null) secondaryColorG.value = team.secondaryColor.g;
        if (secondaryColorB != null) secondaryColorB.value = team.secondaryColor.b;

        // Load formation
        if (formationDropdown != null)
        {
            formationDropdown.value = (int)team.formation;
        }

        // Load player list
        PopulatePlayerList(team);
    }

    void PopulatePlayerList(TeamData team)
    {
        // Clear existing items
        if (playerListContent == null) return;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Create player list items
        for (int i = 0; i < team.players.Count; i++)
        {
            PlayerData player = team.players[i];

            GameObject item = CreatePlayerListItem(player, i);
            if (item != null)
            {
                item.transform.SetParent(playerListContent, false);
            }
        }
    }

    GameObject CreatePlayerListItem(PlayerData player, int index)
    {
        // Create a simple list item
        GameObject item = new GameObject($"PlayerItem_{index}");
        item.AddComponent<RectTransform>();

        // Add background
        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(item.transform);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = $"#{player.jerseyNumber} {player.playerName} - {player.GetPositionAbbreviation()} (OVR: {player.GetOverallRating()})";
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);

        return item;
    }

    public void OnTeamNameChanged()
    {
        if (currentEditingTeam != null && teamNameInput != null)
        {
            teamBuilder.CustomizeTeamName(currentEditingTeam, teamNameInput.text);
        }
    }

    public void OnPrimaryColorChanged()
    {
        if (currentEditingTeam != null)
        {
            Color newColor = new Color(
                primaryColorR != null ? primaryColorR.value : 1f,
                primaryColorG != null ? primaryColorG.value : 0f,
                primaryColorB != null ? primaryColorB.value : 0f
            );

            currentEditingTeam.primaryColor = newColor;
            if (primaryColorPreview != null)
            {
                primaryColorPreview.color = newColor;
            }
        }
    }

    public void OnSecondaryColorChanged()
    {
        if (currentEditingTeam != null)
        {
            Color newColor = new Color(
                secondaryColorR != null ? secondaryColorR.value : 1f,
                secondaryColorG != null ? secondaryColorG.value : 1f,
                secondaryColorB != null ? secondaryColorB.value : 1f
            );

            currentEditingTeam.secondaryColor = newColor;
            if (secondaryColorPreview != null)
            {
                secondaryColorPreview.color = newColor;
            }
        }
    }

    public void OnFormationChanged()
    {
        if (currentEditingTeam != null && formationDropdown != null)
        {
            currentEditingTeam.formation = (FormationType)formationDropdown.value;
        }
    }

    public void RandomizeTeam()
    {
        if (currentEditingTeam != null)
        {
            teamBuilder.RandomizeTeam(currentEditingTeam);
            LoadTeamDataToUI(currentEditingTeam);
        }
    }

    void SaveCurrentTeam()
    {
        if (currentEditingTeam != null)
        {
            if (editingTeam1)
            {
                teamBuilder.SetTeam1Data(currentEditingTeam);
            }
            else
            {
                teamBuilder.SetTeam2Data(currentEditingTeam);
            }

            teamBuilder.SaveTeams();
        }
    }

    #endregion

    #region Match Control

    public void OnTeam1DropdownChanged()
    {
        if (team1Dropdown == null) return;

        int index = team1Dropdown.value - 1; // -1 because first option is "Custom Team"

        if (index < 0)
        {
            // Custom team - keep current
        }
        else
        {
            TeamData selectedTeam = teamBuilder.GetPresetTeam(index);
            if (selectedTeam != null)
            {
                teamBuilder.SetTeam1Data(selectedTeam);
            }
        }
    }

    public void OnTeam2DropdownChanged()
    {
        if (team2Dropdown == null) return;

        int index = team2Dropdown.value - 1;

        if (index < 0)
        {
            // Custom team - keep current
        }
        else
        {
            TeamData selectedTeam = teamBuilder.GetPresetTeam(index);
            if (selectedTeam != null)
            {
                teamBuilder.SetTeam2Data(selectedTeam);
            }
        }
    }

    public void StartMatch()
    {
        SaveCurrentTeam();
        teamBuilder.SaveTeams();

        // Load game scene
        SceneManager.LoadScene("GameScene"); // Change to your game scene name
    }

    public void QuitToMenu()
    {
        SaveCurrentTeam();
        teamBuilder.SaveTeams();

        // Load menu scene
        SceneManager.LoadScene("MainMenu"); // Change to your menu scene name
    }

    #endregion
}
