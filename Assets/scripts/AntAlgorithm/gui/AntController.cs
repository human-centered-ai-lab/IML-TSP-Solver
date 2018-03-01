using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntController : MonoBehaviour
{
    public Camera antCamera;
    public Camera mainCamera;
    private int activeAnt;

    private Boolean focus = false;
    public List<bool> animationsRunning;

    private List<GameObject> antContainerObjects;
    private List<GameObject> antObjects;

    private GameObject _antContainerPrefab;
    private GameObject _antPrefab;
    private GameObject _antPathPrefab;

    private static List<Vector3> previousAntPositions;

    void Start()
    {
        animationsRunning = new List<bool>();
        antContainerObjects = new List<GameObject>();
        antObjects = new List<GameObject>();
        previousAntPositions = new List<Vector3>();
        if (_antContainerPrefab == null)
        {
            _antContainerPrefab = Resources.Load("Prefabs/antContainer") as GameObject;
        }
        if (_antPathPrefab == null)
        {
            _antPathPrefab = Resources.Load("Prefabs/antPath") as GameObject;
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
    private void ClearConnections()
    {
        animationsRunning.Clear();
        DestroyListOfObjects(antContainerObjects);
        DestroyListOfObjects(antObjects);
        previousAntPositions.Clear();
    }
    public static void DestroyListOfObjects(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
        if (list != null)
            list.Clear();
    }

    public void MakeConnections(AntAlgorithms.AntAlgorithm antAlgorithm)
    {
        ClearConnections();
        for (int i = 0; i < AntAlgorithmManager.Instance.Ants.Count; i++)
        {
            Ant currentAnt = AntAlgorithmManager.Instance.Ants[i];
            antContainerObjects.Add(Instantiate(_antContainerPrefab));
            antObjects.Add(Instantiate(_antPrefab));
            animationsRunning.Add(false);
            GameObject firstCity = GameObject.Find("cityGameObject_" + currentAnt.Tour[0]);
            antObjects[i].transform.SetPositionAndRotation(firstCity.transform.position, firstCity.transform.rotation);

            antObjects[i].GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = "" + i;
            antObjects[i].SetActive(false);
            for (int j = 0; j < AntAlgorithmManager.Instance.Ants[i].Tour.Count - 2; j++)
            {
                GameObject currentPath = Instantiate(_antPathPrefab);
                currentPath.GetComponent<LineRenderer>().positionCount = 2;
                currentPath.name = "path" + j + "-" + (j + 1);
                currentPath.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Additive"));
                /*               float colorAmount = (i + 1f) / AntAlgorithmManager.Instance.Ants.Count;
                               Debug.Log(colorAmount);
                               antObjects[i].GetComponent<LineRenderer>().startColor = Color.Lerp(Color.magenta, Color.cyan, colorAmount);
                               antObjects[i].GetComponent<LineRenderer>().endColor = Color.Lerp(Color.magenta, Color.cyan, colorAmount);
                               */
                currentPath.GetComponent<LineRenderer>().startWidth = 10f;
                currentPath.GetComponent<LineRenderer>().endWidth = 10f;
                GameObject gameObject1 = GameObject.Find("cityGameObject_" + currentAnt.Tour[j]);
                GameObject gameObject2 = GameObject.Find("cityGameObject_" + currentAnt.Tour[j + 1]);

                currentPath.GetComponent<LineRenderer>().SetPosition(0, gameObject1.transform.position);
                currentPath.GetComponent<LineRenderer>().SetPosition(1, gameObject2.transform.position);

                currentPath.transform.SetParent(antContainerObjects[i].transform);

                AddColliderToLine(currentPath, gameObject2.transform.position, gameObject1.transform.position);

            }
            antContainerObjects[i].name = "ant" + i;
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
    public void Animate(int i, List<Button> buttons)
    {
        animationsRunning[i] = true;
        Ant currentAnt = AntAlgorithmManager.Instance.Ants[i];
        antObjects[i].SetActive(true);
        Vector3[] currentPath = new Vector3[currentAnt.Tour.Count - 1];
        for (int j = 0; j < currentAnt.Tour.Count - 1; j++)
        {
            currentPath[j] = GameObject.Find("cityGameObject_" + currentAnt.Tour[j + 1]).transform.position;
        }
        Hashtable values = new Hashtable();
        values.Add("buttons", buttons);
        values.Add("id", i);
        iTween.MoveTo(antObjects[i], iTween.Hash("name", "antAnimation" + i, "path", currentPath, "time", currentPath.Length * 2, "easetype", iTween.EaseType.linear, "orienttopath", true, "lookahead", 0.0f, "oncomplete", "OnAnimationComplete", "oncompletetarget", this.gameObject, "oncompleteparams", values));

    }

    public void FocusAnt(int i)
    {
        activeAnt = i;
        antCamera.enabled = true;
        mainCamera.enabled = false;
        if (focus == false)
            focus = true;
        else
        {
            focus = false;
            antCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }

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
    }

    public void StopAnimation(int i)
    {
        iTween.StopByName("antAnimation" + i);
        focus = false;
        animationsRunning[i] = false;
        antCamera.enabled = false;
        mainCamera.enabled = true;
    }

    private static void AddColliderToLine(GameObject line, Vector3 startPos, Vector3 endPos)
    {
        BoxCollider col = line.AddComponent<BoxCollider>();
        col.transform.SetParent(line.GetComponent<LineRenderer>().transform);
        float lineLength = Vector3.Distance(startPos, endPos);
        col.center = new Vector3(0, 0, 0.5f);
        col.size = new Vector3(lineLength, 1f, 1f);
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
