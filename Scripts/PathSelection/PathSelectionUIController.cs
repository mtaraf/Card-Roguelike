using System;
using System.Collections.Generic;
using UnityEngine;

public class PathSelectionUIController : MonoBehaviour
{
    private GameObject grid;
    private List<Tuple<int, int>> iconPositions = new List<Tuple<int, int>>();
    private GameObject pathSelectionIconPrefab;
    private GameObject mainCanvas;



    public void Initialize(List<Tuple<EncounterType, EncounterReward>> options)
    {
        iconPositions.Add(new Tuple<int, int>(-105, -120));
        iconPositions.Add(new Tuple<int, int>(325, -120));
        iconPositions.Add(new Tuple<int, int>(760, -120));

        mainCanvas = GameObject.Find("MainCanvas");
        grid = GameObject.Find("Grid");
        pathSelectionIconPrefab = Resources.Load<GameObject>("UI/PathSelectionUI/PathIconContainer");

        if (grid == null || mainCanvas == null)
        {
            Debug.LogError("Path Selection UI Error: Could not find grids");
        }
        fillIcons(options);
    }

    private void fillIcons(List<Tuple<EncounterType, EncounterReward>> options)
    {
        GameObject icon;
        
        for (int i=0; i < options.Count; i++)
        {
            icon = Instantiate(pathSelectionIconPrefab, mainCanvas.transform);
            icon.transform.localPosition = new Vector2(iconPositions[i].Item1, iconPositions[i].Item2);
            StartCoroutine(icon.GetComponent<PathSelectionIcon>().instantiateIcon(options[i].Item1, options[i].Item2));
        }
    }
}
