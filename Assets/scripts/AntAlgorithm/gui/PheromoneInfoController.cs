/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* PheromoneInfoController is used for mouseover pheromone handling*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PheromoneInfoController : MonoBehaviour
{

    private Texture2D editCrusorTexture;

    public bool displayInfo;
    public Color c1;
    public Color c2 = Color.red;
    public static bool isInstantiated = false;
    public static Text infoText;
    public static GameObject _editCanvasPrefab;
    public GameObject editCanvas;

    private LineRenderer lineRenderer;
    private InputField pheromoneInput;

    void Start()
    {
        if (editCrusorTexture == null)
            editCrusorTexture = Resources.Load("Prefabs/MouseEdit") as Texture2D;
        c1 = new Color32(59, 189, 35, 255);
        if (infoText == null)
            infoText = GameObject.FindGameObjectWithTag("infoText").GetComponent<Text>();
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        displayInfo = false;
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c1;
        if (_editCanvasPrefab == null)
        {
            _editCanvasPrefab = Resources.Load("Prefabs/IMLEditCanvas") as GameObject;
        }
    }

    void Update()
    {
        ShowInfo();
    }

    void OnMouseOver()
    {
        if (!IsPointerOverGameObject())
        {
            Cursor.SetCursor(editCrusorTexture, Vector2.zero, CursorMode.ForceSoftware);
            displayInfo = true;
        }
    }

    private void OnMouseUp()
    {
        if (!IsPointerOverGameObject())
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
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        infoText.text = "";
    }

    void ShowInfo()
    {
        if (displayInfo)
        {
            infoText.text = this.GetComponent<PheromoneData>().name + " " + this.GetComponent<PheromoneData>().value;
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
        editCanvas.GetComponentInChildren<Dropdown>().interactable = false;
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
        float value = float.Parse(pheromoneInput.text);
        AntAlgorithmManager.Instance.Pheromones.SetPheromone(this.GetComponent<PheromoneData>().from, this.GetComponent<PheromoneData>().to, value);
        this.GetComponent<PheromoneData>().value = value;
        CloseCanvas();
    }
    public static bool IsPointerOverGameObject()
    {

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

    }
}
