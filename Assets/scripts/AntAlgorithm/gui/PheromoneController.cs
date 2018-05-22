/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* PheromoneController is used for pheromone ui representation*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PheromoneController : MonoBehaviour
{
    private  List<GameObject> lineObjects;
    private static GameObject _linePrefab;
    private  GameObject editCanvasPrefab;
    private GameObject editCanvas;
    private InputField editPheromoneinputField;
    private Dropdown editPheromoneDropdown;
    private int currentFrom;
    private int currentTo;
    private double currentValue;
    private int currentIndex;

    public void Init()
    {
        lineObjects = new List<GameObject>();
        if (editCanvasPrefab == null)
        {
            editCanvasPrefab = Resources.Load("Prefabs/IMLEditCanvas") as GameObject;
        }
        if (_linePrefab == null)
        {
            _linePrefab = Resources.Load("Prefabs/Line") as GameObject;
        }
    }

    public void MakeConnections(AntAlgorithms.AntAlgorithm antAlgorithm)
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
                lineObjects[counter].GetComponent<LineRenderer>().startWidth = w1;
                lineObjects[counter].GetComponent<LineRenderer>().endWidth = w1;
                counter++;
            }
        }
    }

    public void ClearConnections()
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

    public void HideConnections(bool flag)
    {
        for (int i = 0; i < lineObjects.Count; i++)
        {
            lineObjects[i].SetActive(flag);
        }
    }

    public void ShowEditCanvas()
    {
        editCanvas = Instantiate(editCanvasPrefab);
        editPheromoneinputField = editCanvas.GetComponentInChildren<InputField>();
        editPheromoneDropdown = editCanvas.GetComponentInChildren<Dropdown>();
        Button[] controlButtons = editCanvas.GetComponentsInChildren<Button>();
        controlButtons[0].onClick.AddListener(SaveChanges);
        controlButtons[1].onClick.AddListener(CloseCanvas);
        editCanvas.GetComponentInChildren<Text>().text = "Edit Pheromones";
        for (int i = 0; i< lineObjects.Count; i++) {
            string option = lineObjects[i].GetComponent<PheromoneData>().from + "-" + lineObjects[i].GetComponent<PheromoneData>().to;
            editPheromoneDropdown.options.Add(new Dropdown.OptionData(option));
        }
        editPheromoneDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(editPheromoneDropdown.value, editPheromoneDropdown.options[editPheromoneDropdown.value].text);
        });
    }

    void DropdownValueChanged(int index, string value)
    {
        string[] fromto = value.Split('-');
        currentFrom = Int32.Parse(fromto[0]);
        currentTo = Int32.Parse(fromto[1]);
        currentValue = AntAlgorithmManager.Instance.Pheromones.GetPheromone(currentFrom, currentTo);
        editPheromoneinputField.text = "" + currentValue;
    }

    void CloseCanvas()
    {
        editCanvas.SetActive(false);
        Destroy(editCanvas);
    }

    void SaveChanges()
    {
        float value = float.Parse(editPheromoneinputField.text);
        Debug.Log("Saved " + currentFrom + " - " + currentTo + " with value: " + value);
        AntAlgorithmManager.Instance.Pheromones.SetPheromone(currentFrom, currentTo, value);
        lineObjects[currentIndex].GetComponent<PheromoneData>().value = (float)value;
        CloseCanvas();
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
