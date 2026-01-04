using System;
using System.Collections.Generic;
using UnityEngine;


public class HuntersInstinctLogic : SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        List<Card> hand = HandManager.instance.getCurrentHand();

        int cardsEnhanced = 0;

        foreach (Card cardInHand in hand)
        {

            if (cardInHand.getCardType() == CardType.Attack && cardInHand.getEnergy() > 0)
            {
                // Min value is 0 energy cost
                CardModelSO model = cardInHand.getCardModel();
                model.energy -= 1;
                cardsEnhanced++;
                HandManager.instance.updateCardDisplay(cardInHand);
            }

            if (cardsEnhanced > 1)
                break;
        }

        return new List<CardEffect>();
    }
}

public class StockadeLogic : SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        List<CardEffect> effects = card.getEffects();
        List<Card> hand = HandManager.instance.getCurrentHand();

        int multiplier = 0;

        foreach (Card cardInHand in hand)
        {
            Debug.Log(cardInHand.getCardType());
            if (cardInHand.getCardType() == CardType.Buff)
                multiplier++;
        }

        if (multiplier > 0)
            effects[0].value *= multiplier;

        return effects;
    }
}

public class OnslaughtLogic : SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        List<CardEffect> effects = card.getEffects();
        List<Card> hand = HandManager.instance.getCurrentHand();

        int multiplier = 0;

        foreach (Card cardInHand in hand)
        {
            Debug.Log(cardInHand.getCardType());
            if (cardInHand.getCardType() == CardType.Attack)
                multiplier++;
        }

        if (multiplier > 0)
            effects[0].value *= multiplier;

        return effects;
    }
}

public class GymnasticsLogic : SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        Player player = parentSceneController.getPlayer();

        int agility = player.getAttributeValue(EffectType.Agility);
        player.updateAttribute(EffectType.Agility, Math.Min(agility * 2, 20));

        return effects;
    }
}

public class PewterDragLogic : SpecialCardLogicInterface
{
    bool loseEnergy = false;
    public PewterDragLogic(bool energyLoss)
    {
        loseEnergy = energyLoss;
    }
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        parentSceneController.addPlayerEnergy(2);

        if (loseEnergy)
            cardProcessor.setNextTurnEnergyLoss(1);

        return effects;
    }
}

public class SecondChanceLogic : SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        ObservableDeck discardPile = HandManager.instance.getDiscardPile();

        List<CardModelSO> usableCards = discardPile.cards.FindAll((cardModel) => !cardModel.corrupts);
        int size = usableCards.Count;

        if (size > 0)
        {
            int rand = UnityEngine.Random.Range(0, size);

            // TO-DO (Alpha): card spawn animation from discard pile
            HandManager.instance.moveCardFromDiscardPileToHand(usableCards[rand]);
        }

        return new List<CardEffect>();
    }
}

public class LastHopeLogic : SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, CardProcessor cardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        int handSize = HandManager.instance.getCurrentHand().Count - 1;

        if (handSize > 0)
        {
            effects[0].value = Math.Max(0, effects[0].value - (4 * handSize));
        }

        return effects;
    }
}