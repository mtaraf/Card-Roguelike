using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCardAI
{
    public int currentRound = 0;
    public List<DeckModelSO> attackMovesets = new List<DeckModelSO>();
    public List<DeckModelSO> defenseMovesets = new List<DeckModelSO>();
    public List<DeckModelSO> specialMovesets = new List<DeckModelSO>();
    public string path;
    public Enemy character;

    public void setMovesets()
    {
        Debug.Log(path);
        List<DeckModelSO> attacks = Resources.LoadAll<DeckModelSO>(path + "Test").ToList();
        foreach (DeckModelSO attack in attacks)
        {
            attackMovesets.Add(attack.clone());
        }

        List<DeckModelSO> defenses = Resources.LoadAll<DeckModelSO>(path + "Test").ToList();
        foreach (DeckModelSO defense in defenses)
        {
            defenseMovesets.Add(defense.clone());
        }

        List<DeckModelSO> specials = Resources.LoadAll<DeckModelSO>(path + "Test").ToList();
        foreach (DeckModelSO special in specials)
        {
            specialMovesets.Add(special.clone());
        }
    }

    public void scaleMovesets(double scaling)
    {
        foreach(DeckModelSO deck in attackMovesets)
        {
            deck.scaleCards(scaling);
        }

        foreach(DeckModelSO deck in defenseMovesets)
        {
            deck.scaleCards(scaling);
        }

        foreach(DeckModelSO deck in specialMovesets)
        {
            deck.scaleCards(scaling);
        }
    }

    public virtual DeckModelSO generateNextRoundMoves(Dictionary<EffectType, int> playerAttributes)
    {
        DeckModelSO moveset;

        // Decide Attack or Defensive
        if (attackFocuesRound())
            moveset = randomMoveset(attackMovesets);
        else
            moveset = randomMoveset(defenseMovesets);

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