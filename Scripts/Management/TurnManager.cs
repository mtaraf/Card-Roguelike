using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private TurnState currentState;
    private bool endPlayerTurnBool = true;

    private List<Enemy> enemies;
    private Player player;
    private int goldEarned = 20;

    public void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    public void Initialize(Player player, List<Enemy> enemies)
    {
        this.player = player;
        this.enemies = enemies;
    }

    public void startTurns()
    {
        StartCoroutine(turnLoop());
    }

    private IEnumerator turnLoop()
    {
        currentState = TurnState.PlayerTurn;
        while (enemies.Count > 0 && player.getCurrentHealth() > 1)
        {
            if (currentState == TurnState.PlayerTurn)
            {
                yield return StartCoroutine(playerTurn());
                endPlayerTurn();
                currentState = TurnState.EnemyTurn;
            }
            else
            {
                yield return StartCoroutine(enemyTurn());
                currentState = TurnState.PlayerTurn;
            }
            yield return null;
        }

        if (enemies.Count == 0)
            StartCoroutine(GameManager.instance.encounterVictory(goldEarned));
    }

    private IEnumerator playerTurn()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (Enemy enemy in enemies)
        {
            enemy.setEnergy(Random.Range(1, 4));
        }

        player.processStartOfTurnEffects();
        BaseLevelSceneController.instance.resetPlayerEnergy();
        HandManager.instance.drawCards(GameManager.instance.getPlayerHandSize());

        endPlayerTurnBool = false;
        yield return new WaitUntil(() => endPlayerTurnBool || enemies.Count == 0);
    }

    private void endPlayerTurn()
    {
        HandManager.instance.shuffleCurrentHandIntoDiscardPile();
        player.processEndOfTurnEffects();
    }

    IEnumerator enemyTurn()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Enemy enemy in enemies)
        {
            enemy.processStartOfTurnEffects();
            // TO-DO: depending on current level adjust multiplier for enemy cards
            yield return StartCoroutine(enemy.playCards(1, enemy.getEnergy()));
            enemy.processEndOfTurnEffects();
        }
        yield return new WaitForSeconds(0.5f);
    }

    public void endTurnButtonPressed()
    {
        if (currentState == TurnState.PlayerTurn)
            endPlayerTurnBool = true;
    }

    public TurnState getTurnState() => currentState;
}