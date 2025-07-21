using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PathSelectionIcon : MonoBehaviour
{
    [SerializeField] private EncounterReward encounterReward;
    [SerializeField] private int rewardValue;
    [SerializeField] private Image rewardIcon;
    private Button iconButton;
    private Image iconImage;
    private Image completedOutline; // TO-DO: add this and enable if the path has been completed
    private Tooltip tooltip;

    void Start()
    {
        iconButton = GetComponent<Button>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
        tooltip = GetComponent<Tooltip>();
    }

    public IEnumerator instantiateIcon(EncounterType type, bool completed)
    {
        // Wait for Start() to finish
        yield return null;

        Sprite encounterSprite = null;
        string tooltipTitle = "";
        string tooltipMessage = "";

        // TO-DO: Set icon image and tooltip massage/title
        switch (type)
        {
            case EncounterType.Forge:
                encounterSprite = Resources.Load<Sprite>("UI/Icons/forge_icon");
                tooltipTitle = "The Forge";
                tooltipMessage = "Enter the forge to upgrade or remove cards in your deck";
                break;
            case EncounterType.Regular_Encounter:
                encounterSprite = Resources.Load<Sprite>("UI/Icons/regular_encounter_icon");
                tooltipTitle = "Encounter";
                tooltipMessage = "Battle a variety of enemies and claim the rewards!";
                break;
            case EncounterType.Mini_Boss_Encounter:
                encounterSprite = Resources.Load<Sprite>("UI/Icons/mini_boss_icon");
                tooltipTitle = "Mini Boss";
                tooltipMessage = "Fight against a strong opponent to obtain rare loot!";
                break;
            case EncounterType.Culver_Encounter:
                encounterSprite = Resources.Load<Sprite>("UI/Icons/culver_encounter_icon");
                tooltipTitle = "Culver's Bounty";
                tooltipMessage = "Your task is to unleash your might upon the mighty culver. Deal enough damage to incapacitate Culver to claim your bounty.";
                break;
            case EncounterType.Tank_Encounter:
                encounterSprite = Resources.Load<Sprite>("UI/Icons/tank_encounter_icon");
                tooltipTitle = "Hold the Line";
                tooltipMessage = "Survive the onslaught of barrages without fighting back to claim the rewards for your effort to outlast the damage.";
                break;
            case EncounterType.Final_Boss:
                break;
        }

        if (encounterSprite != null)
        {
            iconImage.sprite = encounterSprite;
        }

        iconButton.onClick.RemoveAllListeners();
        iconButton.onClick.AddListener(() => PathSelectionSceneController.instance.navigateToScene(type, encounterReward, rewardValue));
        tooltip.setTooltipData(tooltipTitle, tooltipMessage);
    }
}
