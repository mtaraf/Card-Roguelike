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
        switch (specialCard.getCardTitle())
        {
            case "Glass Canon":
                // Example: reduce HP to 1 and deal massive damage
                break;
        }

        return specialCard.getEffects();
    }
}