using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BaseLevelSceneController : ParentSceneController
{
    public static BaseLevelSceneController instance;
    [SerializeField] private List<EnemyGroup> enemyPrefabs;

    // UI
    private BaseLevelUIController baseLevelUIController;

    public override void Start()
    {
        baseLevelUIController = transform.AddComponent<BaseLevelUIController>();
        InitializeScene();
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

    public override void InitializeScene()
    {
        GameManager.instance.setCurrentSceneController(instance);
        base.InitializeScene();

        baseLevelUIController.Initialize();

        // Get Enemy Group prefabs

        // Single Enemies
        foreach (GameObject singleEnemy in Resources.LoadAll<GameObject>("CharacterPrefabs/Enemies/ES1"))
        {
            EnemyGroup enemyGroup = new EnemyGroup();
            enemyGroup.size = 1;
            enemyGroup.enemyParentObj = singleEnemy;
            enemyPrefabs.Add(enemyGroup);
        }

        // Two Enemies (uncomment later)
        foreach (GameObject enemies in Resources.LoadAll<GameObject>("CharacterPrefabs/Enemies/ES2"))
        {
            EnemyGroup enemyGroup = new EnemyGroup();
            enemyGroup.size = 2;
            enemyGroup.enemyParentObj = enemies;
            enemyPrefabs.Add(enemyGroup);
        }

        // Randomize enemies enemies
        randomizeEnemyAndStartTurnSequence();
    }

    // Randomize the enemy the player encounters
    void randomizeEnemyAndStartTurnSequence()
    {
        int currentLevel = GameManager.instance.getCurrentLevel();
        if (currentLevel % 10 == 0 && currentLevel != 0)
        {
            // Load Boss level
        }
        else
        {
            int randomSize = Random.Range(1, 2); // TO-DO: change back to 1,4 after testing and have encounter with multiple enemies 1/2/3
            List<EnemyGroup> enemyGroups = enemyPrefabs.FindAll((group) => group.size == randomSize);

            int randomGroup = Random.Range(0, enemyGroups.Count);
            EnemyGroup enemyGroup = enemyGroups[randomGroup];

            for (int i = 0; i < enemyGroup.enemyParentObj.transform.childCount; i++)
            {
                GameObject enemy = spawnEnemy(enemyGroup.enemyParentObj.transform.GetChild(i).gameObject, randomSize - 1);
                if (enemy == null)
                {
                    Debug.Log($" Problem instantiating {enemy.name}!");
                }

                Enemy enemyComponent = enemy.GetComponent<Enemy>();

                if (enemyComponent == null)
                {
                    Debug.LogError($"Instantiated enemy {enemy.name} has no Enemy component!");
                }
                else
                {
                    enemyComponent.setMaxHealth(50); // For Dev Testing
                    enemies.Add(enemyComponent);
                }
            }

            // TO-DO (Beta): remove enemy from list once there are enough enemies
            // enemyPrefabs.Remove(enemyPrefabs[random]);

            // Get enemy component

            // TO-DO (Beta): Add random events instead of common enemy

            // Start turns
            TurnManager.instance.Initialize(player, enemies);
            TurnManager.instance.startTurns();
        }
    }

    GameObject spawnEnemy(GameObject enemyObj, int numEnemies)
    {
        // for (int i = 0; i < spawnLocationParent.transform.childCount; i++)
        // {
        //     if (spawnLocationParent.transform.GetChild(i).childCount == 0)
        //     {
        //         return Instantiate(enemyObj, spawnLocationParent.transform.GetChild(i).transform);
        //     }
        // }
        GameObject enemy = Instantiate(enemyObj, enemySpawnLocation.transform);
        enemy.transform.localPosition = new Vector2(-50,0);
        return enemy;
    }

    public override void updateDrawPile(int count)
    {
        baseLevelUIController.updateDrawPile(count);
    }

    public override void updateDiscardPile(int count)
    {
        baseLevelUIController.updateDiscardPile(count);
    }
}

[System.Serializable]
public class EnemyGroup
{
    public int size;
    public GameObject enemyParentObj;
}