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

public class PathSelectionSceneController : MonoBehaviour
{
    public static PathSelectionSceneController instance;

    [SerializeField] private EncounterMap map = new EncounterMap();
    private int levels = 10;

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
