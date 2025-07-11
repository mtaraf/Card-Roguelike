using System.Collections;
using System.Collections.Generic;
using TMPro;
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


    // Game States
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> enemyPrefabs;
    // [SerializeField] private List<GameObject> bossPrefabs;
    private int currentLevel = 1;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int currentPlayerEnergy;
    private int playerGold;

    // UI
    [SerializeField] private List<GameObject> turnBasedStatuses = new List<GameObject>();
    [SerializeField] private List<GameObject> valueBasedStatuses = new List<GameObject>();

    // Cards
    [SerializeField] private DeckModelSO starterDeck;


    // Private data
    private string pathToSaveFiles = "../../Saves/";


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Detect when a new scene is loaded
        SceneManager.sceneLoaded += onSceneLoaded;

        // Initial player values
        playerDeck = starterDeck;
        playerMaxHealth = 50;
        playerHandSize = 6;
        playerHandEnergy = 3;
        playerCurrentHealth = 50;
        playerGold = 0;
    }


    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name}");
        if (scene.buildIndex == 1)
        {
            StartCoroutine(waitForBaseLevelUI());
        }
    }

    private IEnumerator waitForBaseLevelUI()
    {
        yield return null;
        baseLevelUIController = FindFirstObjectByType<BaseLevelUIController>();
        baseLevelUIController.Initialize();

        if (baseLevelUIController == null)
        {
            Debug.LogError("BaseLevelUIController not found!");
            yield break;
        }

        baseLevelUIController.updateLevelCount(currentLevel);
    }

    public List<GameObject> getTurnBasedStatusObjects()
    {
        return turnBasedStatuses;
    }

    public List<GameObject> getValueBasedStatusObject()
    {
        return valueBasedStatuses;
    }

    public void encounterVictory(int goldEarned)
    {
        playerGold += goldEarned;
        baseLevelUIController.updateGoldCount(playerGold);
        VictoryManager victoryManager = FindFirstObjectByType<VictoryManager>();
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
