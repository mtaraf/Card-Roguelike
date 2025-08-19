using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CulverSceneController : MonoBehaviour
{
    public static CulverSceneController instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject culverGameObject;
    [SerializeField] private GameObject enemySpawnLocation;

    // UI
    private GameObject mainCanvas;
    private GameObject playerEnergyUI;
    private CulverSceneUIController culverSceneUIController;

    // Game States
    private Player player;
    private int playerCurrentEnergy = 0;
    private List<Enemy> enemies = new List<Enemy>();
    private int turns = 5;

    void Start()
    {
        culverSceneUIController = gameObject.AddComponent<CulverSceneUIController>();
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
        culverSceneUIController.Initialize();
        updateTurnCount(1);

        mainCanvas = GameObject.FindGameObjectWithTag("BaseLevelCanvas");
        HandManager.instance.Initialize();

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

        // Spawn Culver
        GameObject culver = Instantiate(culverGameObject, enemySpawnLocation.transform);
        Enemy culverComponent = culver.GetComponent<Enemy>();
        enemies.Add(culverComponent);

        // Start turn sequence
        TurnManager.instance.Initialize(player, enemies);
        TurnManager.instance.startCulver(turns);
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

    public void healPlayer(int value)
    {
        player.healCharacter(value);
        Debug.Log("Player healed by: " + value);
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

    public void processEnemyCardEffectsOnPlayer(List<CardEffect> effects, Enemy enemy)
    {
        player.processCardEffects(effects, enemy);
    }

    public List<Enemy> getEnemies()
    {
        return enemies;
    }

    public Transform getMainCanvasTransfom()
    {
        return mainCanvas.transform;
    }

    public Player getPlayer()
    {
        return player;
    }

    public void updateTurnCount(int currentTurn)
    {
        culverSceneUIController.updateTurnCount(currentTurn, turns);
    }

    public void updateDrawPile(int count)
    {
        culverSceneUIController.updateDrawPile(count);
    }

    public void updateDiscardPile(int count)
    {
        culverSceneUIController.updateDiscardPile(count);
    }
}