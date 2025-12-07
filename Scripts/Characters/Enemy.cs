using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum EnemyName
{
    Samurai,
    Knight
}

public class Enemy : Character
{
    [SerializeField] private GameObject energyUI;
    [SerializeField] private EnemyName enemyName;
    protected EnemyCardProcessor enemyCardProcessor;
    protected DeckModelSO upcomingMoveSet;
    protected EnemyCardAI enemyCardAI;


    // Attributes
    private int currentEnergy = 0;

    public override void Start()
    {
        base.Start();
        currentHealth = maxHealth;

        StartCoroutine(findComponentsAfterFrame());
    }

    private IEnumerator findComponentsAfterFrame()
    {
        yield return null;
        setAIandCardProcessor();
        if (sceneController == null)
        {
            Debug.LogError($"Could not find scene controller for enemy: {gameObject.name}");
        }

        decideUpcomingMoveset();
    }

    public virtual void setAIandCardProcessor()
    {
        switch (enemyName)
        {
            case EnemyName.Samurai:
                enemyCardAI = new SamuraiCardAI(this);
                enemyCardProcessor = new SamuraiCardProcessor(sceneController);
                break;
            case EnemyName.Knight:
                enemyCardAI = new KnightCardAI(this);
                enemyCardProcessor = new KnightCardProccesor(sceneController);
                break;
            default:
                Debug.LogError($"Could not find AI/Card processor for {gameObject.name}");
                break;
        }
    }

    public void Update()
    {
        if (currentHealth < 1 && dead)
        {
            sceneController.removeDeadEnemy(id);
            dead = false;
            animator.SetTrigger("death");
        }
    }

    // Check if selected card can target self, display visual indicator if so
    // private void checkSelectedCard()
    // {
    //     if (HandManager.instance.hasSelectedCard())
    //     {
    //         Card card = HandManager.instance.getSelectedCard();
    //     }
    // }

    public override void processCardEffects(List<CardEffect> effects, Enemy enemy = null)
    {
        base.processCardEffects(effects, enemy);
    }

    // Sets current energy and updates energyUI
    public void setEnergy(int energy)
    {
        currentEnergy = energy;


        for (int i = 0; i < 3; i++)
        {
            energyUI.transform.GetChild(i).gameObject.SetActive(i < energy);
        }
    }

    public int getEnergy()
    {
        return currentEnergy;
    }

    public void decideUpcomingMoveset()
    {
        upcomingMoveSet = enemyCardAI.generateNextRoundMoves(sceneController.getPlayerAttributes());
        setEnergy(upcomingMoveSet.cards.Count);
    }

    public DeckModelSO getPlayerCurrentDeck()
    {
        return sceneController.getCurrentPlayerDeck();
    }

    public IEnumerator playCards(int multiplier)
    {
        yield return StartCoroutine(playAndProcessCards(upcomingMoveSet, multiplier));
    }

    IEnumerator playAndProcessCards(DeckModelSO moveset, int multiplier)
    {
        foreach (CardModelSO card in moveset.cards)
        {
            Debug.Log($"{gameObject.name} played card: {card.title}");
            if (dead)
            {
                yield break;
            }

            yield return new WaitForSeconds(1.0f);

            card.multiplyValues(multiplier);
            List<CardEffect> effects = enemyCardProcessor.processCard(card, attributes, this);

            if (effects != null && effects.Count > 0)
            {
                processCardEffects(effects);
            }

            setEnergy(currentEnergy-1);
            
            // Card animation
            yield return StartCoroutine(HandManager.instance.enemyCardAnimation(card));
        }
    }
}