using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MiniBossSceneController : ParentSceneController
{
    public static MiniBossSceneController miniBossSceneController;
    private List<GameObject> miniBosses;

    // UI
    private BaseLevelUIController baseLevelUIController;

    public override void Start()
    {
        baseLevelUIController = transform.AddComponent<BaseLevelUIController>();
        InitializeScene();
    }

    void Awake()
    {
        if (miniBossSceneController != null && miniBossSceneController != this)
        {
            Destroy(gameObject);
            return;
        }

        miniBossSceneController = this;
    }

    public override void InitializeScene()
    {
        GameManager.instance.setCurrentSceneController(miniBossSceneController);
        base.InitializeScene();

        baseLevelUIController.Initialize();

        // Get Enemy Group prefabs
        miniBosses = Resources.LoadAll<GameObject>("CharacterPrefabs/Bosses/Mini-bosses/Prefabs").ToList();

        chooseMiniBossAndStartTurnSequence();
    }

    void chooseMiniBossAndStartTurnSequence()
    {
        GameObject miniBossObj = miniBosses[Random.Range(0,miniBosses.Count)];

        GameObject miniBoss = Instantiate(miniBossObj, enemySpawnLocation.transform);
        if (miniBoss == null)
        {
            Debug.LogError($"Error instatiating mini boss: {miniBossObj.name}");
        }

        Enemy miniBossComponent = miniBoss.transform.GetChild(0).GetComponent<Enemy>();
        if (miniBossComponent == null)
        {
            Debug.LogError($"Error finding enemy component on {miniBoss.name}");
        }
        enemies.Add(miniBossComponent);

        TurnManager.instance.Initialize(player, enemies);
        TurnManager.instance.startTurns();
    }

}