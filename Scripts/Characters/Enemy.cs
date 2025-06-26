using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum EnemyClass
{
    Minion,
    Fighter,
    Mage,
    Healer
}

public class Enemy : Character
{
    [SerializeField] private int moveset;
    [SerializeField] private EnemyClass enemyClass;
    private List<CardModelSO> currentMoveset = new List<CardModelSO>();
    [SerializeField] private List<DeckModelSO> movesets = new List<DeckModelSO>();

    // Attributes

    public override void Start()
    {
        base.Start();
    }

    // Check if selected card can target self, display visual indicator if so
    private void checkSelectedCard()
    {
        if (HandManager.instance.hasSelectedCard())
        {
            Card card = HandManager.instance.getSelectedCard();
        }
    }

    public override void processCardEffects(CardEffects effects)
    {
        base.processCardEffects(effects);
    }

    public EnemyClass getClass()
    {
        return enemyClass;
    }

    public CardEffects playCard(int multiplier)
    {
        // if enemy does not have a current moveset, add one
        if (currentMoveset.Count == 0)
        {
            int randomMoveset = Random.Range(0, movesets.Count);
            foreach (CardModelSO model in movesets[randomMoveset].cards)
            {
                currentMoveset.Add(model);
            }
        }


        int random = Random.Range(0, currentMoveset.Count);
        Card card = new Card();
        card.setCardDisplayInformation(currentMoveset[0]);
        card.mulitplyValues(multiplier);

        // Card animation
        StartCoroutine(HandManager.instance.enemyCardAnimation(currentMoveset[0]));

        // Remove move from current moveset
        currentMoveset.Remove(currentMoveset[0]);

        return HandManager.instance.processCard(card);
    }
}
