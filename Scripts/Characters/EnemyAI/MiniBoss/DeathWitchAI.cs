using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeathWitchAI: EnemyCardAI
{
    public DeathWitchAI(Enemy enemy)
    {
        character = enemy;
        attackMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/MiniBosses/DeathWitch/Attack").ToList();
        defenseMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/MiniBosses/DeathWitch/Defense").ToList();
        specialMovesets = Resources.LoadAll<DeckModelSO>("ScriptableObjects/Cards/Enemies/MiniBosses/DeathWitch/Special").ToList();
    }

    public override DeckModelSO generateNextRoundMoves(Dictionary<EffectType, int> playerAttributes)
    {
        DeckModelSO moveset = null;

        // Enemy specific logic

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

