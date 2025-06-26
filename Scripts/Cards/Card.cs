using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    private CardModelSO cardModel;

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

    public string getCardTitle()
    {
        return cardModel.title;
    }

    public int getTurns()
    {
        return cardModel.turns;
    }

    public int getStrength()
    {
        return cardModel.strength;
    }

    public CardModelSO getCardModel()
    {
        return cardModel;
    }

    public Target getCardTarget()
    {
        return cardModel.target;
    }

    public bool hasCondition()
    {
        return cardModel.condition.metric != ConditionMetric.NO_CONDITION;
    }

    public int getDamage()
    {
        return cardModel.damage;
    }

    public int getArmor()
    {
        return cardModel.armor;
    }

    public bool isSpecial()
    {
        return cardModel.special;
    }
}
