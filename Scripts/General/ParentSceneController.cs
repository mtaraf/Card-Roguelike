using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParentSceneController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    protected GameObject enemySpawnLocation;
    private bool discardInProgress = false;

    // UI
    private GameObject mainCanvas;
    private GameObject playerEnergyUI;
    private GameObject discardUI;

    // Game States
    protected Player player;
    private int playerCurrentEnergy = 0;
    protected List<Enemy> enemies = new List<Enemy>();

    public virtual void Start()
    {
        InitializeScene();
    }

    public virtual void InitializeScene()
    {
        HandManager.instance.Initialize();
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

        // Find Discard UI
        discardUI = GameObject.FindGameObjectWithTag("DiscardUI");
        discardUI.gameObject.SetActive(false);

        if (enemySpawnLocation == null)
        {
            enemySpawnLocation = GameObject.FindGameObjectWithTag("EnemySpawnLocation");
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
        Debug.Log("parentscene");
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

    public virtual void updateDrawPile(int count)
    {
    }

    public virtual void updateDiscardPile(int count)
    {
    }

    public void toggleDiscardUI()
    {
        discardUI.gameObject.SetActive(true);
    }

    public DiscardUI getDiscardUI()
    {
        return discardUI.GetComponent<DiscardUI>();
    }

    public void startDiscard(int numberToDiscard)
    {
        discardInProgress = true;
        discardUI.gameObject.SetActive(true);
        DiscardUI discard = discardUI.GetComponent<DiscardUI>();

        discard.setDiscardNum(numberToDiscard);

        HandManager.instance.clearSelectedCard();
    }

    public bool getDiscardInProgress()
    {
        return discardInProgress;
    }

    public void setDiscardInProgress(bool inProgress)
    {
        discardInProgress = inProgress;
    }

    public virtual void updateCurrentRound(int currentRound, int totalTurns)
    {
        Debug.Log("Parent update round count called");
    }
}
