using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseLevelSceneController : MonoBehaviour
{
    public static BaseLevelSceneController instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> enemyPrefabs;

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
        randomizeEnemy();

        // Start turns
        TurnManager.instance.Initialize(player, enemies);
        TurnManager.instance.startTurns();
    }

    // Randomize the enemy the player encounters
    void randomizeEnemy()
    {
        int currentLevel = GameManager.instance.getCurrentLevel();
        if (currentLevel % 10 == 0 && currentLevel != 0)
        {
            // Load Boss level
        }
        else
        {
            // Random common enemy
            int random = Random.Range(0, enemyPrefabs.Count - 1);
            GameObject enemyObj = Instantiate(enemyPrefabs[random], mainCanvas.transform);

            // TO-DO: remove enemy from list once there are enough enemies
            // enemyPrefabs.Remove(enemyPrefabs[random]);

            // Get enemy component
            enemies.Add(enemyObj.GetComponent<Enemy>());

            // TO-DO: Add random events instead of common enemy
        }
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

    public void usePlayerEnergy(int value)
    {
        playerCurrentEnergy -= value;
        setPlayerEnergy(playerCurrentEnergy);
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
}