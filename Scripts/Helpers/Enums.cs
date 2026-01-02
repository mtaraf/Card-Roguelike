public class Enums
{
    public static string mapBuildIndex(SceneBuildIndex sceneBuildIndex)
    {
        switch (sceneBuildIndex)
        {
            case SceneBuildIndex.MAIN_MENU:
                return "MainMenuScene";
            case SceneBuildIndex.BASE_LEVEL:
                return "BaseLevelScene";
            case SceneBuildIndex.PATH_SELECTION:
                return "PathSelectionScene";
            case SceneBuildIndex.FORGE:
                return "ForgeScene";
            case SceneBuildIndex.HOLD_THE_LINE:
                return "HoldTheLine";
            case SceneBuildIndex.CHARACTER_SELECTION:
                return "CharacterSelectionScene";
            case SceneBuildIndex.CULVER:
                return "CulverScene";
            case SceneBuildIndex.MINI_BOSS:
                return "MiniBossScene";
            default:
                return "";
        }
    }
}

public enum SceneBuildIndex
{
    MAIN_MENU = 0,
    BASE_LEVEL = 1,
    PATH_SELECTION = 2,
    FORGE = 3,
    HOLD_THE_LINE = 4,
    CHARACTER_SELECTION = 5,
    CULVER = 6,
    MINI_BOSS = 7,
    FINAL_BOSS = 8
}

public enum DeckViewType
{
    FullDeck,
    DiscardPile,
    DrawPile
}

public enum BossType
{
    HoldTheLine,
    Culver,
    FinalBoss,
    MiniBoss
}

public enum EnemyName
{
    Samurai,
    Knight
}

public enum PlayerClass
{
    Paladin,
    Mistborn
}

public enum EffectExecutionType
{
    Turn_Based,
    Value_Based,
}

public enum DamageType
{
    Critical,
    Bleed,
    Corruption,
    Heal,
    General
}

public enum ToolTipDirection
{
    Above,
    Below,
    Left,
    Right,
}

public enum ToolTipSize
{
    Large,
    Medium,
    Small
}

public enum EffectType
{
    Damage,
    Armor,
    Strength,
    Weaken,
    Blind,
    Divinity,
    Poison,
    Frostbite,
    HealDamageDone,
    Heal,
    HealOverTime,
    Stun,
    Bleed,
    Agility,
    Corruption
}

public enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

public enum EncounterType
{
    Forge,
    Regular_Encounter,
    Mini_Boss_Encounter,
    Culver_Encounter,
    Hold_The_Line_Encounter,
    Final_Boss
}

public enum EncounterReward
{
    CardRarity,
    CardChoices,
    Gold,
    MaxHealth
}

public enum CardType
{
    Attack,
    Defense,
    Buff,
    Resource,
    Unique,
    Special
}

public enum CardRarity
{
    Starter,
    COMMON,
    RARE,
    EPIC,
    MYTHIC
}

public enum ConditionMetric
{
    HEALTH,
    ARMOR,
    APPLIED_EFFECT,
    PLAYER_CURRENT_EFFECT,
    NO_CONDITION
}

public enum Target
{
    Enemy_Multiple,
    Enemy_Single,
    Player
}