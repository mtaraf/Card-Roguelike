using UnityEngine;
using UnityEngine.UI;

public class PathSelectionIcon : MonoBehaviour
{
    [SerializeField] private EncounterType path;
    [SerializeField] private EncounterReward encounterReward;
    [SerializeField] private int rewardValue;
    private Button iconButton;
    private Image iconImage;
    private Tooltip tooltip;

    void Start()
    {
        iconButton = GetComponent<Button>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
        tooltip = GetComponent<Tooltip>();
    }

    void instantiateIcon()
    {
        // TO-DO: Set icon image and tooltip massage/title
        switch (path)
        {
            case EncounterType.Forge:
                break;
            case EncounterType.Regular_Encounter:
                break;
            case EncounterType.Mini_Boss_Encounter:
                break;
            case EncounterType.Final_Boss:
                break;
        }
        iconButton.onClick.RemoveAllListeners();
        iconButton.onClick.AddListener(() => PathSelectionSceneController.instance.navigateToScene(path, encounterReward, rewardValue));
    }
}
