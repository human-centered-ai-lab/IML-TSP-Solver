/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* AntController is dragged in to the Scene and is used for ant ui controlling*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntController : MonoBehaviour
{
    public Camera antCamera;
    public Color antPathColor;
    public Camera mainCamera;
    public List<bool> animationsRunning;

    private int activeAnt;
    private Boolean focus = false;
    private List<GameObject> antContainerObjects;
    private List<GameObject> antObjects;
    private GameObject _antContainerPrefab;
    private GameObject _antPrefab;
    private GameObject _antPathPrefab;
    private Button focusButton;
    private Button previousFocusButton;
    private static List<Vector3> previousAntPositions;

    void Start()
    {
        animationsRunning = new List<bool>();
        antContainerObjects = new List<GameObject>();
        antObjects = new List<GameObject>();
        previousAntPositions = new List<Vector3>();

        if (_antContainerPrefab == null)
        {
            _antContainerPrefab = Resources.Load("Prefabs/AntContainer") as GameObject;
        }

        if (_antPathPrefab == null)
        {
            _antPathPrefab = Resources.Load("Prefabs/AntPath") as GameObject;
        }

        if (_antPrefab == null)
        {
            _antPrefab = Resources.Load("Prefabs/Ant") as GameObject;
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < antObjects.Count; i++)
        {
            if (antObjects[i].activeSelf == true && focus == true)
            {

                Vector3 position = antObjects[activeAnt].transform.position;
                position.z = -2.5f;
                antCamera.transform.position = position;
            }
        }
    }

    void Update()
    {

        int numAnts = antObjects.Count;

        for (int i = 0; i < numAnts; i++)
        {
            previousAntPositions.Add(new Vector3(0, 0, 0));
        }

        for (int i = 0; i < numAnts; i++)
        {
            if (antObjects[i].activeSelf == true)
            {
                if (antObjects[i].transform.position.y < previousAntPositions[i].y)
                {
                    antObjects[i].GetComponent<Animator>().SetTrigger("moveDown");
                }
                if (antObjects[i].transform.position.y == previousAntPositions[i].y)
                {
                    antObjects[i].GetComponent<Animator>().SetTrigger("idle");
                }
                if (antObjects[i].transform.position.y > previousAntPositions[i].y)
                {
                    antObjects[i].GetComponent<Animator>().SetTrigger("moveUp");
                }

                float xDif = Math.Abs(antObjects[i].transform.position.x - previousAntPositions[i].x);
                float yDif = Math.Abs(antObjects[i].transform.position.y - previousAntPositions[i].y);

                if ((xDif > yDif) && (antObjects[i].transform.position.x > previousAntPositions[i].x))
                {
                    antObjects[i].GetComponent<Animator>().SetTrigger("moveRight");
                }
                if ((xDif > yDif) && (antObjects[i].transform.position.x < previousAntPositions[i].x))
                {
                    antObjects[i].GetComponent<Animator>().SetTrigger("moveLeft");
                }

                previousAntPositions[i] = antObjects[i].transform.position;
            }
        }
    }

    // clears all connections
    private void ClearConnections()
    {
        animationsRunning.Clear();
        DestroyListOfObjects(antContainerObjects);
        DestroyListOfObjects(antObjects);
        previousAntPositions.Clear();
    }

    // destroys list objects
    public static void DestroyListOfObjects(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
        if (list != null)
            list.Clear();
    }

    // makes the ant route connections
    public void MakeConnections(AntAlgorithms.AntAlgorithm antAlgorithm)
    {
        ClearConnections();
        for (int i = 0; i < AntAlgorithmManager.Instance.Ants.Count; i++)
        {
            Ant currentAnt = AntAlgorithmManager.Instance.Ants[i];
            antContainerObjects.Add(Instantiate(_antContainerPrefab));
            antObjects.Add(Instantiate(_antPrefab));
            animationsRunning.Add(false);
            GameObject firstCity = GameObject.Find("CityGameObject_" + currentAnt.Tour[0]);
            antObjects[i].transform.SetPositionAndRotation(firstCity.transform.position, firstCity.transform.rotation);

            antObjects[i].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "" + i;
            antObjects[i].SetActive(false);
            for (int j = 0; j < AntAlgorithmManager.Instance.Ants[i].Tour.Count - 1; j++)
            {
                GameObject currentPath = Instantiate(_antPathPrefab);
                currentPath.GetComponent<LineRenderer>().positionCount = 2;
                currentPath.name = "Path" + j + "-" + (j + 1);
                currentPath.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Additive"));

                currentPath.GetComponent<LineRenderer>().startWidth = 10f;
                currentPath.GetComponent<LineRenderer>().endWidth = 10f;
                GameObject gameObject1 = GameObject.Find("CityGameObject_" + currentAnt.Tour[j]);
                GameObject gameObject2 = GameObject.Find("CityGameObject_" + currentAnt.Tour[j + 1]);

                currentPath.GetComponent<LineRenderer>().SetPosition(0, gameObject1.transform.position);
                currentPath.GetComponent<LineRenderer>().SetPosition(1, gameObject2.transform.position);

                currentPath.transform.SetParent(antContainerObjects[i].transform);

            }
            antContainerObjects[i].name = "Ant" + i;
        }
    }

    public void SetConnectionVisibility(int id, bool flag)
    {
        antContainerObjects[id].SetActive(flag);
    }

    public void SetConnectionsVisibility(bool flag)
    {
        for (int i = 0; i < antContainerObjects.Count; i++)
        {
            antContainerObjects[i].SetActive(flag);
        }
    }

    // animations for ants
    public void Animate(int i, List<Button> buttons)
    {
        animationsRunning[i] = true;
        Ant currentAnt = AntAlgorithmManager.Instance.Ants[i];
        antObjects[i].SetActive(true);
        Vector3[] currentPath = new Vector3[currentAnt.Tour.Count - 1];
        for (int j = 0; j < currentAnt.Tour.Count - 1; j++)
        {
            currentPath[j] = GameObject.Find("CityGameObject_" + currentAnt.Tour[j + 1]).transform.position;
        }
        Hashtable values = new Hashtable();
        values.Add("buttons", buttons);
        values.Add("id", i);
        if (currentAnt.Tour.Count > 2)
            iTween.MoveTo(antObjects[i], iTween.Hash("name", "antAnimation" + i,
                "path", currentPath,
                "time", currentPath.Length * 2,
                "easetype", iTween.EaseType.linear,
                "orienttopath", true,
                "lookahead", 0.0f,
                "oncomplete",
                "OnAnimationComplete",
                "oncompletetarget", this.gameObject,
                "oncompleteparams", values));
    }

    // focuses an ant
    public void FocusAnt(int i, Button button)
    {
        if (activeAnt != i && focus == true)
        {
            RecolorButton(Color.white, previousFocusButton);
            focus = false;
        }
        activeAnt = i;
        antCamera.enabled = true;
        mainCamera.enabled = false;
        if (focus == false)
        {
            RecolorButton(Color.green, button);
            focus = true;
        }
        else
        {
            RecolorButton(Color.white, button);
            focus = false;
            antCamera.enabled = false;
            mainCamera.enabled = true;
        }
        previousFocusButton = button;
    }

    // routine if animation ends
    public void OnAnimationComplete(Hashtable values)
    {
        List<Button> buttons = (List<Button>)values["buttons"];
        int id = (int)values["id"];

        animationsRunning[id] = false;

        buttons[0].GetComponentInChildren<Text>().text = "ANIM";
        buttons[1].GetComponent<Button>().interactable = false;

        focus = false;
        antCamera.enabled = false;
        mainCamera.enabled = true;
        RecolorButton(Color.white, previousFocusButton);
    }

    // button recolor 
    void RecolorButton(Color color, Button button)
    {
        if (button != null)
        {
            ColorBlock cb = button.colors;
            cb.normalColor = color;
            cb.highlightedColor = color;
            button.colors = cb;
        }
    }

    // stops all animations
    public void StopAnimation(int i)
    {
        iTween.StopByName("antAnimation" + i);
        focus = false;
        animationsRunning[i] = false;
        antCamera.enabled = false;
        mainCamera.enabled = true;
        RecolorButton(Color.white, previousFocusButton);
    }
}
