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

    // Menu
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject loadGameMenu;

    // Controllers
    private BaseLevelUIController baseLevelUIController;
    private VictoryManager victoryManager;


    // Game States
    private int currentLevel = 1;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int currentPlayerEnergy;
    private int playerGold;
    
    [SerializeField] private List<DeckModelSO> victoryCardPools; // 0: common, 1: rare, etc.

    // UI
    [SerializeField] private List<GameObject> turnBasedStatuses = new List<GameObject>();
    [SerializeField] private List<GameObject> valueBasedStatuses = new List<GameObject>();

    // Cards
    [SerializeField] private DeckModelSO starterDeck;


    // Private data
    private string pathToSaveFiles = "../../Saves/";


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

        // Initial player values
        createPlayerDeckCopy();

        playerMaxHealth = 50;
        playerHandSize = 6;
        playerHandEnergy = 3;
        playerCurrentHealth = 50;
        playerGold = 0;
        Debug.Log("Game Manager Awake");
    }


    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name}");
        if (scene.buildIndex == 1)
        {
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

    public void encounterVictory(int goldEarned)
    {
        playerGold += goldEarned;
        baseLevelUIController.updateGoldCount(playerGold);
        victoryManager.showVictoryScreen();
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        playerDeck.cards.Add(model);
    }


    public void moveToNextEncounter()
    {
        currentLevel++;
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

    public void levelCompleted()
    {
        currentLevel++;
    }

    public void endTurn()
    {
        TurnManager.instance.endTurnButtonPressed();
    }

    public int getCurrentPlayerEnergy()
    {
        return currentPlayerEnergy;
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

    // Card/Deck Functions
    public DeckModelSO getStarterDeck()
    {
        return starterDeck;
    }


    // Save Functions



    // Menu functions
    public void playButtonClicked()
    {
        mainMenu.gameObject.SetActive(false);
        loadGameMenu.gameObject.SetActive(true);
    }

    private void checkForSavedGames()
    {

    }

    private void saveGame()
    {

    }

    public void enterGame(int saveSlot)
    {
        // Add checks for save slots

        SceneManager.LoadScene(1);
    }

    public void openSettings()
    {
        mainMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    public void returnToMainMenu()
    {
        loadGameMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    public void quitGame()
    {

        // Save everything here

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
