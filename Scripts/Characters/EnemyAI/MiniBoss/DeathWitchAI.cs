using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeathWitchAI: EnemyCardAI
{
    public DeathWitchAI(Enemy enemy)
    {
        character = enemy;
        path = "ScriptableObjects/Cards/Enemies/MiniBosses/DeathWitch/";
        setMovesets();
    }

    public override DeckModelSO generateNextRoundMoves(Dictionary<EffectType, int> playerAttributes)
    {
        DeckModelSO moveset = null;

        // Enemy specific logic
        checkForHakuCardsInPlayerDeck();

        if (moveset != null)
        {
            return moveset;
        }
        else
        {
            return base.generateNextRoundMoves(playerAttributes);
        }
    }

    private void checkForHakuCardsInPlayerDeck()
    {
        DeckModelSO playerCurrentDeck = HandManager.instance.getPlayerDeck();

        List<CardModelSO> hakuCards = playerCurrentDeck.cards.FindAll((card) => card.title == "Haku's Curse");

        int count = hakuCards.Count;

        character.addAttributeValue(EffectType.Strength, count);
    }
}

