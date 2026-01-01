using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefeatManager : MonoBehaviour
{
    private GameObject defeatScreenPrefab;
    private Transform canvasTransform;
    private GameObject defeatScreenInstance;
    private TextMeshProUGUI statistics;
    private Button returnToMainMenuButton;

    public void instantiate()
    {
        defeatScreenPrefab = Resources.Load<GameObject>("UI/RoundEnd/DefeatScreen");
        canvasTransform = GameObject.FindGameObjectWithTag("OverlayCanvas").transform;
    }

    // TO-DO: (Alpha) add damage dealt/taken, total time played to stats
    public void showDefeatScreen(int level, int gold, DeckModelSO deck)
    {
        defeatScreenInstance = Instantiate(defeatScreenPrefab, canvasTransform);


        statistics = defeatScreenInstance.transform.Find("Statistics").GetComponent<TextMeshProUGUI>();
        if (statistics == null)
        {
            Debug.LogError("Could not find continue button in victory screen");
        }

        int upgradedCards = 0;

        foreach (CardModelSO card in deck.cards)
        {
            if (card.title.Contains('+'))
                upgradedCards++;
        }

        statistics.text = $"Level: {level} \n\nGold: {gold} \n\nUpgraded Cards: {upgradedCards}";

        returnToMainMenuButton = defeatScreenInstance.transform.Find("MainMenuButton").GetComponent<Button>();
        if (returnToMainMenuButton == null)
        {
            Debug.LogError("Could not find continue button in victory screen");
        }

        returnToMainMenuButton.onClick.RemoveAllListeners();
        returnToMainMenuButton.onClick.AddListener(() => returnToMainMenu());
    }

    void returnToMainMenu()
    {
        // TO-DO: (Beta) Add all new cards to card book and any statistics needed

        // Clear Save slot
        bool cleared = GameManager.instance.clearSaveSlot();

        if (!cleared)
            Debug.LogError("Could not clear current save slot after defeat!");

        // Move to Main Menu Scene
        GameManager.instance.loadScene((int)SceneBuildIndex.MAIN_MENU);
    }
}