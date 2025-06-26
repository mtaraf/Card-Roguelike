using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

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
    private bool endPlayerTurnBool = true;
    private DeckModelSO playerDeck;
    private int playerMaxHealth;
    private int playerHandSize;
    private int playerHandEnergy;
    private int currentPlayerEnergy;

    // UI
    private GameObject playerEnergyUI;

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


        if (currentLevel == 0)
        {

        }

        // add enemy to level
        randomizeEnemy();


        // Start turn sequence
        StartCoroutine(startTurnSequence());
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

    IEnumerator startTurnSequence()
    {
        yield return null;
        TurnState currentState = TurnState.PlayerTurn;
        while (enemies.Count > 0 && player.getCurrentHealth() > 1)
        {
            if (currentState == TurnState.PlayerTurn)
            {
                yield return StartCoroutine(startPlayerTurn());
                currentState = TurnState.EnemyTurn;
                endPlayerTurn();
                Debug.Log("Player turn has ended, moving to enemy turn");
            }
            else
            {
                yield return StartCoroutine(startEnemyTurn());
                currentState = TurnState.PlayerTurn;
                Debug.Log("Enemy turn has ended, moving to player turn");
            }

            // Removes dead enemies at the end of turn
            List<Enemy> enemiesCopy = new List<Enemy>(enemies);
            foreach (Enemy enemy in enemiesCopy)
            {
                if (enemy.getCurrentHealth() < 1)
                {
                    enemies.Remove(enemy);
                }
            }
            yield return null; // Keep this for ui frame update (maybe lengthen if animations get put in)
        }

        if (enemies.Count == 0)
        {
            // Victory!
        }
        else
        {
            // Game over screen
        }
    }

    IEnumerator startPlayerTurn()
    {
        yield return new WaitForSeconds(1);
        player.processStartOfTurnEffects();
        currentPlayerEnergy = playerHandEnergy;
        updatePlayerEnergyUI();
        if (endPlayerTurnBool == false)
        {
            endPlayerTurnBool = true;
        }

        HandManager.instance.drawCards(playerHandSize);

        yield return new WaitUntil(() => endPlayerTurnBool == false);
    }

    void endPlayerTurn()
    {
        Debug.Log("end turn called");
        HandManager.instance.shuffleCurrentHandIntoDiscardPile();
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

    IEnumerator startEnemyTurn()
    {
        // Delay before enemy turn
        yield return new WaitForSeconds(1);

        // TO-DO: implement enemy turn
        foreach (Enemy enemy in enemies)
        {
            enemy.processStartOfTurnEffects();
            // TO-DO: depending on current level adjust multiplier for enemy cards
            enemy.playCard(1);
            yield return new WaitForSeconds(1.5f);
        }

        // Delay before handing turn to player
        yield return new WaitForSeconds(1);
    }

    public void endTurn()
    {
        endPlayerTurnBool = false;
    }

    // UI Updates

    void updatePlayerEnergyUI()
    {
        Debug.Log(currentPlayerEnergy);
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
        Debug.Log("Play clicked!");
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
        Debug.Log("Open Settings");
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
        Debug.Log("Quit Game");

        // Save everything here

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
