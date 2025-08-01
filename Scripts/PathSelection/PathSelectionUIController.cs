using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PathSelectionUIController : MonoBehaviour
{
    private GameObject scrollView;
    private GameObject scrollViewContent;
    [SerializeField] private GameObject pathSelectionIcon;
    [SerializeField] private GameObject arrowIndicator;
    [SerializeField] private Vector2 mapStartingPosition;
    private Dictionary<int, GameObject> nodeIdToGameObject = new();
    private float height;


    public void Initialize()
    {
        scrollView = GameObject.FindGameObjectWithTag("PathSelectionScrollView");
        scrollViewContent = scrollView.transform.GetChild(0).transform.GetChild(0).gameObject;
        RectTransform rectTransform = scrollView.GetComponent<RectTransform>();
        mapStartingPosition = new Vector2(200, -(rectTransform.sizeDelta.y / 2));
        height = rectTransform.sizeDelta.y;

        pathSelectionIcon = Resources.Load<GameObject>("UI/PathSelectionUI/PathIconContainer");
        arrowIndicator = Resources.Load<GameObject>("UI/PathSelectionUI/ArrowIndicator");
    }

    public void fillUIWithCurrentEncounterMap(EncounterMap map, int levels)
    {
        if (map == null)
        {
            Debug.LogError("Map is null when trying to fill ui");
            return;
        }
        List<int> interactableNodeIds;
        EncounterNode currentNode = map.nodes.Find((node) => node.id == map.currentEncounterId);
        if (currentNode != null)
        {
            interactableNodeIds = currentNode.progressPathIds;
        }
        else
        {
            interactableNodeIds = new List<int>{0};
        }
        GameObject icon;

        float x_position = mapStartingPosition.x;
        float y_position = 0;
        int numberOfNodesOnLevel;
        List<EncounterNode> levelNodes = new List<EncounterNode>();

        // Adjust the scroll view width
        RectTransform scrollViewContentRect = scrollViewContent.GetComponent<RectTransform>();
        scrollViewContentRect.sizeDelta = new Vector2(x_position + (200 * levels) + 1000, scrollViewContentRect.sizeDelta.y);

        // Creating all the icons
        for (int i = 0; i < levels; i++)
        {
            levelNodes = map.nodes.FindAll((node) => node.level == i);
            numberOfNodesOnLevel = levelNodes.Count;

            if (numberOfNodesOnLevel == 1)
            {
                icon = Instantiate(pathSelectionIcon, scrollViewContent.transform);
                icon.transform.localPosition = new Vector2(x_position, mapStartingPosition.y);
                EncounterNode node = levelNodes[0];
                StartCoroutine(icon.GetComponent<PathSelectionIcon>().instantiateIcon(node.type, node.completed, node.encounterReward, node.id, interactableNodeIds.Contains(node.id)));
                nodeIdToGameObject[node.id] = icon;
            }
            else
            {
                y_position = -125 * numberOfNodesOnLevel - 595;
                foreach (EncounterNode node in levelNodes)
                {
                    icon = Instantiate(pathSelectionIcon, scrollViewContent.transform);
                    icon.transform.localPosition = new Vector2(x_position, y_position);
                    StartCoroutine(icon.GetComponent<PathSelectionIcon>().instantiateIcon(node.type, node.completed, node.encounterReward, node.id, interactableNodeIds.Contains(node.id)));
                    y_position += 250;
                    nodeIdToGameObject[node.id] = icon;
                }
            }
            x_position += 450;
        }

        foreach (EncounterNode node in map.nodes)
        {
            if (!nodeIdToGameObject.ContainsKey(node.id)) continue;
            GameObject fromIcon = nodeIdToGameObject[node.id];

            foreach (EncounterNode targetNode in node.progressPaths)
            {
                if (!nodeIdToGameObject.ContainsKey(targetNode.id)) continue;
                GameObject toIcon = nodeIdToGameObject[targetNode.id];
                createArrowBetween(fromIcon.GetComponent<RectTransform>(), toIcon.GetComponent<RectTransform>());
            }
        }
    }

    private void createArrowBetween(RectTransform from, RectTransform to)
    {
        GameObject arrow = Instantiate(arrowIndicator, scrollViewContent.transform);
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();

        // Add 85 to start/end outside the icon container
        Vector3 start = from.localPosition;
        start.x += 95;
        Vector3 end = to.localPosition;
        end.x -= 95;
        Vector3 direction = end - start;

        float distance = direction.magnitude;
        Vector3 midPoint = (start + end) / 2;

        arrowRect.localPosition = midPoint;
        arrowRect.sizeDelta = new Vector2(distance, arrowRect.sizeDelta.y); // Stretch arrow
        arrowRect.rotation = Quaternion.FromToRotation(Vector3.right, direction.normalized);
        arrow.transform.SetAsFirstSibling();
    }
}
