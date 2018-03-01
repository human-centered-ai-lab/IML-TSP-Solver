using System.Collections;
using System.Collections.Generic;
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
