/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* CityInfoController is used for mousover city options*/

using UnityEngine;
using UnityEngine.UI;

public class CityInfoController : MonoBehaviour {

    public Text myText;
    public bool displayInfo;

    void Start()
    {
        myText = GetComponentInChildren<Text>();
        myText.color = Color.clear;
    }

    void Update()
    {
        FadeText();
    }

    void OnMouseOver()
    {
        displayInfo = true;
    }

    void OnMouseExit()
    {
        displayInfo = false;

    }

    void FadeText()
    {
        if (displayInfo)
        {
            myText.color = Color.blue;
        }
        else
        {
            myText.color = Color.cyan;
        }
    }
}
