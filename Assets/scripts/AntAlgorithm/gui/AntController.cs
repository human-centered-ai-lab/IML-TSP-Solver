using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour {

    private static List<GameObject> antObjects;
    private static GameObject _antPrefab;

    public static void Init()
    {
        antObjects = new List<GameObject>();

        if (_antPrefab == null)
        {
            _antPrefab = Resources.Load("Prefabs/ant") as GameObject;
        }
    }

    public static void MakeConnections(AntAlgorithms.AntAlgorithm antAlgorithm)
    {
        for (int i = 0; i < AntAlgorithmManager.Instance.Ants.Count; i++)
        {
            Ant currentAnt = AntAlgorithmManager.Instance.Ants[i];
            antObjects.Add(Instantiate(_antPrefab));
            antObjects[i].GetComponent<LineRenderer>().positionCount = AntAlgorithmManager.Instance.Ants[i].Tour.Count;
            antObjects[i].GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Additive"));
            float colorAmount = (i + 1f) / AntAlgorithmManager.Instance.Ants.Count;
            Debug.Log(colorAmount);
            antObjects[i].GetComponent<LineRenderer>().startColor = Color.Lerp(Color.magenta, Color.cyan, colorAmount);
            antObjects[i].GetComponent<LineRenderer>().endColor = Color.Lerp(Color.magenta, Color.cyan, colorAmount);
            antObjects[i].GetComponent<LineRenderer>().startWidth = 10f;
            antObjects[i].GetComponent<LineRenderer>().endWidth = 10f;



            for (int j = 0; j < AntAlgorithmManager.Instance.Ants[i].Tour.Count; j ++)
            {
                GameObject gameObject1 = GameObject.Find("cityGameObject_" + currentAnt.Tour[j]);

                antObjects[i].GetComponent<LineRenderer>().SetPosition(j, gameObject1.transform.position);
                //addColliderToLine(antObjects[i], gameObject2.transform.position, gameObject1.transform.position);

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

    public static void HideConnections( int id,  bool flag)
    {
        antObjects[id].SetActive(flag);   
    }
    public static void HideAllConnections(bool flag)
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
