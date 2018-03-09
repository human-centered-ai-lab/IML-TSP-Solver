/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* CameraController for Camera swithing*/

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject ant;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - ant.transform.position;
    }

    void LateUpdate()
    {
        transform.position = ant.transform.position + offset;
    }
}
