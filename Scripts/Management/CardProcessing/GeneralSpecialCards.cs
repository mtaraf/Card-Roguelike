using System;
using System.Collections.Generic;


public class HuntersInstinctLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardModelSO> hand = HandManager.instance.getCurrentHand();

        int cardsEnhanced = 0;

        foreach (CardModelSO cardModel in hand)
        {

            if (cardModel.type == CardType.Attack && cardModel.energy > 0)
            {
                // Min value is 0 energy cost
                cardModel.energy -= 1;
                cardsEnhanced++;
                HandManager.instance.updateCardDisplay(cardModel);
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
        List<CardModelSO> hand = HandManager.instance.getCurrentHand();

        int multiplier = 1;

        foreach (CardModelSO cardModel in hand)
        {

            if (cardModel.type == CardType.Buff)
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
        List<CardModelSO> hand = HandManager.instance.getCurrentHand();

        int multiplier = 1;

        foreach (CardModelSO cardModel in hand)
        {

            if (cardModel.type == CardType.Attack)
                multiplier++;
        }

        effects[0].value *= multiplier;

        return effects;
    }
}