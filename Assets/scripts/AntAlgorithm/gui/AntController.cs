using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour {

    private static List<GameObject> antObjects;

    private static GameObject _antPrefab;
    private static GameObject _antPathPrefab;

    public static void Init()
    {
        antObjects = new List<GameObject>();

        if (_antPrefab == null)
        {
            _antPrefab = Resources.Load("Prefabs/ant") as GameObject;
        }
        if (_antPathPrefab == null)
        {
            _antPathPrefab = Resources.Load("Prefabs/antPath") as GameObject;
        }
    }

    public static void MakeConnections(AntAlgorithms.AntAlgorithm antAlgorithm)
    {
        for (int i = 0; i < AntAlgorithmManager.Instance.Ants.Count; i++)
        {
            Ant currentAnt = AntAlgorithmManager.Instance.Ants[i];
            antObjects.Add(Instantiate(_antPrefab));
            
            



            for (int j = 0; j < AntAlgorithmManager.Instance.Ants[i].Tour.Count-1; j ++)
            {
                GameObject currentPath = Instantiate(_antPathPrefab);
                currentPath.GetComponent<LineRenderer>().positionCount = 2;
                currentPath.name = "path" + j + "-" + (j+1);
                currentPath.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Additive"));
                /*               float colorAmount = (i + 1f) / AntAlgorithmManager.Instance.Ants.Count;
                               Debug.Log(colorAmount);
                               antObjects[i].GetComponent<LineRenderer>().startColor = Color.Lerp(Color.magenta, Color.cyan, colorAmount);
                               antObjects[i].GetComponent<LineRenderer>().endColor = Color.Lerp(Color.magenta, Color.cyan, colorAmount);
                               */
                currentPath.GetComponent<LineRenderer>().startWidth = 10f;
                currentPath.GetComponent<LineRenderer>().endWidth = 10f;
                GameObject gameObject1 = GameObject.Find("cityGameObject_" + currentAnt.Tour[j]);
                GameObject gameObject2 = GameObject.Find("cityGameObject_" + currentAnt.Tour[j+1]);


                currentPath.GetComponent<LineRenderer>().SetPosition(0, gameObject1.transform.position);
                currentPath.GetComponent<LineRenderer>().SetPosition(1, gameObject2.transform.position);

                currentPath.transform.SetParent(antObjects[i].transform);

                addColliderToLine(currentPath, gameObject2.transform.position, gameObject1.transform.position);

            }


            antObjects[i].name = "ant" + i;

        }

      
    }
    public static void ClearConnections()
    {
        for (int i = 0; i < antObjects.Count; i++)
        {
            Destroy(antObjects[i]);
        }
        if (antObjects != null)
            antObjects.Clear();
    }

    public static void SetConnectionVisibility(int id,  bool flag)
    {
        antObjects[id].SetActive(flag);   
    }
    public static void SetConnectionsVisibility(bool flag)
    {
        for (int i = 0; i < antObjects.Count; i++)
        {
            antObjects[i].SetActive(flag);
        }
    }


    private static void addColliderToLine(GameObject line, Vector3 startPos, Vector3 endPos)
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
