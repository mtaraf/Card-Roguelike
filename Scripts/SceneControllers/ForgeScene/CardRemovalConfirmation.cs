using UnityEngine;
using UnityEngine.UI;

public class CardRemovalConfirmation : MonoBehaviour
{
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;
    private CardModelSO cardToRemove;

    void Awake()
    {
        cancel.onClick.RemoveAllListeners();
        cancel.onClick.AddListener(() => cancelRemoval());

        confirm.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(() => confirmRemoval());
    }

    private void cancelRemoval()
    {
        Destroy(gameObject);
    }

    private void confirmRemoval()
    {
        // Remove card from player deck
        GameManager.instance.removeCardFromPlayerDeck(cardToRemove);

        // Delete card container in forge view
        ForgeSceneController.instance.removeCardDisplay(cardToRemove);

        // Subtract Player Gold
        GameManager.instance.subtractPlayerGold(25);

        // Reorganize card containers in forge view
        

        Destroy(gameObject);
    }
    
    public void setCardInformation(CardModelSO card) {
        cardToRemove = card;
    }
}
