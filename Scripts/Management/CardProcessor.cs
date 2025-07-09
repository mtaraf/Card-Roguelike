using System.Collections.Generic;

public class CardProcessor
{
    private readonly BaseLevelSceneController sceneController;

    public CardProcessor(BaseLevelSceneController controller)
    {
        sceneController = controller;
    }

    public List<CardEffect> processCard(Card card, Dictionary<EffectType, int> attributes)
    {
        if (card.isSpecial())
        {
            return processSpecialCard(card);
        }

        int draw = card.getCardsToDraw();

        if (draw > 0)
        {
            HandManager.instance.drawCards(draw);
        }

        sceneController.playAnimationsForCard(card.getCardType());
        return card.getEffects();
    }

    public List<CardEffect> processEnemyCard(CardModelSO model, Dictionary<EffectType, int> attributes)
    {
        if (model.target == Target.Player)
        {
            sceneController.processEnemyCardEffectsOnPlayer(model.effects);
            return null;
        }

        return model.effects;
    }

    private List<CardEffect> processSpecialCard(Card specialCard)
    {
        List<CardEffect> cardEffects = new List<CardEffect>();
        switch (specialCard.getCardTitle())
        {
            case "Stacked Hand":
                CardEffect cardDamage = new CardEffect();
                cardDamage.type = EffectType.Damage;
                cardDamage.value = (HandManager.instance.getNumCardsInHand() * 2) - 1;
                cardDamage.turns = 0;
                cardEffects.Add(cardDamage);
                break;
        }

        return cardEffects;
    }
}