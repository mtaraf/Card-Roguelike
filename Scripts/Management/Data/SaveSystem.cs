using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string saveFolder = Application.persistentDataPath + "/Saves/";

    static SaveSystem()
    {
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }

    public static void saveGame(SaveData data, int slotIndex)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(getSlotPath(slotIndex), json);
        Debug.Log($"Game saved to slot {slotIndex}");
    }

    public static SaveData loadGame(int slotIndex)
    {
        string path = getSlotPath(slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"Loaded from slot {slotIndex}");
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("No save file found.");
            return null;
        }
    }

    public static SerializableCardModel convertToSerializableModel(CardModelSO model)
    {
        return new SerializableCardModel
        {
            title = model.title,
            details = model.details,
            type = model.type,
            rarity = model.rarity,
            energy = model.energy,
            effects = new List<CardEffect>(model.effects), // assuming CardEffect is serializable
            target = model.target,
            cardsDrawn = model.cardsDrawn,
            special = model.special,
            corrupts = model.corrupts
        };
    }

    public static CardModelSO convertToRuntimeCard(SerializableCardModel serial)
    {
        CardModelSO model = CardModelSO.CreateInstance<CardModelSO>();

        model.title = serial.title;
        model.details = serial.details;
        model.type = serial.type;
        model.rarity = serial.rarity;
        model.energy = serial.energy;
        model.effects = new List<CardEffect>(serial.effects);
        model.target = serial.target;
        model.cardsDrawn = serial.cardsDrawn;
        model.special = serial.special;
        model.corrupts = serial.corrupts;

        return model;
    }

    public static bool saveFileExists(int slotIndex)
    {
        return File.Exists(getSlotPath(slotIndex));
    }

    public static bool deleteSave(int slotIndex)
    {
        string path = getSlotPath(slotIndex);

        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        else
        {
            Debug.LogError($"Could not find save file to delete in slot: {slotIndex}");
            return false;
        }
    }

    public static string getSlotSummary(int slotIndex)
    {
        string path = getSlotPath(slotIndex);
        if (!File.Exists(path)) return "Empty";

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        return $"Level: {data.currentLevel} \n Gold: {data.playerGold}";
    }

    public static string getSlotTitle(int slotIndex)
    {
        string path = getSlotPath(slotIndex);
        if (!File.Exists(path)) return "Empty";

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        return data.saveName;
    }

    static string getSlotPath(int slotIndex) => $"{saveFolder}save_slot_{slotIndex}.json";
}