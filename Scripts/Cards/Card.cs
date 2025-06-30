using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    private CardModelSO cardModel;
    [SerializeField] private GameObject cardTitle;
    [SerializeField] private GameObject cardDescription;
    [SerializeField] private GameObject cardEnergyValue;

    public void setCardDisplayInformation(CardModelSO model)
    {
        cardModel = model;

        cardTitle = cardTitle ?? Helpers.findDescendant(transform, "Title");
        cardDescription = cardDescription ?? transform.Find("Description")?.gameObject;
        cardEnergyValue = cardEnergyValue ?? transform.Find("EnergyCost")?.gameObject;

        if (cardTitle == null || cardDescription == null || cardEnergyValue == null)
        {
            Debug.LogError("Card UI element(s) not found!");
            return;
        }

        cardTitle.GetComponent<TextMeshProUGUI>().text = model.title;
        cardEnergyValue.GetComponent<TextMeshProUGUI>().text = model.energy.ToString();
        cardDescription.GetComponent<TextMeshProUGUI>().text = model.details;
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

    public CardType getCardType()
    {
        return cardModel.type;
    }

    public bool isSpecial()
    {
        return cardModel.special;
    }

    public void setModel(CardModelSO model)
    {
        cardModel = model;
    }

    public void mulitplyValues(int multiplier)
    {
        cardModel.damage *= multiplier;
        cardModel.armor *= multiplier;
        cardModel.strength *= multiplier;
    }
}
