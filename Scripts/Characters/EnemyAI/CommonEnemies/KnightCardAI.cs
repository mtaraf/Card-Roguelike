using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightCardAI: EnemyCardAI
{
    public KnightCardAI(Enemy enemy)
    {
        character = enemy;
        path = "ScriptableObjects/Cards/Enemies/RegularEnemies/Knight/";
        setMovesets();
        scaleMovesets(2.0);
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

