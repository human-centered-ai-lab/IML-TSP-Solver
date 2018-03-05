using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;

public class PheromoneController : Singleton<PheromoneController>
{

    private static List<GameObject> lineObjects;
    private static GameObject _linePrefab;

    public static void Init()
    {
        lineObjects = new List<GameObject>();

        if (_linePrefab == null)
        {
            _linePrefab = Resources.Load("Prefabs/line") as GameObject;
        }
    }

    public static void MakeConnections(AntAlgorithms.AntAlgorithm antAlgorithm)
    {
        float pheromoneMaxValue = 0f;
        int counter = 0;

        for (int i = 0; i < AntAlgorithmManager.Instance.Cities.Count; i++)
        {
            for (int j = (i + 1); j < AntAlgorithmManager.Instance.Cities.Count; j++)
            {
                float currentPheromone = (float)antAlgorithm.Pheromones.GetPheromone(i, j);
                if (pheromoneMaxValue < currentPheromone)
                    pheromoneMaxValue = currentPheromone;
            }
        }
        for (int i = 0; i < AntAlgorithmManager.Instance.Cities.Count; i++)
        {
            for (int j = (i + 1); j < AntAlgorithmManager.Instance.Cities.Count; j++)
            {
                GameObject gameObject1 = CityController.cityObjects[i];
                GameObject gameObject2 = CityController.cityObjects[j];
                lineObjects.Add(Instantiate(_linePrefab));
                float w1 = ((float)antAlgorithm.Pheromones.GetPheromone(i, j) / pheromoneMaxValue) * 2.0f;
                lineObjects[counter].GetComponent<LineRenderer>().SetPosition(0, gameObject1.transform.position);
                lineObjects[counter].GetComponent<LineRenderer>().SetPosition(1, gameObject2.transform.position);
                lineObjects[counter].name = "pheromone" + i + "_" + j;
                AddColliderToLine(lineObjects[counter], gameObject2.transform.position, gameObject1.transform.position, 1);
                PheromoneData pd = lineObjects[counter].AddComponent<PheromoneData>();
                pd.from = i;
                pd.to = j;
                pd.name = "Pheromone " + i + " to " + j + ": ";
                pd.value = (float)antAlgorithm.Pheromones.GetPheromone(i, j);
                // GameObject.FindGameObjectWithTag("infoText").GetComponent<Text>().text = "Pheromone from " + i + " to " + j + ":\n value: " + w1;
                lineObjects[counter].GetComponent<LineRenderer>().startWidth = w1;
                lineObjects[counter].GetComponent<LineRenderer>().endWidth = w1;
                counter++;
            }
        }
    }
    public static void ClearConnections()
    {
        if (lineObjects != null)
        {
            for (int i = 0; i < lineObjects.Count; i++)
            {
                Destroy(lineObjects[i]);
            }
            lineObjects.Clear();
        }
    }

    public static void HideConnections(bool flag)
    {
        for (int i = 0; i < lineObjects.Count; i++)
        {
            lineObjects[i].SetActive(flag);
        }
    }


    private static void AddColliderToLine(GameObject line, Vector3 startPos, Vector3 endPos, float colliderThikness)
    {
        BoxCollider col = line.AddComponent<BoxCollider>();
        col.transform.SetParent(line.GetComponent<LineRenderer>().transform);
        float lineLength = Vector3.Distance(startPos, endPos);
        col.center = new Vector3(0, 0, 0.5f);
        col.size = new Vector3(lineLength, colliderThikness + 0.5f, 1f);
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint;
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        col.transform.Rotate(0, 0, angle);
    }
}
