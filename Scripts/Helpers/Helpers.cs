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

    public static GameObject findSiblingByTag(string tag, Transform parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("This object has no parent.");
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.gameObject.CompareTag(tag))
            {
                return child.gameObject;
            }
        }

        Debug.LogWarning($"No sibling found with tag: {tag}");
        return null;
    }

    public static T GetRandomEnumValue<T>()
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));
        return values[Random.Range(0, values.Length)];
    }
}