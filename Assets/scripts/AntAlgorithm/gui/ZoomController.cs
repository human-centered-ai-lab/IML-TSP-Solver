/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* ZoomController is used for zooming */

using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomController : MonoBehaviour
{
    public Texture2D cursorTexture;

    private Vector3 mouseOrigin;
    private bool isPanning;
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !PheromoneInfoController.IsPointerOverGameObject())
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        if (isPanning)
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * 100, pos.y * 100, 0);
            transform.Translate(-move, Space.Self);
        }
        if (!PheromoneInfoController.IsPointerOverGameObject())
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && GetComponent<Camera>().orthographicSize > 100)
            {
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 100;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 100;
            }
        }
    }
}
