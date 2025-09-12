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

    private ParentSceneController sceneController;

    public void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    public void Initialize(Player player, List<Enemy> enemies)
    {
        this.player = player;
        this.enemies = enemies;
        sceneController = GameManager.instance.getCurrentSceneController();
    }

    // Culver Scene Turns
    public void startCulver(int turns)
    {
        Debug.Log("Start Culver Turns: " + turns);
        StartCoroutine(culverTurnLoop(turns));
    }

    public IEnumerator culverTurnLoop(int turns)
    {
        int currentTurn = 1;
        while (currentTurn <= turns && enemies.Count > 0)
        {
            yield return StartCoroutine(playerCulverTurn());
            endPlayerTurn();

            CulverSceneController.instance.updateTurnCount(currentTurn);
            yield return new WaitForSeconds(1.5f);
        }

        if (enemies.Count == 0)
            StartCoroutine(GameManager.instance.encounterVictory(goldEarned));
    }

    private IEnumerator playerCulverTurn()
    {
        Debug.Log("Player Culver Turn");
        yield return new WaitForSeconds(0.2f);

        player.processStartOfTurnEffects();
        CulverSceneController.instance.resetPlayerEnergy();
        HandManager.instance.drawCards(GameManager.instance.getPlayerHandSize());

        endPlayerTurnBool = false;
        yield return new WaitUntil(() => endPlayerTurnBool || enemies.Count == 0);
    }


    // Base Scene Turns
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
        sceneController.resetPlayerEnergy();
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