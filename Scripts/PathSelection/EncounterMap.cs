using System.Collections.Generic;

[System.Serializable]
public class EncounterMap
{
    public int currentEncounterId;
    public List<EncounterNode> nodes = new List<EncounterNode>();
}

[System.Serializable]
public class EncounterNode
{
    public int id;
    public EncounterType type;
    public List<int> progressPaths;
    public bool completed;
}