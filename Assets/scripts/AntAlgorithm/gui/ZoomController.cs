using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour {
    private Vector3 mouseOrigin;    
    private bool isPanning;    
 
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }
        if (!Input.GetMouseButton(0)) isPanning = false;
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * 100, pos.y * 100, 0);
            transform.Translate (move, Space.Self);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && GetComponent<Camera>().orthographicSize > 100)
        {
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 100;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 )
        {
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 100;
        }
    }
}
