using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    private CardModelSO cardModel;
    // public CardType type;
    // public int energy;
    // public int damage;
    // public int armor;
    // public int ward;
    // public int turns;
    // public bool special;
    // public Special[] condition;
    // public double multiplier;

    public void setCardDisplayInformation(CardModelSO model)
    {
        cardModel = model;

        // Change Title
        transform.Find("Title").GetComponent<TextMeshProUGUI>().text = model.title;

        // Change Description
        transform.Find("Description").GetComponent<TextMeshProUGUI>().text = model.details;

        // Change energy value
        transform.Find("EnergyTextContainer").transform.Find("EnergyCost").GetComponent<TextMeshProUGUI>().text = model.energy.ToString();
    }

    public CardModelSO getCardModel()
    {
        return cardModel;
    }

    public Target getCardTarget()
    {
        return cardModel.target;
    }
}
