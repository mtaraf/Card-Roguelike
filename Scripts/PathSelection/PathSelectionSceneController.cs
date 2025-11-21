using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    private int levels = 10;
    private int currentLevel;
    private PathSelectionUIController pathSelectionUIController;
    private List<PathOptionData> options = new List<PathOptionData>();
    private List<GameObject> doorButtons = new List<GameObject>();
    private Transform playerDisplayLocation;


    void Start()
    {
        pathSelectionUIController = FindFirstObjectByType<PathSelectionUIController>();
        doorButtons.Add(GameObject.Find("DoorOne"));
        doorButtons.Add(GameObject.Find("DoorTwo"));
        doorButtons.Add(GameObject.Find("DoorThree"));

        playerDisplayLocation = GameObject.Find("PlayerDisplayPosition").transform;

        GameObject playerDisplay;
        // For testing purposes, TO-DO: Replace with error checking later
        if (GameManager.instance.getPlayerDisplayObject() == null)
        {
            playerDisplay = Instantiate(Resources.Load<GameObject>("CharacterPrefabs/CharacterDisplays/MistbornDisplay"), playerDisplayLocation);
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
        GameManager.instance.loadEncounterTypeAndRewards(option.encounterType, option.encounterReward);
    }

    private PathOptionData generateOption()
    {
        int random = UnityEngine.Random.Range(0, 11);
        PathOptionData option;

        if (currentLevel < 3)
        {
            option = new PathOptionData(EncounterType.Regular_Encounter, generateRandomReward(EncounterType.Regular_Encounter));
            
        }
        else if (currentLevel < 6)
        {
            option = random switch
            {
                < 8 => new PathOptionData(EncounterType.Regular_Encounter, generateRandomReward(EncounterType.Regular_Encounter)),
                < 10 => new PathOptionData(EncounterType.Culver_Encounter, generateRandomReward(EncounterType.Culver_Encounter)),
                _ => new PathOptionData(EncounterType.Hold_The_Line_Encounter, generateRandomReward(EncounterType.Hold_The_Line_Encounter))
            };
        }
        else
        {
            option = random switch
            {
                < 8 => new PathOptionData(EncounterType.Regular_Encounter, generateRandomReward(EncounterType.Regular_Encounter)),
                < 9 => new PathOptionData(EncounterType.Culver_Encounter, generateRandomReward(EncounterType.Culver_Encounter)),
                < 10 => new PathOptionData(EncounterType.Hold_The_Line_Encounter, generateRandomReward(EncounterType.Hold_The_Line_Encounter)),
                10 => new PathOptionData(EncounterType.Mini_Boss_Encounter, generateRandomReward(EncounterType.Mini_Boss_Encounter)),
                _ => new PathOptionData(EncounterType.Final_Boss, generateRandomReward(EncounterType.Final_Boss))
            };
        }

        return option;
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
