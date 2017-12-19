using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PheromoneInfoController : MonoBehaviour {

    public bool displayInfo;
    public Text infoText;
    public GameObject _editCanvasPrefab;
    public GameObject editCanvas;

    private LineRenderer lineRenderer;
    private InputField pheromoneInput;
    public Color c1 = Color.green;
    public Color c2 = Color.red;
    public static bool isInstantiated = false;

    void Start()
    {
        if(infoText == null)
            infoText = GameObject.FindGameObjectWithTag("infoText").GetComponent<Text>();
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        displayInfo = false;
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c1;
        if (_editCanvasPrefab == null)
        {
            _editCanvasPrefab = Resources.Load("Prefabs/EditCanvas") as GameObject;
        }
    }

    void Update()
    { 
        ShowInfo();
    }

    void OnMouseOver()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
            displayInfo = true;
    }


    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            if (!isInstantiated)
            {
                editCanvas = Instantiate(_editCanvasPrefab);
                pheromoneInput = editCanvas.GetComponentInChildren<InputField>();
                editCanvas.SetActive(true);
                isInstantiated = true;
            }
            if (editCanvas == null)
                return;
            ShowEditCanvas();
        }
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
    void ShowEditCanvas()
    {
        Debug.Log("ShowEditCanvas()");

        Button[] controlButtons = editCanvas.GetComponentsInChildren<Button>();
        controlButtons[0].onClick.AddListener(SaveChanges);
        controlButtons[1].onClick.AddListener(CloseCanvas);
        Text desciptionText = editCanvas.GetComponentInChildren<Text>();
        desciptionText.text = this.GetComponent<PheromoneData>().name;
        pheromoneInput.text = this.GetComponent<PheromoneData>().value + "";
    }
    void CloseCanvas()
    {
        editCanvas.SetActive(false);
        isInstantiated = false;
        Destroy(editCanvas);
    }
    void SaveChanges()
    {
        Debug.Log("SaveChanges()");
        float value = float.Parse(pheromoneInput.text);
        AntAlgorithmManager.Instance.Pheromones.SetPheromone(this.GetComponent<PheromoneData>().from, this.GetComponent<PheromoneData>().to, value);
        this.GetComponent<PheromoneData>().value = value;
        CloseCanvas();
    }
}
