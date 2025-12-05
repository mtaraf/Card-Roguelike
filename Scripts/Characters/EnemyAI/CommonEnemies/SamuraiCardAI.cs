using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SamuraiCardAI: EnemyCardAI
{
    public SamuraiCardAI(Enemy enemy)
    {
        character = enemy;
        attackMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Samurai/Movesets/Attack").ToList();
        defenseMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Samurai/Movesets/Defense").ToList();
        specialMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Samurai/Movesets/Special").ToList();
    }

    public override DeckModelSO generateNextRoundMoves(Dictionary<EffectType, int> playerAttributes)
    {
        DeckModelSO moveset = null;

        // Check player deck for Gash cards
        CardModelSO gashCardPresent = character.getPlayerCurrentDeck().cards.Find((card) => card.title == "Gash");

        if (gashCardPresent != null)
        {
            // Choose random gash based card
            List<DeckModelSO> gashMovesets = specialMovesets.FindAll((move) => move.name.Contains("Gash"));
            moveset = gashMovesets[Random.Range(0,gashMovesets.Count)];
        }

        if (moveset != null)
        {
            return moveset;
        }
        else
        {
            return base.generateNextRoundMoves(playerAttributes);
        }
    }
}

