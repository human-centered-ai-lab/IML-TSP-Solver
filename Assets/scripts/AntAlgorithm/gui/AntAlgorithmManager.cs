using AntAlgorithm.tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using util;

public class AntAlgorithmManager : Singleton<AntAlgorithmManager>
{

    private AntAlgorithms.AntAlgorithmChooser antAlgorithmChooser;
    private AntAlgorithms.AntAlgorithm antAlgorithm;
    private TSPImporter tspImporter;
    public Transform dropdownMenuTSP;
    public Transform dropdownMenuAlgorithm;
    public Button reloadButton;
    public Button stepButton;
    public Button iterationButton;
    public InputField iterationInputField;
    public Toggle pheromoneToggle;
    public GameObject antScrollView;
    public InputField alphaInputField;
    public InputField betaInputField;
    public InputField numAntsInputField;





    private List<GameObject> antToggles;
    private GameObject antToggle;







    public List<City> Cities { get; private set; }
    public List<Ant> Ants { get; private set; }



    // Use this for initialization
    void Start()
    {
        antToggles = new List<GameObject>();
        Cities = new List<City>();
        Ants = new List<Ant>();
        tspImporter = new TSPImporter();
        reloadButton.onClick.AddListener(LoadParam);
        stepButton.onClick.AddListener(algoStep);
        iterationButton.onClick.AddListener(algoIteration);
        pheromoneToggle.onValueChanged.AddListener((isSelected) =>
        {
            if (!isSelected)
            {
                showPheromones(false);
                return;
            }
            showPheromones(true);

        });

        iterationInputField.text = "" + 1;

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
        List<string> fileNames = new List<string>();

        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == ".tsp")
            {
                fileNames.Add(file.Name);
            }
        }

        for (int i = 0; i < fileNames.Count; i++)
            dropdownMenuTSP.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(fileNames[i]));
        dropdownMenuTSP.GetComponent<Dropdown>().RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadParam()
    {
        CityController.DestroyAll();
        PheromoneController.ClearConnections();

        string algorithm = dropdownMenuAlgorithm.GetComponent<Dropdown>().options[dropdownMenuAlgorithm.GetComponent<Dropdown>().value].text;
        string file = dropdownMenuTSP.GetComponent<Dropdown>().options[dropdownMenuTSP.GetComponent<Dropdown>().value].text;
        if (algorithm.Equals("MMAS"))
        {
            antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.MinMaxAntSystem, Int32.Parse(alphaInputField.text), Int32.Parse(betaInputField.text), 0.02, Int32.Parse(numAntsInputField.text), -1, 0.05);
            antAlgorithm = antAlgorithmChooser.Algorithm;
            Cities = TSPImporter.ImportTsp(file);
            antAlgorithm.Cities = (Cities);
            CityController.Init();
            antAlgorithm.Init();
            Ants = antAlgorithm.Ants();
            antAlgorithm.PrintBestTour("MMAS-" + file, 1);

        }
        PheromoneController.Init();
        PheromoneController.MakeConnections(antAlgorithm);
        AntController.Init();
        AntController.MakeConnections(antAlgorithm);
        LoadAntToggles(Int32.Parse(numAntsInputField.text));

        showAnts(false);


    }
    void LoadAntToggles(int numOfAnts)
    {
        destroyCurrentAnts();
        antToggle = Resources.Load("Prefabs/antToggle") as GameObject;
        for (int i = 0; i < numOfAnts; i++)
        {
            antToggles.Add(Instantiate(antToggle));
            antToggles[i].transform.SetParent(antScrollView.transform, false);
            antToggles[i].name = "antToggle" + i;
            antToggles[i].GetComponentInChildren<Text>().text = "ant " + i;
            antToggles[i].GetComponent<Toggle>().isOn = false;
            UnityEngine.Events.UnityAction<bool> listener = AntListener(i);
            antToggles[i].GetComponent<Toggle>().onValueChanged.AddListener(listener);
        }

    }
    private UnityEngine.Events.UnityAction<bool> AntListener(int id)
    {
        return (val) => HandleAntListener(val, id);
    }

    void HandleAntListener(bool isSelected, int id)
    {
        if (!isSelected)
        {
            showAnt(id, false);
            return;
        }
        showAnt(id, true);
    }
    void destroyCurrentAnts()
    {
        for (int i = 0; i < antToggles.Count; i++)
        {
            Destroy(antToggles[i]);
        }
        antToggles.Clear();
    }

    void algoStep()
    {
        PheromoneController.ClearConnections();
        AntController.ClearConnections();
        antAlgorithm.Step();
        PheromoneController.MakeConnections(antAlgorithm);
        if (!pheromoneToggle.isOn)
        {
            showPheromones(false);
        }
        AntController.MakeConnections(antAlgorithm);
        for (int i = 0; i < antToggles.Count; i++)
            if (!antToggles[i].GetComponent<Toggle>().isOn)
                showAnt(i,false);
                

    }

    void showPheromones(bool flag)
    {
        PheromoneController.HideConnections(flag);

    }
    void showAnt(int id, bool flag)
    {
        AntController.HideConnections(id, flag);
    }
    void showAnts(bool flag)
    {
        AntController.HideAllConnections(flag);
    }
    void algoIteration()
    {
        int iterations = Int32.Parse(iterationInputField.text);
        PheromoneController.ClearConnections();
        for (int i = 0; i < iterations; i++)
        {
            antAlgorithm.Iteration();
        }
        PheromoneController.MakeConnections(antAlgorithm);

    }
}
