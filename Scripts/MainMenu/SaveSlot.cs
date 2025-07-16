using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject details;
    [SerializeField] private GameObject button;
    [SerializeField] private int slotNumber;
    void Start()
    {
        title.GetComponent<TextMeshProUGUI>().text = $"Save Slot {slotNumber}";
    }

    public void setSaveSlotInformation(string slotTitle, string slotDetails, Action onClickAction = null)
    {
        if (onClickAction != null)
        {
            Button slotButton = button.GetComponent<Button>();
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClickAction());
        }

        title.GetComponent<TextMeshProUGUI>().text = slotTitle;
        details.GetComponent<TextMeshProUGUI>().text = slotDetails;
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Load Game";
    }

    public int getSlotNumber()
    {
        return slotNumber;
    }
}
