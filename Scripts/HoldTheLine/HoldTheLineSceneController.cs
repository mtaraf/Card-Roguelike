using System.Collections;
using UnityEngine;

public class HoldTheLineSceneController : ParentSceneController
{
    public static HoldTheLineSceneController instance;

    [SerializeField] private GameObject holdTheLineBoss;
    private HoldTheLineUIController holdTheLineUIController;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public override void Start()
    {
        holdTheLineUIController = gameObject.AddComponent<HoldTheLineUIController>();
        InitializeScene();
    }

    public override void InitializeScene()
    {
        GameManager.instance.setCurrentSceneController(instance);
        base.InitializeScene();
        holdTheLineUIController.Initialize();

        // Spawn Boss
        GameObject mainBoss = Instantiate(holdTheLineBoss, enemySpawnLocation.transform);
        Enemy bossComponent = mainBoss.GetComponent<Enemy>();
        enemies.Add(bossComponent);

        // Start turn sequence
        TurnManager.instance.Initialize(player, enemies);
        TurnManager.instance.startRoundBasedTurnLoop(3); // TO-DO: make this dynamically scale with level
    }

    public override void updateCurrentRound(int currentRound, int totalTurns)
    {
        holdTheLineUIController.updateRoundCount(currentRound + 1, totalTurns);
    }
}
