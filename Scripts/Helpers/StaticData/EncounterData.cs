using System.Collections.Generic;

public static class EncounterData
{
    public static readonly Dictionary<EncounterType, EncounterInfo> InfoMap = new()
    {
        { EncounterType.Forge, new EncounterInfo("UI/Icons/forge_icon", "The Forge", "Enter the forge to upgrade or remove cards in your deck") },
        { EncounterType.Regular_Encounter, new EncounterInfo("UI/Icons/regular_encounter_icon", "Encounter", "Battle a variety of enemies and claim the rewards!") },
        { EncounterType.Mini_Boss_Encounter, new EncounterInfo("UI/Icons/mini_boss_icon", "Mini Boss", "Fight against a strong opponent to obtain rare loot!") },
        { EncounterType.Culver_Encounter, new EncounterInfo("UI/Icons/culver_encounter_icon", "Culver's Bounty", "Your task is to unleash your might upon the mighty Culver. Deal enough damage to incapacitate Culver to claim your bounty.") },
        { EncounterType.Tank_Encounter, new EncounterInfo("UI/Icons/tank_encounter_icon", "Hold the Line", "Survive the onslaught of barrages without fighting back to claim the rewards for your effort to outlast the damage.") },
        // No Final_Boss here, since you treat it specially
    };
}

public class EncounterInfo
{
    public string iconPath;
    public string title;
    public string message;

    public EncounterInfo(string path, string title, string message)
    {
        iconPath = path;
        this.title = title;
        this.message = message;
    }
}