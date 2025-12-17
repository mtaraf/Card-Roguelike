using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class EncounterRewardExtensions
{
    public static string ToDisplayString(this EncounterReward eff)
    {
        switch (eff)
        {
            case EncounterReward.CardRarity: return "Card Rarity";
            case EncounterReward.CardChoices: return "+1 Card Choice";
            case EncounterReward.Gold: return "Gold";
            case EncounterReward.MaxHealth: return "Max Health";
            default: return "Default";
        }
    }
}

public class PathSelectionSceneController : MonoBehaviour
{
    public static PathSelectionSceneController instance;
    private int currentLevel;
    private PathSelectionUIController pathSelectionUIController;
    private List<PathOptionData> options = new List<PathOptionData>();
    private List<GameObject> doorButtons = new List<GameObject>();
    private Transform playerDisplayLocation;

    // Base rewards
    public Tuple<EncounterReward, int> baseGoldReward = new Tuple<EncounterReward, int>(EncounterReward.Gold, 10);
    public Tuple<EncounterReward, int> baseMaxHealthReward = new Tuple<EncounterReward, int>(EncounterReward.MaxHealth, 5);
    public Tuple<EncounterReward, int> baseCardChoicesReward = new Tuple<EncounterReward, int>(EncounterReward.CardChoices, 1);
    public Tuple<EncounterReward, int> baseCardRarityReward = new Tuple<EncounterReward, int>(EncounterReward.CardRarity, 5); // TO-DO: Might be too high

    // Path Selection Chances
    private List<Tuple<int, EncounterType>> encounters_three_to_ten = new List<Tuple<int, EncounterType>>
    {
        new Tuple<int, EncounterType>(8, EncounterType.Regular_Encounter),
        new Tuple<int, EncounterType>(9, EncounterType.Culver_Encounter),
        new Tuple<int, EncounterType>(10, EncounterType.Hold_The_Line_Encounter)
    };

     private List<Tuple<int, EncounterType>> encounters_ten_to_twenty = new List<Tuple<int, EncounterType>>
    {
        new Tuple<int, EncounterType>(6, EncounterType.Regular_Encounter),
        new Tuple<int, EncounterType>(8, EncounterType.Mini_Boss_Encounter),
        new Tuple<int, EncounterType>(9, EncounterType.Culver_Encounter),
        new Tuple<int, EncounterType>(9, EncounterType.Hold_The_Line_Encounter)
    };


    void Start()
    {
        pathSelectionUIController = FindFirstObjectByType<PathSelectionUIController>();
        doorButtons.Add(GameObject.Find("DoorOne"));
        doorButtons.Add(GameObject.Find("DoorTwo"));
        doorButtons.Add(GameObject.Find("DoorThree"));

        playerDisplayLocation = GameObject.Find("PlayerDisplayPosition").transform;

        GameObject playerDisplay = GameManager.instance.getPlayerDisplayObject();
        // For testing purposes, TO-DO: Replace with error checking later
        if (playerDisplay == null)
        {
            Debug.LogError("Player display is null, using default");
            playerDisplay = Instantiate(Resources.Load<GameObject>("CharacterPrefabs/CharacterDisplays/READY/MistbornDisplay"), playerDisplayLocation);
        }
        else
        {
            playerDisplay = Instantiate(GameManager.instance.getPlayerDisplayObject(), playerDisplayLocation);
        }
        playerDisplay.GetComponent<DisplayInformation>().pathSelectionAlignment();

        if (GameManager.instance.getPreviousSceneNumber() == 0)
        {
            options = GameManager.instance.getPathOptions();
            currentLevel = GameManager.instance.getCurrentLevel();
        }
        else
        {
            GameManager.instance.incrementCurrentLevel();
            currentLevel = GameManager.instance.getCurrentLevel();
            options.Add(generateOption());
            options.Add(generateOption());
            options.Add(generateOption());
            GameManager.instance.setPathOptions(options);
        }

        StartCoroutine(assignDoorButtonAction());

        pathSelectionUIController.Initialize(options);
    }

    IEnumerator assignDoorButtonAction()
    {
        yield return null;
        for (int i=0; i<doorButtons.Count; i++)
        {
            int index = i;
            doorButtons[index].GetComponent<Button>().onClick.AddListener(() => moveToEncounter(options[index]));
        }
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

    public void moveToEncounter(PathOptionData option)
    {
        GameManager.instance.loadEncounterTypeAndRewards(option.encounterType, option.encounterReward, option.rewardValue);
    }

    private PathOptionData generateOption()
    {
        int random = UnityEngine.Random.Range(0, 11);
        PathOptionData option;
        Tuple<EncounterReward,int> encounterReward;

        if (currentLevel < 3)
        {
            encounterReward = generateRandomReward(EncounterType.Regular_Encounter);
            option = new PathOptionData(EncounterType.Regular_Encounter, encounterReward.Item1, encounterReward.Item2);
        }
        else if (currentLevel < 10)
        {
            Tuple<int, EncounterType> encounterInfo = encounters_three_to_ten.First(e => random < e.Item1);
            encounterReward = generateRandomReward(encounterInfo.Item2);
            option = new PathOptionData(encounterInfo.Item2, encounterReward.Item1, encounterReward.Item2);
        }
        else
        {
            Tuple<int, EncounterType> encounterInfo = encounters_ten_to_twenty.First(e => random < e.Item1);
            encounterReward = generateRandomReward(encounterInfo.Item2);
            option = new PathOptionData(encounterInfo.Item2, encounterReward.Item1, encounterReward.Item2);
        }

        return option;
    }
    
    Tuple<EncounterReward,int> generateRandomReward(EncounterType encounterType)
    {
        int random = UnityEngine.Random.Range(0, 101);

        if (encounterType == EncounterType.Mini_Boss_Encounter)
        {
            // Specific mini-boss rewards
            return new Tuple<EncounterReward, int>(EncounterReward.Gold, 0);
        }

        if (random < 71)
        {
            return new Tuple<EncounterReward, int>(baseGoldReward.Item1, baseGoldReward.Item2 * currentLevel);
        }
        else
        {
            EncounterReward rewardType = Helpers.GetRandomEnumValue<EncounterReward>();
            switch (rewardType)
            {
                case EncounterReward.CardChoices:
                    return new Tuple<EncounterReward, int>(baseCardChoicesReward.Item1, baseGoldReward.Item2);
                case EncounterReward.CardRarity:
                    return new Tuple<EncounterReward, int>(baseCardRarityReward.Item1, baseGoldReward.Item2);
                case EncounterReward.Gold:
                    return new Tuple<EncounterReward, int>(baseGoldReward.Item1, baseGoldReward.Item2 * currentLevel);
                case EncounterReward.MaxHealth:
                    return new Tuple<EncounterReward, int>(baseMaxHealthReward.Item1, baseMaxHealthReward.Item2 * currentLevel);
                default:
                    return new Tuple<EncounterReward, int>(EncounterReward.Gold, 0);
            }
        }
    }
}
