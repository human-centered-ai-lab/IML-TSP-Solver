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
    private int numOfFilesNotSA = 3;

    public Transform dropdownMenuTSP;
    public Transform dropdownMenuAlgorithm;
    public Button reloadButton;
    public Button stepButton;
    public Button iterationButton;
    public Button exitButton;

    public InputField iterationInputField;
    public Toggle pheromoneToggle;
    public GameObject antScrollView;
    public InputField alphaInputField;
    public InputField betaInputField;
    public InputField numAntsInputField;
    public Text infoText;






    private List<GameObject> antToggles;
    private GameObject antToggle;
    private string currentFile = null;






    public List<City> Cities { get; private set; }
    public List<Ant> Ants { get; private set; }
    public Pheromone Pheromones { get; private set; }




    // Use this for initialization
    void Start()
    {
        antToggles = new List<GameObject>();
        Cities = new List<City>();
        Ants = new List<Ant>();
        tspImporter = new TSPImporter();
        reloadButton.onClick.AddListener(LoadParam);
        stepButton.onClick.AddListener(AlgoStep);
        exitButton.onClick.AddListener(Exit);

        iterationButton.onClick.AddListener(AlgoIteration);
        pheromoneToggle.onValueChanged.AddListener((isSelected) =>
        {
            if (!isSelected)
            {
                ShowPheromones(false);
                return;
            }
            ShowPheromones(true);

        });

        iterationInputField.text = "" + 1;


        List<string> fileNames = new List<string>();


#if UNITY_STANDALONE_WIN
           DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
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
#endif

#if UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        for (int i = 0; i < numOfFilesNotSA; i++)
            dropdownMenuTSP.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("cust" + (i + 1) + ".tsp"));
        dropdownMenuTSP.GetComponent<Dropdown>().RefreshShownValue();
#endif
    }
    private IEnumerator InitWebGL(TSPImporter tsp)
    {
        while (!tsp.loadingComplete)
            yield return new WaitForSeconds(0.1f);
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Init()
    {
        CityController.DestroyAll();
        PheromoneController.ClearConnections();

        string algorithm = dropdownMenuAlgorithm.GetComponent<Dropdown>().options[dropdownMenuAlgorithm.GetComponent<Dropdown>().value].text;
        if (algorithm.Equals("MMAS"))
        {
            antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.MinMaxAntSystem, Int32.Parse(alphaInputField.text), Int32.Parse(betaInputField.text), 0.02, Int32.Parse(numAntsInputField.text), -1, 0.05);
            antAlgorithm = antAlgorithmChooser.Algorithm;
            antAlgorithm.Cities = (Cities);
            CityController.Init();
            antAlgorithm.Init();
            Ants = antAlgorithm.Ants();
            Pheromones = antAlgorithm.Pheromones;
            antAlgorithm.PrintBestTour("MMAS-" + currentFile, 1);

        }
        if (algorithm.Equals("ACS"))
        {
            antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.AntColonySystem, Int32.Parse(alphaInputField.text), Int32.Parse(betaInputField.text), 0.02, Int32.Parse(numAntsInputField.text), -1, 0.05);
            antAlgorithm = antAlgorithmChooser.Algorithm;
            antAlgorithm.Cities = (Cities);
            CityController.Init();
            antAlgorithm.Init();
            Ants = antAlgorithm.Ants();
            Pheromones = antAlgorithm.Pheromones;
            antAlgorithm.PrintBestTour("ACS-" + currentFile, 1);

        }
        if (algorithm.Equals("AS"))
        {
            antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.AntSystem, Int32.Parse(alphaInputField.text), Int32.Parse(betaInputField.text), 0.02, Int32.Parse(numAntsInputField.text), -1, 0.05);
            antAlgorithm = antAlgorithmChooser.Algorithm;
            antAlgorithm.Cities = (Cities);
            CityController.Init();
            antAlgorithm.Init();
            Ants = antAlgorithm.Ants();
            Pheromones = antAlgorithm.Pheromones;
            antAlgorithm.PrintBestTour("AS-" + currentFile, 1);

        }

        PheromoneController.Init();
        PheromoneController.MakeConnections(antAlgorithm);
        AntController.Init();
        AntController.MakeConnections(antAlgorithm);
        LoadAntToggles(Int32.Parse(numAntsInputField.text));

        ShowAnts(false);

    }
    void LoadParam()
    {
        currentFile = dropdownMenuTSP.GetComponent<Dropdown>().options[dropdownMenuTSP.GetComponent<Dropdown>().value].text;

#if UNITY_STANDALONE_WIN
        Debug.Log("Stand Alone Windows");
        Cities = TSPImporter.ImportTsp(currentFile);
        Init();
#endif

#if UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        TSPImporter tsp = new TSPImporter();
        Debug.Log("WebGL or Mobile");
        StartCoroutine(tsp.ImportTspFromWebWebGL(currentFile));
        StartCoroutine(InitWebGL(tsp));
        Cities = tsp.Cities;
#endif


        ShowAnts(false);
    }

    void SetPheromones(int a, int b, float value)
    {
        antAlgorithm.Pheromones.SetPheromone(a, b, value);
    }

    void ShowSolution()
    {
        string solution = "Best iteration: " + antAlgorithm.BestIteration + "\nBest Tour: " + antAlgorithm.GetBestTour(1);
        infoText.text = solution;
    }

    void LoadAntToggles(int numOfAnts)
    {
        DestroyCurrentAnts();
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
            ShowAnt(id, false);
            return;
        }
        ShowAnt(id, true);
    }

    void DestroyCurrentAnts()

    {
        for (int i = 0; i < antToggles.Count; i++)
        {
            Destroy(antToggles[i]);
        }
        antToggles.Clear();
    }

    void AlgoStep()
    {
        PheromoneController.ClearConnections();
        AntController.ClearConnections();
        antAlgorithm.Step();
        PheromoneController.MakeConnections(antAlgorithm);
        if (!pheromoneToggle.isOn)
        {
            ShowPheromones(false);
        }
        AntController.MakeConnections(antAlgorithm);
        for (int i = 0; i < antToggles.Count; i++)
            if (!antToggles[i].GetComponent<Toggle>().isOn)
                ShowAnt(i, false);


        ShowSolution();
    }

    void ShowPheromones(bool flag)
    {
        PheromoneController.HideConnections(flag);
    }
    void ShowAnt(int id, bool flag)
    {
        AntController.SetConnectionVisibility(id, flag);
    }
    void ShowAnts(bool flag)
    {
        AntController.SetConnectionsVisibility(flag);
    }
    void AlgoIteration()
    {
        int iterations = Int32.Parse(iterationInputField.text);
        PheromoneController.ClearConnections();
        for (int i = 0; i < iterations; i++)
        {
            antAlgorithm.Iteration();
        }
        PheromoneController.MakeConnections(antAlgorithm);
        ShowSolution();

    }

    void Exit()
    {
        Application.Quit();
    }
}
