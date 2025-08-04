using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardConfirmation : MonoBehaviour
{
    [SerializeField] private Card currentCard;
    [SerializeField] private Card upgradedCard;
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;

    private CardModelSO currentCardModel;
    private CardModelSO upgradedCardModel;

    void Awake()
    {
        cancel.onClick.RemoveAllListeners();
        cancel.onClick.AddListener(() => cancelUpgrade());

        confirm.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(() => confirmUpgrade());
    }

    private void cancelUpgrade() {
        Destroy(gameObject);
    }

    private void confirmUpgrade()
    {
        // Add card to player deck
        GameManager.instance.addCardToPlayerDeck(upgradedCardModel);

        // Remove card old card from player deck
        GameManager.instance.removeCardFromPlayerDeck(currentCardModel);

        // Subtract gold from player
        GameManager.instance.subtractPlayerGold(30);

        // refresh forge ui to reflect changes
        ForgeSceneController.instance.updateUpgradedCardDisplay(currentCardModel, upgradedCardModel);

        Destroy(gameObject);
    }

    public void setCardInformation(CardModelSO current, CardModelSO upgrade) {
        currentCardModel = current;
        upgradedCardModel = upgrade;
        currentCard.setCardDisplayInformation(current);
        upgradedCard.setCardDisplayInformation(upgrade);
    }
}
