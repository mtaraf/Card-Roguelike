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
        playerPrefab = GameManager.instance.getPlayerCharacter();

        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load<GameObject>("CharacterPrefabs/PlayableCharacters/Mistborn/MistbornCharacter");
        }
        GameObject playerObj = Instantiate(playerPrefab, mainCanvas.transform);
        player = playerObj.GetComponent<Player>();

        GameManager.instance.setPlayer(player);

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
        Debug.Log("Playing player animation");
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

    public DeckModelSO getCurrentPlayerDeck()
    {
        return player.getCards();
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

    public List<CardEffect> checkCritHits(List<CardEffect> cardEffects)
    {
        foreach (CardEffect effect in cardEffects)
        {
            if (effect.type == EffectType.Damage && effect.critRate > 0)
            {
                float rand = Random.Range(0,101);
                if (rand <= effect.critRate)
                {
                    effect.critRate = 100;
                }
                else
                {
                    effect.critRate = 0;
                }
            }
        }
        return cardEffects;
    }
}
