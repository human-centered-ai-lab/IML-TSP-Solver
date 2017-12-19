using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomController : MonoBehaviour
{
    private Vector3 mouseOrigin;
    private bool isPanning;
    public Texture2D cursorTexture;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        if (!Input.GetMouseButton(1))
        {
            isPanning = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * 100, pos.y * 100, 0);
            transform.Translate(-move, Space.Self);
        }
        if (!EventSystem.current.IsPointerOverGameObject())
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
