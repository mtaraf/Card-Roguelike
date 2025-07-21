using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Controllers
    private BaseLevelUIController baseLevelUIController;
    private VictoryManager victoryManager;
    private int currentSaveSlot = -1;


    // Game States
    private EncounterMap map;
    private int currentLevel = 1;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int playerGold;
    private Player player;
    private int currentCardRarity = 0;
    [SerializeField] private List<DeckModelSO> victoryCardPools; // 0: common, 1: rare, etc.

    // UI
    [SerializeField] private List<GameObject> turnBasedStatuses = new List<GameObject>();
    [SerializeField] private List<GameObject> valueBasedStatuses = new List<GameObject>();

    // Cards
    [SerializeField] private DeckModelSO starterDeck;


    // Encounter Information
    private EncounterType currentPath;
    private EncounterReward encounterReward;
    private int encounterRewardValue;


    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        initializePrefabs();

        // Detect when a new scene is loaded
        SceneManager.sceneLoaded += onSceneLoaded;
    }


    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name}");
        if (scene.buildIndex == 1)
        {
            if (!loadGame(currentSaveSlot))
            {
                createPlayerDeckCopy();
                playerMaxHealth = 50;
                playerHandSize = 6;
                playerHandEnergy = 3;
                playerCurrentHealth = 50;
                playerGold = 0;
                playerHandEnergy = 3;
            }
            StartCoroutine(waitForBaseLevelUI());
        }
    }

    private void initializePrefabs()
    {
        turnBasedStatuses = new List<GameObject>(Resources.LoadAll<GameObject>("UI/Effects/TurnBasedEffects"));
        valueBasedStatuses = new List<GameObject>(Resources.LoadAll<GameObject>("UI/Effects/ValueBasedEffects"));
        starterDeck = Resources.Load<DeckModelSO>("ScriptableObjects/Decks/StarterDeck");

        // Get Victory Card Pools
        victoryCardPools.Insert(0,cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/CommonVictoryCards")));
        victoryCardPools.Insert(1,cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/RareVictoryCards")));
        victoryCardPools.Insert(2,cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/EpicVictoryCards")));
        victoryCardPools.Insert(3,cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/MythicVictoryCards")));
    }

    private IEnumerator waitForBaseLevelUI()
    {
        yield return null;
        baseLevelUIController = FindFirstObjectByType<BaseLevelUIController>();
        if (baseLevelUIController == null)
        {
            baseLevelUIController = transform.AddComponent<BaseLevelUIController>();
        }

        baseLevelUIController.Initialize();
        baseLevelUIController.updateLevelCount(currentLevel);
        baseLevelUIController.updateGoldCount(playerGold);


        victoryManager = transform.AddComponent<VictoryManager>();
        victoryManager.instantiate();

        player = BaseLevelSceneController.instance.getPlayer();
    }

    private DeckModelSO cloneDeck(DeckModelSO deck)
    {
        DeckModelSO clonedDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        clonedDeck.cards = new List<CardModelSO>();

        foreach (CardModelSO card in deck.cards)
        {
            clonedDeck.cards.Add(card.clone());
        }

        return clonedDeck;
    }

    public void createPlayerDeckCopy()
    {
        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>();

        foreach (CardModelSO card in starterDeck.cards)
        {
            playerDeck.cards.Add(card.clone());
        }
    }

    public List<GameObject> getTurnBasedStatusObjects()
    {
        return turnBasedStatuses;
    }

    public List<GameObject> getValueBasedStatusObject()
    {
        return valueBasedStatuses;
    }

    public List<DeckModelSO> getVictoryCardsPools()
    {
        return victoryCardPools;
    }

    public IEnumerator encounterVictory(int goldEarned)
    {
        AudioManager.instance.playVictory();
        playerGold += goldEarned;
        baseLevelUIController.updateGoldCount(playerGold);
        yield return new WaitForSeconds(1.0f);
        victoryManager.showVictoryScreen();
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        playerDeck.cards.Add(model);
    }


    public IEnumerator moveToNextEncounter()
    {
        currentLevel++;
        playerCurrentHealth = player.getCurrentHealth();
        playerMaxHealth = player.getMaxHealth();
        yield return new WaitUntil(() => saveGame(currentSaveSlot));
        SceneLoader.instance.loadScene(1, () =>
        {
            Debug.Log("Scene loaded!");
        });
    }

    public int getPlayerGold()
    {
        return playerGold;
    }

    public void addPlayerGold(int value)
    {
        playerGold += value;
    }

    public void subtractPlayerGold(int value)
    {
        playerGold -= value;
    }

    public int getPlayerHandSize()
    {
        return playerHandSize;
    }

    public int getPlayerHandEnergy()
    {
        return playerHandEnergy;
    }

    public int getCurrentLevel()
    {
        return currentLevel;
    }

    public void endTurn()
    {
        TurnManager.instance.endTurnButtonPressed();
    }

    public DeckModelSO getPlayerDeck()
    {
        return playerDeck;
    }

    public int getPlayerMaxHealth()
    {
        return playerMaxHealth;
    }

    public int getPlayerCurrentHealth()
    {
        return playerCurrentHealth;
    }

    public void updatePlayerHealth(int current, int max)
    {
        playerCurrentHealth = current;
        playerMaxHealth = max;
    }

    public DeckModelSO getStarterDeck()
    {
        return starterDeck;
    }

    public void setEncounterTypeAndRewards(EncounterType encounterType, EncounterReward rewardType, int rewardValue)
    {
        currentPath = encounterType;
        encounterReward = rewardType;
        encounterRewardValue = rewardValue;
    }

    public void setEncounterMap(EncounterMap map)
    {
        this.map = map;
    }

    public EncounterMap getEncounterMap()
    {
        return map;
    }

    // Save Functions
    public bool checkForSavedGames(int saveSlot)
    {
        return SaveSystem.saveFileExists(saveSlot);
    }

    public string getSaveSlotSummary(int saveSlot)
    {
        return SaveSystem.getSlotSummary(saveSlot);
    }

    public string getSaveSlotTitle(int saveSlot)
    {
        return SaveSystem.getSlotTitle(saveSlot);
    }

    public bool saveGame(int saveSlot)
    {
        SaveData saveData = new SaveData();
        saveData.currentLevel = currentLevel;
        saveData.playerCurrentHealth = playerCurrentHealth;
        saveData.playerMaxHealth = playerMaxHealth;
        saveData.playerGold = playerGold;
        saveData.playerHandSize = playerHandSize;
        saveData.playerHandEnergy = playerHandEnergy;

        saveData.playerCards = new List<SerializableCardModel>();

        foreach (CardModelSO model in playerDeck.cards)
        {
            saveData.playerCards.Add(SaveSystem.convertToSerializableModel(model));
        }

        SaveSystem.saveGame(saveData, saveSlot);

        return true;
    }

    private bool loadGame(int saveSlot)
    {
        SaveData data = SaveSystem.loadGame(saveSlot);
        if (data == null)
        {
            return false;
        }

        currentLevel = data.currentLevel;
        playerCurrentHealth = data.playerCurrentHealth;
        playerMaxHealth = data.playerMaxHealth;
        playerGold = data.playerGold;
        playerHandSize = data.playerHandSize;
        playerHandEnergy = data.playerHandEnergy;

        playerDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        playerDeck.cards = new List<CardModelSO>();

        foreach (SerializableCardModel serializableCardModel in data.playerCards)
        {
            playerDeck.cards.Add(SaveSystem.convertToRuntimeCard(serializableCardModel));
        }

        return true;
    }

    public void setCurrentSaveSlot(int slot)
    {
        currentSaveSlot = slot;
    }

    public void loadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void quitGame()
    {
        saveGame(currentSaveSlot);

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
