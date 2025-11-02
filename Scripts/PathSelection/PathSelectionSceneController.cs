using System;
using System.Collections.Generic;
using UnityEngine;


public enum EncounterType
{
    Forge,
    Regular_Encounter,
    Mini_Boss_Encounter,
    Culver_Encounter,
    Hold_The_Line_Encounter,
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
    private int currentLevel = 1;

    private PathSelectionUIController pathSelectionUIController;
    private List<Tuple<EncounterType, EncounterReward>> options = new List<Tuple<EncounterType, EncounterReward>>();


    void Start()
    {
        pathSelectionUIController = FindFirstObjectByType<PathSelectionUIController>();

        options.Add(generateOption());
        if (currentLevel % 5 != 4)
        {
            options.Add(generateOption());
            options.Add(generateOption());
        }

        pathSelectionUIController.Initialize(options);
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
                // TODO: Change this
                SceneLoader.instance.loadScene("BaseLevelScene", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
            case EncounterType.Final_Boss:
                // TODO: Change this
                SceneLoader.instance.loadScene("BaseLevelScene", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
            case EncounterType.Culver_Encounter:
                SceneLoader.instance.loadScene("CulverScene", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
            case EncounterType.Hold_The_Line_Encounter:
                SceneLoader.instance.loadScene("HoldTheLine", () => GameManager.instance.loadEncounterTypeAndRewards(map));
                break;
        }
    }

    private Tuple<EncounterType, EncounterReward> generateOption()
    {
        int random = UnityEngine.Random.Range(0, 11);
        Tuple<EncounterType, EncounterReward> tuple;

        if (currentLevel < 3)
        {
            tuple = new Tuple<EncounterType, EncounterReward>(EncounterType.Regular_Encounter, generateRandomReward(EncounterType.Regular_Encounter));
        }
        else if (currentLevel < 6)
        {
            tuple = random switch
            {
                < 8 => new Tuple<EncounterType, EncounterReward>(EncounterType.Regular_Encounter, generateRandomReward(EncounterType.Regular_Encounter)),
                < 10 => new Tuple<EncounterType, EncounterReward>(EncounterType.Culver_Encounter, generateRandomReward(EncounterType.Culver_Encounter)),
                _ => new Tuple<EncounterType, EncounterReward>(EncounterType.Hold_The_Line_Encounter, generateRandomReward(EncounterType.Hold_The_Line_Encounter))
            };
        }
        else
        {
            tuple = random switch
            {
                < 8 => new Tuple<EncounterType, EncounterReward>(EncounterType.Regular_Encounter, generateRandomReward(EncounterType.Regular_Encounter)),
                < 9 => new Tuple<EncounterType, EncounterReward>(EncounterType.Culver_Encounter, generateRandomReward(EncounterType.Culver_Encounter)),
                < 10 => new Tuple<EncounterType, EncounterReward>(EncounterType.Hold_The_Line_Encounter, generateRandomReward(EncounterType.Hold_The_Line_Encounter)),
                10 => new Tuple<EncounterType, EncounterReward>(EncounterType.Mini_Boss_Encounter, generateRandomReward(EncounterType.Mini_Boss_Encounter)),
                _ => new Tuple<EncounterType, EncounterReward>(EncounterType.Final_Boss, generateRandomReward(EncounterType.Final_Boss))
            };
        }

        return tuple;
    }
    
    EncounterReward generateRandomReward(EncounterType encounterType)
    {

        int random = UnityEngine.Random.Range(0, 101);

        if (encounterType == EncounterType.Mini_Boss_Encounter)
        {
            // Specific mini-boss rewards
            return EncounterReward.Gold;
        }

        if (random < 71)
        {
            return EncounterReward.Gold;
        }
        else
        {
            return Helpers.GetRandomEnumValue<EncounterReward>();
        }
    }
}
