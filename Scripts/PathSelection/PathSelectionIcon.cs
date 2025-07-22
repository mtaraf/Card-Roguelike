using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PathSelectionIcon : MonoBehaviour
{
    [SerializeField] private EncounterReward encounterReward;
    [SerializeField] private int rewardValue;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private GameObject rewardContainer;
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

    public IEnumerator instantiateIcon(EncounterType type, bool completed, EncounterReward reward, int nodeId)
    {
        // Wait for Start() to finish
        yield return null;

        if (type == EncounterType.Forge || type == EncounterType.Final_Boss)
        {
            deleteRewardIcon();
        }

        if (EncounterData.InfoMap.TryGetValue(type, out var info))
        {
            iconImage.sprite = Resources.Load<Sprite>(info.iconPath);
            tooltip.setTooltipData(info.title, info.message);
        }

        
        iconButton.onClick.RemoveAllListeners();
        iconButton.onClick.AddListener(() =>
            PathSelectionSceneController.instance.navigateToScene(nodeId));

        updateRewardIcon(reward);
    }

    void updateRewardIcon(EncounterReward reward)
    {
        switch (reward)
        {
            case EncounterReward.CardRarity:
                rewardIcon.sprite = Resources.Load<Sprite>("UI/Icons/rarity_icon");
                break;
            case EncounterReward.CardChoices:
                rewardIcon.sprite = Resources.Load<Sprite>("UI/Icons/card_choices_icon");
                break;
            case EncounterReward.Gold:
                rewardIcon.sprite = Resources.Load<Sprite>("UI/Icons/gold_icon");
                break;
        }
    }

    void deleteRewardIcon()
    {
        Destroy(rewardContainer);
    }
}
