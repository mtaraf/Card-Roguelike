using System.Collections.Generic;
using UnityEngine;


public enum EncounterType
{
    Forge,
    Regular_Encounter,
    Mini_Boss_Encounter,
    Final_Boss
}

public enum EncounterReward
{
    CardSelection,
    CardRarity,
    CardChoices,
    CardRemoval,
    Forge
}

public class PathSelectionSceneController : MonoBehaviour
{
    public static PathSelectionSceneController instance;

    [SerializeField] private EncounterMap map = new EncounterMap();
    [SerializeField] private int levels = 7;

    private PathSelectionUIController pathSelectionUIController;


    void Start()
    {
        pathSelectionUIController = FindFirstObjectByType<PathSelectionUIController>();
        pathSelectionUIController.Initialize();

        // Check if on a current map, if not create one
        if (GameManager.instance.getEncounterMap() != null)
        {
            map = GameManager.instance.getEncounterMap();
        }
        else
        {
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

    public void navigateToScene(EncounterType path, EncounterReward encounterReward, int rewardValue)
    {
        switch (path)
        {
            case EncounterType.Forge:
                //SceneLoader.instance.loadScene(2);
                break;
            case EncounterType.Regular_Encounter:
                //SceneLoader.instance.loadScene(1, () => GameManager.instance.setEncounterTypeAndRewards(path, encounterReward, rewardValue));
                break;
            case EncounterType.Mini_Boss_Encounter:
                //SceneLoader.instance.loadScene(1, () => GameManager.instance.setEncounterTypeAndRewards(path, encounterReward, rewardValue));
                break;
            case EncounterType.Final_Boss:
                //SceneLoader.instance.loadScene(1, () => GameManager.instance.setEncounterTypeAndRewards(path, encounterReward, rewardValue));
                break;
        }
    }

    public void setEncounterReward()
    {

    }
}
