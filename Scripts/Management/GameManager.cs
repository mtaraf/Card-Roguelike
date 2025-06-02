using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Menu
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject loadGameMenu;


    // Game States


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Game Functions

    // Save Functions

    // Menu functions
    public void playButtonClicked()
    {
        mainMenu.gameObject.SetActive(false);
        loadGameMenu.gameObject.SetActive(true);
        Debug.Log("Play clicked!");
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
    }
}
