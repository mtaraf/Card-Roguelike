using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private CardModelSO cardModel;
    [SerializeField] private GameObject cardTitle;
    [SerializeField] private GameObject cardDescription;
    [SerializeField] private GameObject cardEnergyValue;
    [SerializeField] private GameObject cardBackgroundImage;

    public void setCardDisplayInformation(CardModelSO model)
    {
        cardModel = model;
        Sprite background = null;

        cardTitle = cardTitle ?? Helpers.findDescendant(transform, "Title");
        cardDescription = cardDescription ?? transform.Find("Description")?.gameObject;
        cardEnergyValue = cardEnergyValue ?? transform.Find("EnergyCost")?.gameObject;

        if (cardTitle == null || cardDescription == null || cardEnergyValue == null || cardBackgroundImage == null)
        {
            Debug.LogError("Card UI element(s) not found!");
            return;
        }

        switch (model.rarity) {
            case CardRarity.COMMON:
                background = Resources.Load<Sprite>("UI/Art/Cards/common_card");
                break;
            case CardRarity.RARE:
                background = Resources.Load<Sprite>("UI/Art/Cards/rare_card");
                break;
            case CardRarity.EPIC:
                background = Resources.Load<Sprite>("UI/Art/Cards/epic_card");
                break;
            case CardRarity.MYTHIC:
                background = Resources.Load<Sprite>("UI/Art/Cards/mythic_card");
                break;
        }

        cardTitle.GetComponent<TextMeshProUGUI>().text = model.title;
        cardEnergyValue.GetComponent<TextMeshProUGUI>().text = model.energy.ToString();
        cardDescription.GetComponent<TextMeshProUGUI>().text = model.details;

        if (background != null)
            cardBackgroundImage.GetComponent<Image>().sprite = background;
    }

    public string getCardTitle()
    {
        return cardModel.title;
    }

    public void setCardDetails(string details)
    {
        cardModel.details = details;
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
