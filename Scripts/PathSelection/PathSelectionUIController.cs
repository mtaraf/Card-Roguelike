using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PathSelectionUIController : MonoBehaviour
{
    private GameObject scrollView;
    private GameObject scrollViewContent;
    [SerializeField] private GameObject pathSelectionIcon;
    [SerializeField] private Vector2 mapStartingPosition;
    private float height;


    public void Initialize()
    {
        scrollView = GameObject.FindGameObjectWithTag("PathSelectionScrollView");
        scrollViewContent = scrollView.transform.GetChild(0).transform.GetChild(0).gameObject;
        RectTransform rectTransform = scrollView.GetComponent<RectTransform>();
        mapStartingPosition = new Vector2(-(rectTransform.sizeDelta.x / 2) + 200, 0);
        height = rectTransform.sizeDelta.y;

        pathSelectionIcon = Resources.Load<GameObject>("UI/PathSelectionUI/PathIconContainer");
    }

    public void fillUIWithCurrentEncounterMap(EncounterMap map, int levels)
    {
        GameObject icon;

        float x_position = mapStartingPosition.x;
        float y_position = 0;
        int numberOfNodesOnLevel;
        List<EncounterNode> levelNodes = new List<EncounterNode>();

        // Adjust the scroll view width
        RectTransform scrollViewContentRect = scrollViewContent.GetComponent<RectTransform>();
        scrollViewContentRect.sizeDelta = new Vector2(x_position + (200*levels) + 200, scrollViewContentRect.sizeDelta.y);

        // Creating all the icons
        for (int i = 0; i < levels; i++)
        {
            levelNodes = map.nodes.FindAll((node) => node.level == i);
            numberOfNodesOnLevel = levelNodes.Count;
            if (numberOfNodesOnLevel == 1)
            {
                icon = Instantiate(pathSelectionIcon, scrollViewContent.transform);
                icon.transform.localPosition = new Vector2(x_position, mapStartingPosition.y);
                StartCoroutine(icon.GetComponent<PathSelectionIcon>().instantiateIcon(levelNodes[0].type, levelNodes[0].completed, levelNodes[0].encounterReward, levelNodes[0].id));
            }
            else
            {
                y_position = -125 * numberOfNodesOnLevel + 125; // 5: 500 4: 375, 3: 250
                foreach (EncounterNode node in levelNodes)
                {
                    icon = Instantiate(pathSelectionIcon, scrollViewContent.transform);
                    icon.transform.localPosition = new Vector2(x_position, y_position);
                    StartCoroutine(icon.GetComponent<PathSelectionIcon>().instantiateIcon(node.type, node.completed, node.encounterReward, node.id));
                    y_position += 250;
                }
            }
            x_position += 350;
        }

        // Creating all the arrows indicators
    }
}
