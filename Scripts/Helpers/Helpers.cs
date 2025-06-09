using UnityEngine;

public class Helpers
{
    public static GameObject findDescendant(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject found = findDescendant(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}