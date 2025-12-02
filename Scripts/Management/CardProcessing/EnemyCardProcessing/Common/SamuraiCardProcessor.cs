using System.Collections.Generic;
using UnityEngine;


public class SamuraiCardProcessor : EnemyCardProcessor
{
    private Dictionary<string, SpecialEnemyCardLogicInterface> specialCards;

    public SamuraiCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        specialCards = new Dictionary<string, SpecialEnemyCardLogicInterface>
        {
            {"Tainted Blade", new TainedBladeLogic()},
        };
    }

    public override List<CardEffect> processCard(CardModelSO card, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        if (card.special)
        {
            sceneController.playAnimationsForCard(card.type);
            processSpecialCard(card, attributes, enemy);
            return new List<CardEffect>();
        }

        return base.processCard(card, attributes, enemy);
    }

    protected override void processSpecialCard(CardModelSO specialCard, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        SpecialEnemyCardLogicInterface specialCardLogic = specialCards[specialCard.title];

        if (specialCardLogic == null)
        {
            Debug.LogError($"No Special Card Logic for {specialCard.title}");
            return;
        }

        List<CardEffect> cardEffects = specialCardLogic.process(specialCard,attributes,sceneController, enemy);


        processSpecialCardEffects(cardEffects, specialCard.target, enemy);
    }
}

public class TainedBladeLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        

        return new List<CardEffect>();
    }
}
