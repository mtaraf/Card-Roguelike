using System;
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
    private TopBarUIManager topBarUIManager;
    private VictoryManager victoryManager;
    private int currentSaveSlot = -1;


    // Game States
    private EncounterMap map;
    private int currentLevel = -1;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int playerGold;
    private Player player;
    private int cardRarity = 0;
    private int cardChoices = 3;
    [SerializeField] private List<DeckModelSO> victoryCardPools; // 0: common, 1: rare, etc.
    private ParentSceneController currentSceneController;

    // UI
    [SerializeField] private List<GameObject> turnBasedStatuses = new List<GameObject>();
    [SerializeField] private List<GameObject> valueBasedStatuses = new List<GameObject>();

    // Cards
    [SerializeField] private DeckModelSO starterDeck;


    // Encounter Information
    private EncounterType currentPath;
    private EncounterReward encounterReward;
    private int encounterRewardValue;

    // Scene
    private string currentScene;
    private HashSet<int> battleScenes = new HashSet<int>() {1,4};


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
        currentScene = scene.name;
        Debug.Log($"Game Manager detected scene loaded: {scene.name} + {scene.buildIndex}");
        if (scene.buildIndex != 0)
        {
            // for dev testing
            if (currentSaveSlot != -1)
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
            }
            else
            {
                createPlayerDeckCopy();
                playerMaxHealth = 50;
                playerHandSize = 6;
                playerHandEnergy = 3;
                playerCurrentHealth = 50;
                playerGold = 30;
                playerHandEnergy = 3;
            }

            StartCoroutine(updateUI());
        }

        if (battleScenes.Contains(scene.buildIndex))
        {
            StartCoroutine(waitForUI());
        }
    }

    private IEnumerator updateUI()
    {
        yield return null;
        topBarUIManager = FindFirstObjectByType<TopBarUIManager>();
        if (topBarUIManager != null)
        {
            topBarUIManager.Initialize(playerGold, currentLevel, cardRarity);
        }
    }

    public void setCurrentSceneController(ParentSceneController controller)
    {
        currentSceneController = controller;
    }

    public ParentSceneController getCurrentSceneController()
    {
        return currentSceneController;
    }

    private void initializePrefabs()
    {
        turnBasedStatuses = new List<GameObject>(Resources.LoadAll<GameObject>("UI/Effects/TurnBasedEffects"));
        valueBasedStatuses = new List<GameObject>(Resources.LoadAll<GameObject>("UI/Effects/ValueBasedEffects"));
        starterDeck = Resources.Load<DeckModelSO>("ScriptableObjects/Decks/PaladinTestDeck");

        // Get Victory Card Pools
        victoryCardPools.Insert(0, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/CommonVictoryCards")));
        victoryCardPools.Insert(1, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/RareVictoryCards")));
        victoryCardPools.Insert(2, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/EpicVictoryCards")));
        victoryCardPools.Insert(3, cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Decks/MythicVictoryCards")));
    }

    private IEnumerator waitForUI()
    {
        yield return null;
        victoryManager = transform.AddComponent<VictoryManager>();
        victoryManager.instantiate();

        getPlayer();
    }

    private void getPlayer()
    {
        currentSceneController.getPlayer();
    }

    public void updateDrawPile(int count)
    {
        currentSceneController.updateDrawPile(count);
    }

    public void updateDiscardPile(int count)
    {
        currentSceneController.updateDiscardPile(count);
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
        addEncounterRewards();
        yield return new WaitForSeconds(1.0f);
        victoryManager.showVictoryScreen(cardChoices, cardRarity);
    }

    void addEncounterRewards()
    {
        switch (encounterReward)
        {
            case EncounterReward.Gold:
                addPlayerGold(5 * currentLevel);
                break;
            case EncounterReward.CardRarity:
                cardRarity += 1;
                break;
            case EncounterReward.CardChoices:
                cardChoices += 1;
                break;
        }
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        CardModelSO clone = model.clone();
        playerDeck.cards.Add(clone);
    }

    public void removeCardFromPlayerDeck(CardModelSO model)
    {
        playerDeck.cards.Remove(model);
    }


    public IEnumerator moveToNextEncounter()
    {
        // Mark node as completed
        map.getNode(map.currentEncounterId).completed = true;

        // update player stats
        playerCurrentHealth = player.getCurrentHealth();
        playerMaxHealth = player.getMaxHealth();

        // Save game
        yield return new WaitUntil(() => saveGame(currentSaveSlot));

        // Go to path selection scene
        SceneLoader.instance.loadScene("PathSelectionScene", () =>
        {
            Debug.Log("Scene loaded!");
        });
    }

    public int getPlayerGold()
    {
        return playerGold;
    }

    // TO-DO: add cha-ching sound
    public void addPlayerGold(int value)
    {
        playerGold += value;
        topBarUIManager.updateGoldCount(playerGold);
    }

    // TO-DO: add gold spending sound
    public void subtractPlayerGold(int value)
    {
        playerGold -= value;
        topBarUIManager.updateGoldCount(playerGold);
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

    public DeckModelSO getStarterDeck()
    {
        return starterDeck;
    }

    public void loadEncounterTypeAndRewards(EncounterMap map)
    {
        this.map = map;
        EncounterNode currentNode = map.getNode(map.currentEncounterId);
        currentPath = currentNode.type;
        encounterReward = currentNode.encounterReward;
        //encounterRewardValue = rewardValue;
        currentLevel = currentNode.level;

        StartCoroutine(updateUI());
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
        saveData.encounterMap = map;

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
        map = data.encounterMap;
        //map.rebuildPaths();

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

    public void exitToMainMenu()
    {
        saveGame(currentSaveSlot);

        loadScene(0);
    }
}
