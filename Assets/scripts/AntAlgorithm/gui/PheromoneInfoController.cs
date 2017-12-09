using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PheromoneInfoController : MonoBehaviour {

    public bool displayInfo;
    public Text infoText;
    private LineRenderer lineRenderer;

    public Color c1 = Color.green;
    public Color c2 = Color.red;

    void Start()
    {
        if(infoText == null)
            infoText = GameObject.FindGameObjectWithTag("infoText").GetComponent<Text>();
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        displayInfo = false;
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c1;
    }

    void Update()
    { 
        ShowInfo();
    }

    void OnMouseOver()
    {
        displayInfo = true;
    }

    void OnMouseExit()

    {
        displayInfo = false;
    }

    void ShowInfo()
    {
        if (displayInfo)
        {
            infoText.text = this.GetComponent<PheromoneData>().name + "\n" + this.GetComponent<PheromoneData>().value;
            lineRenderer.startColor = c2;
            lineRenderer.endColor = c2;
        }
        else
        {
            lineRenderer.startColor = c1;
            lineRenderer.endColor = c1;
        }
    }
}
