using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject details;
    [SerializeField] private GameObject topButton;
    [SerializeField] private GameObject bottomButton;
    [SerializeField] private int slotNumber;
    void Start()
    {
        title.GetComponent<TextMeshProUGUI>().text = $"Save Slot {slotNumber}";
    }

    public void setSaveSlotInformation(string slotTitle, string slotDetails)
    {
        if (slotDetails == "")
        {
            topButton.SetActive(false);
            bottomButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "New Game";

            Button slotButton = bottomButton.GetComponent<Button>();
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => enterGame(slotNumber));
        }
        else
        {
            topButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Load Game";

            Button slotButton = topButton.GetComponent<Button>();
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => enterGame(slotNumber));

            Button button = bottomButton.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => deleteSave());
        }

        title.GetComponent<TextMeshProUGUI>().text = slotTitle;
        details.GetComponent<TextMeshProUGUI>().text = slotDetails;
    }

    private void deleteSave()
    {
        // Deletes Save
        GameManager.instance.deleteSave(slotNumber);
        setSaveSlotInformation("Save Slot " + slotNumber, "");
    }

    public int getSlotNumber()
    {
        return slotNumber;
    }

    public void enterGame(int saveSlot)
    {
        GameManager.instance.setCurrentSaveSlot(saveSlot);

        // Check for SaveSlot
        if (!GameManager.instance.checkForSavedGames(saveSlot))
        {
            // Navigate to Character Selection
            GameManager.instance.loadScene((int)SceneBuildIndex.CHARACTER_SELECTION);
        }
        else
        {
            // Navigate to save slot level
            GameManager.instance.loadScene((int)SceneBuildIndex.PATH_SELECTION);
        }
        AudioManager.instance.playClick();
    }
}
