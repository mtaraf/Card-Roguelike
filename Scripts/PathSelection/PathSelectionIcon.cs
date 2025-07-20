using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PathSelectionIcon : MonoBehaviour
{
    [SerializeField] private EncounterReward encounterReward;
    [SerializeField] private int rewardValue;
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

        Sprite sprite = null;

        // TO-DO: Set icon image and tooltip massage/title
        switch (type)
        {
            case EncounterType.Forge:
                sprite = Resources.Load<Sprite>("UI/Icons/forge_icon");
                break;
            case EncounterType.Regular_Encounter:
                break;
            case EncounterType.Mini_Boss_Encounter:
                sprite = Resources.Load<Sprite>("UI/Icons/mini_boss_icon");
                break;
            case EncounterType.Final_Boss:
                break;
        }

        if (sprite != null)
        {
            iconImage.sprite = sprite;
        }

        iconButton.onClick.RemoveAllListeners();
        iconButton.onClick.AddListener(() => PathSelectionSceneController.instance.navigateToScene(type, encounterReward, rewardValue));
    }
}
