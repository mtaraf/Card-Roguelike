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

    [SerializeField] private List<EncounterMap> encounterMaps = new List<EncounterMap>();

    // UI
    private GameObject scrollViewBaseUI;


    void Start()
    {
        scrollViewBaseUI = GameObject.FindGameObjectWithTag("PathSelectionContent");
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

    void instantiatePath()
    {
        
    }

    public void navigateToScene(EncounterType path, EncounterReward encounterReward, int rewardValue)
    {
        switch (path)
        {
            case EncounterType.Forge:
                SceneLoader.instance.loadScene(2);
                break;
            case EncounterType.Regular_Encounter:
                SceneLoader.instance.loadScene(1, () => GameManager.instance.setEncounterTypeAndRewards(path, encounterReward, rewardValue));
                break;
            case EncounterType.Mini_Boss_Encounter:
                SceneLoader.instance.loadScene(1, () => GameManager.instance.setEncounterTypeAndRewards(path, encounterReward, rewardValue));
                break;
            case EncounterType.Final_Boss:
                SceneLoader.instance.loadScene(1, () => GameManager.instance.setEncounterTypeAndRewards(path, encounterReward, rewardValue));
                break;
        }
    }

    public void setEncounterReward()
    {

    }
}
