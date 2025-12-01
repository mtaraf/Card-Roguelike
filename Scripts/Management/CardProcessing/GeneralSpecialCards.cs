using System;
using System.Collections.Generic;


public class HuntersInstinctLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
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

public class StockadeLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();
        List<Card> hand = HandManager.instance.getCurrentHand();

        int multiplier = 1;

        foreach (Card cardInHand in hand)
        {

            if (cardInHand.getCardType() == CardType.Buff)
                multiplier++;
        }

        effects[0].value *= multiplier;

        return effects;
    }
}

public class OnslaughtLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();
        List<Card> hand = HandManager.instance.getCurrentHand();

        int multiplier = 1;

        foreach (Card cardInHand in hand)
        {

            if (cardInHand.getCardType() == CardType.Attack)
                multiplier++;
        }

        effects[0].value *= multiplier;

        return effects;
    }
}