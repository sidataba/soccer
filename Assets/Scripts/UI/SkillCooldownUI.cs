using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays skill cooldowns for a player
/// Simple UI showing which skills are ready
/// </summary>
public class SkillCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSkills playerSkills;
    [SerializeField] private int playerIndex = 0;

    [Header("UI Elements")]
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private Image[] skillIcons;
    [SerializeField] private Image[] cooldownOverlays;
    [SerializeField] private TextMeshProUGUI[] skillLabels;

    [Header("Colors")]
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color cooldownColor = Color.gray;

    private SkillType[] skills = new SkillType[]
    {
        SkillType.SpeedBoost,
        SkillType.SkillMove,
        SkillType.SlideTackle
    };

    void Start()
    {
        // Auto-find player skills if not assigned
        if (playerSkills == null)
        {
            PlayerSkills[] allSkills = FindObjectsOfType<PlayerSkills>();
            if (allSkills.Length > playerIndex)
            {
                playerSkills = allSkills[playerIndex];
            }
        }

        SetupUI();
    }

    void Update()
    {
        if (playerSkills != null)
        {
            UpdateSkillDisplay();
        }
    }

    private void SetupUI()
    {
        // Create simple UI if not already set up
        if (skillPanel == null)
        {
            CreateSimpleUI();
        }
    }

    private void CreateSimpleUI()
    {
        // Create a simple panel with skill indicators
        skillPanel = new GameObject($"SkillUI_Player{playerIndex + 1}");
        skillPanel.transform.SetParent(transform);

        RectTransform panelRect = skillPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10 + (playerIndex * 200), -100);
        panelRect.sizeDelta = new Vector2(180, 100);

        // Create skill slots
        skillIcons = new Image[3];
        cooldownOverlays = new Image[3];
        skillLabels = new TextMeshProUGUI[3];

        for (int i = 0; i < 3; i++)
        {
            CreateSkillSlot(i, panelRect);
        }
    }

    private void CreateSkillSlot(int index, RectTransform parent)
    {
        GameObject slot = new GameObject($"Skill{index}");
        slot.transform.SetParent(parent);

        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0, 1);
        slotRect.anchorMax = new Vector2(0, 1);
        slotRect.pivot = new Vector2(0, 1);
        slotRect.anchoredPosition = new Vector2(5, -5 - (index * 30));
        slotRect.sizeDelta = new Vector2(170, 25);

        // Background
        Image bg = slot.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.5f);

        // Cooldown overlay
        GameObject overlayObj = new GameObject("Cooldown");
        overlayObj.transform.SetParent(slot.transform);
        RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;

        cooldownOverlays[index] = overlayObj.AddComponent<Image>();
        cooldownOverlays[index].color = cooldownColor;
        cooldownOverlays[index].type = Image.Type.Filled;
        cooldownOverlays[index].fillMethod = Image.FillMethod.Horizontal;
        cooldownOverlays[index].fillOrigin = (int)Image.OriginHorizontal.Left;

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(slot.transform);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;
        labelRect.anchoredPosition = Vector2.zero;

        skillLabels[index] = labelObj.AddComponent<TextMeshProUGUI>();
        skillLabels[index].text = GetSkillName(skills[index]);
        skillLabels[index].fontSize = 14;
        skillLabels[index].alignment = TextAlignmentOptions.Center;
        skillLabels[index].color = Color.white;

        skillIcons[index] = bg;
    }

    private void UpdateSkillDisplay()
    {
        for (int i = 0; i < skills.Length && i < cooldownOverlays.Length; i++)
        {
            bool isReady = playerSkills.IsSkillReady(skills[i]);
            float cooldownPercent = playerSkills.GetSkillCooldownPercent(skills[i]);

            if (cooldownOverlays[i] != null)
            {
                cooldownOverlays[i].fillAmount = 1f - cooldownPercent;
                cooldownOverlays[i].color = isReady ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.5f);
            }

            if (skillLabels[i] != null)
            {
                skillLabels[i].color = isReady ? readyColor : Color.gray;
            }
        }
    }

    private string GetSkillName(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.SpeedBoost: return "BOOST (Q)";
            case SkillType.SkillMove: return "SKILL (F)";
            case SkillType.SlideTackle: return "TACKLE (C)";
            default: return skill.ToString();
        }
    }

    public void SetPlayerSkills(PlayerSkills skills)
    {
        playerSkills = skills;
    }
}
