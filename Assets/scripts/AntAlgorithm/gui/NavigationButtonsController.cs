/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* NavigationButtonsController is used for hiding ui canvas elements*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationButtonsController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Camera mainCamera;
    public bool isDown;
    public bool isUp;
    public bool isLeft;
    public bool isRight;
    public bool isIn;
    public bool isOut;
    public int zoomSpeed = 30;
    public int transfSpeed = 10;

    private Vector3 position;

    void Update()
    {
        position = mainCamera.GetComponent<RectTransform>().position;
        if (isRight)
            mainCamera.GetComponent<RectTransform>().position = new Vector3(position.x + transfSpeed, position.y, position.z);
        if (isDown)
            mainCamera.GetComponent<RectTransform>().position = new Vector3(position.x, position.y- transfSpeed, position.z);
        if (isUp)
            mainCamera.GetComponent<RectTransform>().position = new Vector3(position.x, position.y + transfSpeed, position.z);
        if (isLeft)
            mainCamera.GetComponent<RectTransform>().position = new Vector3(position.x - transfSpeed, position.y, position.z);
        if (isOut)
            mainCamera.orthographicSize = mainCamera.orthographicSize + zoomSpeed;
        if (isIn)
            mainCamera.orthographicSize = mainCamera.orthographicSize - zoomSpeed;
    }
    void Down()
    {
        isDown = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.name.Equals("LeftButton"))
            isLeft = true;
        if (this.name.Equals("RightButton"))
            isRight = true;
        if (this.name.Equals("UpButton"))
            isUp = true;
        if (this.name.Equals("DownButton"))
            isDown = true;
        if (this.name.Equals("InButton"))
            isIn = true;
        if (this.name.Equals("OutButton"))
            isOut = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.name.Equals("LeftButton"))
            isLeft = false;
        if (this.name.Equals("RightButton"))
            isRight = false;
        if (this.name.Equals("UpButton"))
            isUp = false;
        if (this.name.Equals("DownButton"))
            isDown = false;
        if (this.name.Equals("InButton"))
            isIn = false;
        if (this.name.Equals("OutButton"))
            isOut = false;
    }
}
