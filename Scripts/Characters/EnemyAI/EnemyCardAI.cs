using System.Collections.Generic;
using UnityEngine;

public class EnemyCardAI
{
    public int currentRound = 0;
    public List<DeckModelSO> attackMovesets = new List<DeckModelSO>();
    public List<DeckModelSO> defenseMovesets = new List<DeckModelSO>();
    public List<DeckModelSO> specialMovesets = new List<DeckModelSO>();
    public Enemy character;

    public virtual DeckModelSO generateNextRoundMoves(Dictionary<EffectType, int> playerAttributes)
    {
        DeckModelSO moveset;

        // Decide Attack or Defensive
        if (attackFocuesRound())
            moveset = randomMoveset(attackMovesets);
        else
            moveset = randomMoveset(defenseMovesets);


        // Update energy ui
        character.setEnergy(moveset.cards.Count);
        return moveset;
    }

    public bool attackFocuesRound()
    {
        int random = Random.Range(1,11);
        return random > 5;
    }

    public DeckModelSO randomMoveset(List<DeckModelSO> movesetList)
    {
        int random = Random.Range(0,movesetList.Count);
        return movesetList[random];
    }
}