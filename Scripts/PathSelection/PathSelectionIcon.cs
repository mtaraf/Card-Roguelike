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
    private Image container; // TO-DO: add this and enable if the path has been completed
    private Tooltip tooltip;

    void Start()
    {
        iconButton = GetComponent<Button>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
        tooltip = GetComponent<Tooltip>();
        container = GetComponent<Image>();
    }

    public IEnumerator instantiateIcon(EncounterType type, bool completed, EncounterReward reward, int nodeId, bool clickable)
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
            string message = info.message + "\n\n" + reward.ToDisplayString();
            tooltip.setTooltipData(info.title, message);
        }

        if (completed)
        {
            container.sprite = Resources.Load<Sprite>("UI/Art/Icons/completed_icon_container");
        }

        iconButton.onClick.RemoveAllListeners();
        if (clickable)
        {
            iconButton.onClick.AddListener(() =>
            PathSelectionSceneController.instance.navigateToScene(nodeId));
        }

        updateRewardIcon(reward);
    }

    void updateRewardIcon(EncounterReward reward)
    {
        switch (reward)
        {
            case EncounterReward.CardRarity:
                rewardIcon.sprite = Resources.Load<Sprite>("UI/Art/Icons/rarity_icon");
                break;
            case EncounterReward.CardChoices:
                rewardIcon.sprite = Resources.Load<Sprite>("UI/Art/Icons/extra_card_icon");
                break;
            case EncounterReward.Gold:
                rewardIcon.sprite = Resources.Load<Sprite>("UI/Art/Icons/gold_icon");
                break;
        }
    }

    void deleteRewardIcon()
    {
        Destroy(rewardContainer);
    }
}
