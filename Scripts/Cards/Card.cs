using System.Collections;
using System.Collections.Generic;
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

    public List<CardEffect> getEffects()
    {
        return cardModel.effects;
    }

    public CardType getCardType()
    {
        return cardModel.type;
    }

    public int getCardsToDraw()
    {
        return cardModel.cardsDrawn;
    }

    public bool isSpecial()
    {
        return cardModel.special;
    }

    public CardRarity getRarity()
    {
        return cardModel.rarity;
    }

    public void setModel(CardModelSO model)
    {
        cardModel = model;
    }

    public bool isCorrupt()
    {
        return cardModel.corrupts;
    }

    public void mulitplyValues(int multiplier)
    {
        cardModel.multiplyValues(multiplier);
    }
}
