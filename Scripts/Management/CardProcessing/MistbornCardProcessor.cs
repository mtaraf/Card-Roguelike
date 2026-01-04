using System;
using System.Collections.Generic;
using UnityEngine;


public class MistbornCardProcessor : CardProcessor
{
    private Dictionary<string, MistbornSpecialCardLogicInterface> mistBornspecialCards;
    private Dictionary<string, SpecialCardLogicInterface> specialCards;
    private Tuple<bool, int> bloodLust = new Tuple<bool, int>(false, 0);

    public MistbornCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        mistBornspecialCards = new Dictionary<string, MistbornSpecialCardLogicInterface>
        {
            // Corruption
            {"Cleanse", new CleanseLogic()},
            {"Cleanse+", new CleanseLogic()},
            {"Corrupt Dagger", new CorruptDaggerLogic()},
            {"Corrupt Dagger+", new CorruptDaggerLogic()},
            {"Greed", new GreedLogic()},
            {"Greed+", new GreedLogic()},
            {"Plague", new PlagueLogic(2)},
            {"Plague+", new PlagueLogic(3)},
            {"Corruption Shroud", new CorruptionShroudLogic()},
            {"Corruption Shroud+", new CorruptionShroudLogic()},
            {"Crescendo", new CrescendoLogic()},
            {"Crescendo+", new CrescendoLogic()},
            // Bleed
            {"Bloodlust", new BloodlustLogic()},
            {"Bloodlust+", new BloodlustLogic()},
            {"Taste of Blood", new TasteOfBloodLogic()},
            {"Taste of Blood+", new TasteOfBloodLogic()},
            {"Vampiric", new VampiricLogic()},
            {"Vampiric+", new VampiricLogic()},
            {"Bleed It Out", new BleedItOutLogic()},
            {"Bleed It Out+", new BleedItOutLogic()},
            {"Ravage", new RavageLogic()},
            {"Ravage+", new RavageLogic()},
            {"Transfusion", new TransfusionLogic()},
            {"Transfusion+", new TransfusionLogic()},
            {"Blood Burst", new BloodBurstLogic()},
            {"Blood Burst+", new BloodBurstLogic()},
            // Crit
            {"Showdown", new ShowdownLogic()},
            {"Showdown+", new ShowdownLogic()},
            {"Lucky Seven", new LuckySevenLogic(1)},
            {"Lucky Seven+", new LuckySevenLogic(2)},
            {"Luck of the Draw", new LuckOfTheDrawLogic(1)},
            {"Luck of the Draw+", new LuckOfTheDrawLogic(2)},
            {"Teamwork", new TeamworkLogic()},
            {"Teamwork+", new TeamworkLogic()},
            {"Assassin's Mark", new AssassinsMarkLogic(2)},
            {"Assassin's Mark+", new AssassinsMarkLogic(2)},
            {"Agile Focus", new AgileFocusLogic()},
            {"Agile Focus+", new AgileFocusLogic()},
            {"Critical Success", new CriticalSuccessLogic()},
            {"Critical Success+", new CriticalSuccessLogic()},
            // Dagger
            {"Load Out", new LoadOutLogic()},
            {"Load Out+", new LoadOutLogic()},
            {"Close Combat", new CloseCombatLogic()},
            {"Close Combat+", new CloseCombatLogic()},
            {"Dance of Daggers", new DanceOfDaggersLogic()},
            {"Dance of Daggers+", new DanceOfDaggersLogic()},
            {"Hidden Inventory", new HiddenInventoryLogic(2)},
            {"Hidden Inventory+", new HiddenInventoryLogic(3)},
        };

        specialCards = new Dictionary<string, SpecialCardLogicInterface>
        {
            // General
            {"Hunter's Instinct", new HuntersInstinctLogic()},
            {"Hunter's Instinct+", new HuntersInstinctLogic()},
            {"Stockade", new StockadeLogic()},
            {"Stockade+", new StockadeLogic()},
            {"Onslaught", new OnslaughtLogic()},
            {"Onslaught+", new OnslaughtLogic()},
            {"Gymnastics", new GymnasticsLogic()},
            {"Gymnastics+", new GymnasticsLogic()},
            {"Pewter Drag", new PewterDragLogic(true)},
            {"Pewter Drag+", new PewterDragLogic(false)},
            {"Second Chance", new SecondChanceLogic()},
            {"Second Chance+", new SecondChanceLogic()},
            {"Last Hope", new LastHopeLogic()},
            {"Last Hope+", new LastHopeLogic()}
        };
    }

    public override List<CardEffect> processCard(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        List<CardEffect> cardEffects;
        if (card.isSpecial())
        {
            sceneController.playAnimationsForCard(card.getCardType());
            cardEffects = processSpecialCard(card, attributes, enemies);
        }
        else
        {
            cardEffects = base.processCard(card, attributes, enemies);
        }

        // Check bloodlust
        if (bloodLust.Item1 && cardEffects.Find((effect) => effect.type == EffectType.Bleed) != null)
            sceneController.healPlayer(bloodLust.Item2);

        return cardEffects;
    }

    protected override List<CardEffect> processSpecialCard(Card specialCard, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        MistbornSpecialCardLogicInterface mistbornSpecialCardLogic;
        mistBornspecialCards.TryGetValue(specialCard.getCardTitle(), out mistbornSpecialCardLogic);

        List<CardEffect> cardEffects;

        if (mistbornSpecialCardLogic == null)
        {
            Debug.Log($"No Mistborn Special Card Logic for {specialCard.getCardTitle()}, checking Special Card Logic");
            SpecialCardLogicInterface specialCardLogic;
            specialCards.TryGetValue(specialCard.getCardTitle(), out specialCardLogic);

            if (specialCardLogic == null)
            {
                Debug.Log($"No Special Card Logic for {specialCard.getCardTitle()}, ignoring card");
                return new List<CardEffect>();
            }

            cardEffects = specialCardLogic.process(specialCard, attributes, enemies, sceneController, this);

            checkCritHits(cardEffects);

            return applyEffectsToCardDamage(cardEffects, attributes);
        }

        cardEffects = mistbornSpecialCardLogic.process(specialCard, attributes, enemies, sceneController, this);

        checkCritHits(cardEffects);

        return applyEffectsToCardDamage(cardEffects, attributes);
    }

    public override void endOfRoundEffects()
    {
        setBloodlust(false, 0);
    }

    public void setBloodlust(bool toggle, int heal)
    {
        bloodLust = new Tuple<bool, int>(toggle, heal);

        Debug.Log($"Bloodlust: {toggle}");

        // Maybe add an active icon to player
    }
}
