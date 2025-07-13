using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseLevelSceneController : MonoBehaviour
{
    public static BaseLevelSceneController instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<EnemyGroup> enemyPrefabs;
    [SerializeField] private List<GameObject> enemySpawnLocations;

    private GameObject mainCanvas;
    private GameObject playerEnergyUI;

    private Player player;
    private int playerCurrentEnergy = 0;

    private List<Enemy> enemies = new List<Enemy>();

    void Start()
    {
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

    void InitializeScene()
    {
        mainCanvas = GameObject.FindGameObjectWithTag("BaseLevelCanvas");

        // Get Enemy Group prefabs
        foreach (GameObject singleEnemy in Resources.LoadAll<GameObject>("EnemyPrefabs/Regular/ES1"))
        {
            EnemyGroup enemyGroup = new EnemyGroup();
            enemyGroup.size = 1;
            enemyGroup.enemyParentObj = singleEnemy;
            enemyPrefabs.Add(enemyGroup);
        }

        // Spawn player
        GameObject playerObj = Instantiate(playerPrefab, mainCanvas.transform);
        player = playerObj.GetComponent<Player>();

        player.setDeck(GameManager.instance.getPlayerDeck());
        player.setMaxHealth(GameManager.instance.getPlayerMaxHealth());
        player.setCurrentHealth(GameManager.instance.getPlayerCurrentHealth());

        // Find energy UI
        playerEnergyUI = Helpers.findDescendant(mainCanvas.transform, "EnergyUI");
        playerCurrentEnergy = GameManager.instance.getPlayerHandEnergy();
        setPlayerEnergy(playerCurrentEnergy);

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
            int randomSize = Random.Range(1, 2); // TO-DO: change back to 1,5 after testing
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
                    enemies.Add(enemyComponent);
                }
            }

            // foreach (GameObject enemyObj in enemyGroup.enemyParentObj.transform.GetChild())
            // {
            //     GameObject enemy = spawnEnemy(enemyObj, randomSize - 1);
            //     if (enemy == null)
            //     {
            //         Debug.Log($" Problem instantiating {enemy.name}!");
            //     }

            //     Enemy enemyComponent = enemy.GetComponent<Enemy>();

            //     if (enemyComponent == null)
            //     {
            //         Debug.LogError($"Instantiated enemy {enemy.name} has no Enemy component!");
            //     }
            //     else
            //     {
            //         enemies.Add(enemyComponent);
            //     }
            // }

            // TO-DO: remove enemy from list once there are enough enemies
            // enemyPrefabs.Remove(enemyPrefabs[random]);

            // Get enemy component

            // TO-DO: Add random events instead of common enemy

            // Start turns
            TurnManager.instance.Initialize(player, enemies);
            TurnManager.instance.startTurns();
        }
    }

    GameObject spawnEnemy(GameObject enemyObj, int numEnemies)
    {
        GameObject spawnLocationParent = enemySpawnLocations[numEnemies];
        GameObject enemy = null;
        for (int i = 0; i < spawnLocationParent.transform.childCount; i++)
        {
            if (spawnLocationParent.transform.GetChild(i).childCount == 0)
            {
                return Instantiate(enemyObj, spawnLocationParent.transform.GetChild(i).transform);
            }
        }

        return enemy;
    }

    public void removeDeadEnemy(int id)
    {
        Enemy enemy = enemies.Find((enemy) => enemy.getId() == id);
        if (enemy != null)
        {
            enemies.Remove(enemy);
        }
        else
        {
            Debug.LogError("Could not find enemy for id: " + id);
        }
    }

    public void setPlayerEnergy(int energy)
    {
        playerEnergyUI.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>().text = energy.ToString();
    }

    public void clearPlayerNegativeEffects()
    {
        player.clearAllNegativeEffects();
    }

    public void usePlayerEnergy(int value)
    {
        playerCurrentEnergy -= value;
        setPlayerEnergy(playerCurrentEnergy);
    }

    public void resetPlayerEnergy()
    {
        playerCurrentEnergy = GameManager.instance.getPlayerHandEnergy();
        setPlayerEnergy(playerCurrentEnergy);
    }

    public int getCurrentPlayerEnergy()
    {
        return playerCurrentEnergy;
    }

    public Dictionary<EffectType, int> getPlayerAttributes()
    {
        return player.getAttributes();
    }

    public void playAnimationsForCard(CardType type)
    {
        player.playAnimation(type);
    }

    public void processEnemyCardEffectsOnPlayer(List<CardEffect> effects)
    {
        player.processCardEffects(effects);
    }

    public List<Enemy> getEnemies()
    {
        return enemies;
    }
}

[System.Serializable]
public class EnemyGroup
{
    public int size;
    public GameObject enemyParentObj;
}