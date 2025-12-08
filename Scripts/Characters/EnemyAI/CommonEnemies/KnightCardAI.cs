using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightCardAI: EnemyCardAI
{
    public KnightCardAI(Enemy enemy)
    {
        character = enemy;
        attackMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Knight/Attack").ToList();
        defenseMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Knight/Defense").ToList();
        specialMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Knight/Special").ToList();
    }

    public override DeckModelSO generateNextRoundMoves(Dictionary<EffectType, int> playerAttributes)
    {
        DeckModelSO moveset = null;

        // Knight Specific Logic


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

