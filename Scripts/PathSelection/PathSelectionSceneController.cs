using System.Collections.Generic;
using UnityEngine;


public enum EncounterType
{
    Forge,
    Regular_Encounter,
    Mini_Boss_Encounter,
    Culver_Encounter,
    Tank_Encounter,
    Final_Boss
}

public enum EncounterReward
{
    CardRarity,
    CardChoices,
    Gold,
}

public static class EncounterRewardExtensions
{
    public static string ToDisplayString(this EncounterReward eff)
    {
        switch (eff)
        {
            case EncounterReward.CardRarity: return "+25% Card Rarity";
            case EncounterReward.CardChoices: return "+1 Card Choice";
            case EncounterReward.Gold: return "+25 Gold";
            default: return "Default";
        }
    }
}

public class PathSelectionSceneController : MonoBehaviour
{
    public static PathSelectionSceneController instance;

    [SerializeField] private EncounterMap map;
    private int levels = 10;

    private PathSelectionUIController pathSelectionUIController;


    void Start()
    {
        pathSelectionUIController = FindFirstObjectByType<PathSelectionUIController>();
        pathSelectionUIController.Initialize();

        map = GameManager.instance.getEncounterMap();

        // Check if on a current map, if not create one
        if (map != null && map.nodes.Count > 0)
        {
            map.rebuildPaths();
            Debug.Log("Map found!");
        }
        else
        {
            map = new EncounterMap();
            Debug.Log("No encounter map present!");
            map.generateRandomMap(levels);
            GameManager.instance.setEncounterMap(map);
        }

        pathSelectionUIController.fillUIWithCurrentEncounterMap(map, levels);
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void navigateToScene(int id)
    {
        map.currentEncounterId = id;
        Debug.Log("Current Encounter Level ID: " + id);
        EncounterType type = map.getNode(id).type;
        switch (type)
        {
            case EncounterType.Forge:
                //SceneLoader.instance.loadScene(3);
                break;
            case EncounterType.Regular_Encounter:
                SceneLoader.instance.loadScene("BaseLevelScene", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
            case EncounterType.Mini_Boss_Encounter:
                SceneLoader.instance.loadScene("BaseLevelScene", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
            case EncounterType.Final_Boss:
                SceneLoader.instance.loadScene("BaseLevelScene", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
        }
    }

    public void setEncounterReward()
    {

    }
}
