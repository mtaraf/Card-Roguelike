using System.Collections;
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

    public IEnumerator playCards(int multiplier, int energy)
    {
        // Get all movesets with the number of cards equal to energy
        List<DeckModelSO> applicableMovesets = movesets.FindAll((moveset) => moveset.cards.Count == energy);
        if (applicableMovesets.Count == 0)
        {
            Debug.Log("No applicable movesets with " + energy + " energy for this enemy");
        }

        // Get all the cards from a random moveset within the applicable movesets
        int randomMoveset = Random.Range(0, applicableMovesets.Count);
        yield return StartCoroutine(playAndProcessCards(applicableMovesets[randomMoveset].cards, multiplier));

        currentMoveset.Clear();
    }

    IEnumerator playAndProcessCards(List<CardModelSO> moveset, int multiplier)
    {
        Debug.Log(moveset.Count);
        for (int i = 0; i < moveset.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            CardModelSO cardModel = moveset[i];
            cardModel.multiplyValues(multiplier);

            CardEffects effects = HandManager.instance.processEnemyCard(cardModel);
            if (effects != null)
            {
                processCardEffects(effects);
            }
            // Card animation
            yield return StartCoroutine(HandManager.instance.enemyCardAnimation(cardModel));
        }
    }
}