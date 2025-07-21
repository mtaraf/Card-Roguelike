using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncounterMap
{
    public int currentEncounterId = 0;
    public List<EncounterNode> nodes = new List<EncounterNode>();

    public void generateRandomMap(int levels)
    {
        int nodeId = 0;
        EncounterNode startingNode = new EncounterNode(nodeId, 0, EncounterType.Regular_Encounter);
        nodeId++;
        nodes.Add(startingNode);

        List<EncounterNode> previousLevelNodes;
        for (int i = 1; i < levels; i++)
        {
            if (i % 5 == 0)
            {
                // Forge is generated every 5 levels
                EncounterNode forgeNode = new EncounterNode(nodeId, i, EncounterType.Forge);
                nodeId++;
                nodes.Add(forgeNode);
            }
            else if (i % 5 == 1)
            {
                // Adds 3-5 encounter paths from the starting node or the forge nodes
                EncounterNode previousNode = nodes.Find((node) => node.level == i - 1);
                int random = Random.Range(3, 6);
                for (int j = 0; j < random; j++)
                {
                    EncounterNode newNode = generateRandomEncounter(nodeId, i);
                    previousNode.progressPaths.Add(newNode);
                    nodes.Add(newNode);
                    nodeId++;
                }
            }
            else if (i == levels - 1)
            {
                // Add final boss level
            }
            else
            {
                // Add 1 random encounter to all previous level nodes
                previousLevelNodes = nodes.FindAll((node) => node.level == i - 1);
                foreach (EncounterNode node in previousLevelNodes)
                {
                    EncounterNode newNode = generateRandomEncounter(nodeId, i);
                    node.progressPaths.Add(newNode);
                    nodeId++;
                    nodes.Add(newNode);
                }
            }
        }
    }

    private EncounterNode generateRandomEncounter(int nodeId, int level)
    {
        int random = Random.Range(0, 11);
        EncounterNode node;

        if (level == 1)
        {
            return new EncounterNode(nodeId, level, EncounterType.Regular_Encounter);
        }

        if (level < 6)
        {
            node = random switch
            {
                < 8 => new EncounterNode(nodeId, level, EncounterType.Regular_Encounter),
                < 10 => new EncounterNode(nodeId, level, EncounterType.Culver_Encounter),
                _ => new EncounterNode(nodeId, level, EncounterType.Tank_Encounter)
            };
        }
        else
        {
            node = random switch
            {
                < 8 => new EncounterNode(nodeId, level, EncounterType.Regular_Encounter),
                < 9 => new EncounterNode(nodeId, level, EncounterType.Culver_Encounter),
                < 10 => new EncounterNode(nodeId, level, EncounterType.Tank_Encounter),
                10 => new EncounterNode(nodeId, level, EncounterType.Mini_Boss_Encounter),
                _ => new EncounterNode(nodeId, level, EncounterType.Final_Boss)
            };
        }
        return node;
    }
}

[System.Serializable]
public class EncounterNode
{
    public int id;
    public int level;
    public EncounterType type;
    public List<EncounterNode> progressPaths = new List<EncounterNode>();
    public bool completed;

    public EncounterNode(int nodeId, int nodeLevel, EncounterType nodeType)
    {
        id = nodeId;
        type = nodeType;
        level = nodeLevel;
    }
}