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
    [SerializeField] private GameObject mainLevelCanvas;


    // Game States
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> enemyPrefabs;
    // [SerializeField] private List<GameObject> bossPrefabs;
    private int currentLevel = 0;
    private List<Enemy> enemies = new List<Enemy>();
    private Player player;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerCurrentHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int currentPlayerEnergy;

    // UI
    private GameObject playerEnergyUI;
    [SerializeField] private List<GameObject> statuses = new List<GameObject>();

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
    }


    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Game Manager detected scene loaded: {scene.name}");

        // Level scene
        if (scene.name == "BaseLevelScene")
        {
            baseLevelSceneStart();
        }
    }

    // Scene Start Functionality

    void baseLevelSceneStart()
    {
        // Get UI data
        playerEnergyUI = Helpers.findDescendant(mainLevelCanvas.transform, "EnergyUI");

        if (playerEnergyUI == null)
        {
            Debug.LogError("Could not find a piece of UI within the main level scene");
        }


        // Instantiate and set up player data
        GameObject playerObj = Instantiate(playerPrefab, mainLevelCanvas.transform);
        player = playerObj.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Could not find player component in player prefab");
        }

        player.setDeck(playerDeck);
        player.setMaxHealth(playerMaxHealth);
        player.setCurrentHealth(playerCurrentHealth);


        if (currentLevel == 0)
        {

        }

        // add enemy to level
        randomizeEnemy();


        // Start turn sequence
        TurnManager.instance.Initialize(player, enemies);
        TurnManager.instance.startTurns();
    }

    public List<GameObject> getStatusObjects()
    {
        return statuses;
    }

    // Randomize the enemy the player encounters
    void randomizeEnemy()
    {
        if (currentLevel % 10 == 0 && currentLevel != 0)
        {
            // Load Boss level
        }
        else
        {
            // Random common enemy
            int random = Random.Range(0, enemyPrefabs.Count - 1);
            GameObject enemyObj = Instantiate(enemyPrefabs[random], mainLevelCanvas.transform);

            // remove enemy from list
            enemyPrefabs.Remove(enemyPrefabs[random]);

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

    // Encounter End Functions

    public void encounterVictory()
    {
        VictoryManager victoryManager = FindFirstObjectByType<VictoryManager>();
        victoryManager.showVictoryScreen();
    }

    public void addCardToPlayerDeck(CardModelSO model)
    {
        playerDeck.cards.Add(model);
    }


    public void moveToNextEncounter()
    {
        SceneManager.LoadScene(1);
    }

    public int getPlayerHandSize()
    {
        return playerHandSize;
    }

    public void resetPlayerEnergy()
    {
        currentPlayerEnergy = playerHandEnergy;
    }

    public void endTurn()
    {
        TurnManager.instance.endTurnButtonPressed();
    }


    // Variable Mutations

    public Dictionary<EffectType, int> getPlayerAttributes()
    {
        return player.getAttributes();
    }

    public int getCurrentPlayerEnergy()
    {
        return currentPlayerEnergy;
    }

    public void usePlayerEnergy(int value)
    {
        currentPlayerEnergy -= value;
        updatePlayerEnergyUI();
    }

    public void processEnemyCardEffectsOnPlayer(List<CardEffect> effects)
    {
        player.processCardEffects(effects);
    }

    public void playAnimationsForCard(CardType type)
    {
        player.playAnimation(type);
    }

    // UI Updates

    void updatePlayerEnergyUI()
    {
        playerEnergyUI.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>().text = currentPlayerEnergy.ToString();
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
