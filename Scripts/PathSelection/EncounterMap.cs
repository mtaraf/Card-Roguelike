using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EncounterMap
{
    public int currentEncounterId = 0;
    public List<EncounterNode> nodes = new List<EncounterNode>();

    public EncounterNode getNode(int id)
    {
        return nodes.Find((node) => node.id == id);
    }

    public void rebuildPaths()
    {
        Dictionary<int, EncounterNode> nodeLookup = new();
        foreach (EncounterNode node in nodes)
        {
            nodeLookup[node.id] = node;
        }

        foreach (EncounterNode node in nodes)
        {
            node.progressPaths = new List<EncounterNode>();
            foreach (int pathId in node.progressPathIds)
            {
                if (nodeLookup.TryGetValue(pathId, out EncounterNode targetNode))
                {
                    node.progressPaths.Add(targetNode);
                }
            }
        }
    }

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
                previousLevelNodes = nodes.FindAll((node) => node.level == i - 1);
                foreach (EncounterNode node in previousLevelNodes)
                {
                    node.progressPaths.Add(forgeNode);
                    node.progressPathIds.Add(forgeNode.id);
                }
            }
            else if (i % 5 == 1)
            {
                // Adds 3-5 encounter paths from the starting node or the forge nodes
                EncounterNode previousNode = nodes.Find((node) => node.level == i - 1);
                int random = UnityEngine.Random.Range(3, 6);
                for (int j = 0; j < random; j++)
                {
                    EncounterNode newNode = generateRandomEncounter(nodeId, i);
                    previousNode.progressPaths.Add(newNode);
                    previousNode.progressPathIds.Add(newNode.id);
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
                    node.progressPathIds.Add(newNode.id);
                    nodeId++;
                    nodes.Add(newNode);
                }
            }
        }
    }

    private EncounterNode generateRandomEncounter(int nodeId, int level)
    {
        int random = UnityEngine.Random.Range(0, 11);
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

        generateRandomReward(node);

        return node;
    }

    // Rewards weighted mostly towards gold -> card rarity -> card choice
    void generateRandomReward(EncounterNode node)
    {
        if (node.type != EncounterType.Forge && node.type != EncounterType.Final_Boss)
        {
            node.encounterReward = Helpers.GetRandomEnumValue<EncounterReward>();
        }
    }
}

[Serializable]
public class EncounterNode
{
    public int id;
    public int level;
    public EncounterType type;
    [NonSerialized] public List<EncounterNode> progressPaths = new List<EncounterNode>();
    public List<int> progressPathIds = new();
    public bool completed;
    public EncounterReward encounterReward;
    public int rewardValue;

    public EncounterNode(int nodeId, int nodeLevel, EncounterType nodeType)
    {
        id = nodeId;
        type = nodeType;
        level = nodeLevel;
    }
}