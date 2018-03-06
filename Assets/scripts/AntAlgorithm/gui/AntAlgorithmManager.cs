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
    public Canvas rightCanvas;
    public Canvas leftCanvas;
    public GameObject startCanvasPrefab;
    public Canvas aboutCanvas;

    private AntAlgorithms.AntAlgorithmChooser antAlgorithmChooser;
    private AntAlgorithms.AntAlgorithm antAlgorithm;
    private TSPImporter tspImporter;
    private int numOfFilesNotSA = 3;
    private int numOfAnts;

    public AntController antController;
    public PheromoneController pheromoneController;

    public Transform dropdownMenuTSP;
    public Transform dropdownMenuAlgorithm;
    public Button reloadButton;
    public Button stepButton;
    public Button iterationButton;
    public Button animateAllButton;
    public Button editPheromonesButton;

    public Button aboutButton;


    public InputField iterationInputField;
    public InputField stepInputField;

    public Toggle pheromoneToggle;
    public GameObject antScrollView;
    public InputField alphaInputField;
    public InputField betaInputField;
    public InputField numAntsInputField;
    public Text infoText;
    public Text iterationText;
    public Text filenameText;

    List<Button> animationButtons;
    List<Button> focusButtons;

    private List<GameObject> antToggles;
    private GameObject antToggle;
    private string currentFile = null;
    private bool animateAllActive = false;

    public List<City> Cities { get; private set; }
    public List<Ant> Ants { get; private set; }
    public Pheromone Pheromones { get; private set; }

    private void ClearLists()
    {
        DestroyDynamicUIElemets();
        animationButtons.Clear();
        focusButtons.Clear();
        antToggles.Clear();
        Cities.Clear();
        Ants.Clear();
    }
    private void DestroyDynamicUIElemets()
    {
        for (int i = 0; i < numOfAnts; i++)
        {
            Destroy(antToggles[i].gameObject);
            Destroy(animationButtons[i].gameObject);
            Destroy(focusButtons[i].gameObject);

        }

    }
    // Use this for initialization
    void Start()
    {
        startCanvasPrefab = Instantiate(Resources.Load("Prefabs/StartCanvas") as GameObject);

        animationButtons = new List<Button>();
        focusButtons = new List<Button>();
        antToggles = new List<GameObject>();
        Cities = new List<City>();
        Ants = new List<Ant>();
        tspImporter = new TSPImporter();

        editPheromonesButton.onClick.AddListener(EditPheromones);

        reloadButton.onClick.AddListener(Reload);
        stepButton.onClick.AddListener(AlgoStep);
        animateAllButton.onClick.AddListener(ShowAllAntsAnimation);
        iterationButton.onClick.AddListener(AlgoIteration);
        rightCanvas.GetComponent<CanvasHider>().Hide();
        startCanvasPrefab.GetComponent<Canvas>().enabled = true;
        startCanvasPrefab.GetComponent<Canvas>().GetComponentInChildren<Button>().onClick.AddListener(EnterMainMode);
        aboutButton.onClick.AddListener(About);
        aboutCanvas.GetComponentInChildren<Button>().onClick.AddListener(About);

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
        stepInputField.text = "" + 1;
        alphaInputField.text = "" + 1;
        betaInputField.text = "" + 2;
        numAntsInputField.text = "" + 51;


#if UNITY_STANDALONE_WIN
        List<string> fileNames = new List<string>();

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
            dropdownMenuTSP.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("data" + (i + 1) + ".tsp"));
        dropdownMenuTSP.GetComponent<Dropdown>().RefreshShownValue();
#endif
    }

    private IEnumerator InitWebGL(TSPImporter tsp)
    {
        while (!tsp.loadingComplete)
            yield return new WaitForSeconds(0.1f);
        Init();
    }

    void EditPheromones()
    {
        pheromoneController.ShowEditCanvas();
    }

    void Init()
    {
        numOfAnts = Int32.Parse(numAntsInputField.text);

        CityController.DestroyAll();
        pheromoneController.ClearConnections();

        string algorithm = dropdownMenuAlgorithm.GetComponent<Dropdown>().options[dropdownMenuAlgorithm.GetComponent<Dropdown>().value].text;
        if (algorithm.Equals("MMAS"))
        {
            antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.MinMaxAntSystem, Int32.Parse(alphaInputField.text), Int32.Parse(betaInputField.text), 0.02, Int32.Parse(numAntsInputField.text), -1, 0.05);
            antAlgorithm = antAlgorithmChooser.Algorithm;
            antAlgorithm.Cities = (Cities);
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
            antAlgorithm.Init();
            Ants = antAlgorithm.Ants();
            Pheromones = antAlgorithm.Pheromones;
            antAlgorithm.PrintBestTour("AS-" + currentFile, 1);
        }

        CityController.Init();
        pheromoneController.Init();
        pheromoneController.MakeConnections(antAlgorithm);
        antController.MakeConnections(antAlgorithm);
        //ShowAnts(false);
        LoadAntToggles(numOfAnts);
        VisibilityCheck();

    }
    void Reload()
    {
        StopAllAnimations();

        leftCanvas.GetComponent<CanvasHider>().Hide();
        rightCanvas.GetComponent<CanvasHider>().Show();
        reloadButton.GetComponentInChildren<Text>().text = "RELOAD";

        StopAllAnimations();
        ClearLists();
        currentFile = dropdownMenuTSP.GetComponent<Dropdown>().options[dropdownMenuTSP.GetComponent<Dropdown>().value].text;
        filenameText.text = currentFile;
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

    }

    void SetPheromones(int a, int b, float value)
    {
        antAlgorithm.Pheromones.SetPheromone(a, b, value);
    }

    void ShowSolution()
    {
        string solution = "Best iteration: " + antAlgorithm.BestIteration + "\nBest Tour: " + antAlgorithm.GetBestTour(1);
        infoText.text = solution;
        iterationText.text = "Current iteration: " + antAlgorithm.CurrentIteration + "\nCurrent step: " + antAlgorithm.AlgStep;
    }

    void LoadAntToggles(int numOfAnts)
    {
        antToggle = Resources.Load("Prefabs/antToggle") as GameObject;
        for (int i = 0; i < numOfAnts; i++)
        {
            antToggles.Add(Instantiate(antToggle));
            antToggles[i].transform.SetParent(antScrollView.transform, false);
            antToggles[i].name = "antToggle" + i;
            antToggles[i].GetComponentInChildren<Text>().text = "ant route " + i;
            UnityEngine.Events.UnityAction animationButtonlistener = AnimationButtonListener(i);
            UnityEngine.Events.UnityAction focusButtonlistener = FocusButtonListener(i);

            Button[] buttons = new Button[2];
            buttons = antToggles[i].GetComponentsInChildren<Button>();

            animationButtons.Add(buttons[0]);
            focusButtons.Add(buttons[1]);

            animationButtons[i].onClick.AddListener(animationButtonlistener);
            focusButtons[i].onClick.AddListener(focusButtonlistener);

            antToggles[i].GetComponent<Toggle>().isOn = false;
            UnityEngine.Events.UnityAction<bool> toggleListener = AntToggleListener(i);
            antToggles[i].GetComponent<Toggle>().onValueChanged.AddListener(toggleListener);
        }
    }

    void ShowAntAnimation(int id)
    {
        if (antController.animationsRunning[id])
        {
            StopAntAnimation(id);
        }
        else
        {
            StartAntAnimation(id);
        }
    }

    void StopAntAnimation(int id)
    {
        focusButtons[id].interactable = false;
        //iterationButton.interactable = true;
        //stepButton.interactable = true;
        //reloadButton.interactable = true;
        antController.StopAnimation(id);
        animationButtons[id].GetComponentInChildren<Text>().text = "ANIM";

    }

    void StartAntAnimation(int id)
    {
        focusButtons[id].interactable = true;
        //iterationButton.interactable = false;
        //stepButton.interactable = false;
        //reloadButton.interactable = false;



        animationButtons[id].GetComponentInChildren<Text>().text = "STOP";

        List<Button> buttons = new List<Button>
            {
                animationButtons[id],
                focusButtons[id]
            };

        antController.Animate(id, buttons);
    }

    void ShowAllAntsAnimation()
    {
        if (!animateAllActive)
        {
            animateAllButton.GetComponentInChildren<Text>().text = "STOP";
            for (int i = 0; i < numOfAnts; i++)
                StartAntAnimation(i);
            animateAllActive = true;
        }
        else
        {
            animateAllButton.GetComponentInChildren<Text>().text = "animate all ants";
            for (int i = 0; i < numOfAnts; i++)
                StopAntAnimation(i);
            animateAllActive = false;
        }
    }
    void StopAllAnimations()
    {
        animateAllButton.GetComponentInChildren<Text>().text = "animate all ants";
        for (int i = 0; i < numOfAnts; i++)
            StopAntAnimation(i);
        animateAllActive = false;
    }

    void FocusAnt(int id)
    {
        antController.FocusAnt(id, focusButtons[id]);
    }
    private UnityEngine.Events.UnityAction AnimationButtonListener(int id)
    {
        return () => ShowAntAnimation(id);
    }
    private UnityEngine.Events.UnityAction FocusButtonListener(int id)
    {
        return () => FocusAnt(id);
    }
    private UnityEngine.Events.UnityAction<bool> AntToggleListener(int id)
    {
        return (val) => HandleAntToggleListener(val, id);
    }

    void HandleAntToggleListener(bool isSelected, int id)
    {
        if (!isSelected)
        {
            ShowAnt(id, false);
            return;
        }
        ShowAnt(id, true);
    }

    void HandleAnimationButtonListener(int id)
    {
        ShowAntAnimation(id);
    }

    void AlgoStep()
    {
        StopAllAnimations();
        int steps = Int32.Parse(stepInputField.text);
        pheromoneController.ClearConnections();
        Debug.Log("" + steps);
        for (int i = 0; i < steps; i++)
        {
            antAlgorithm.Step();
        }
        pheromoneController.MakeConnections(antAlgorithm);
        antController.MakeConnections(antAlgorithm);

        ShowSolution();
        VisibilityCheck();
    }
    void AlgoIteration()
    {
        StopAllAnimations();

        pheromoneController.ClearConnections();

        int iterations = Int32.Parse(iterationInputField.text);
        for (int i = 0; i < iterations; i++)
        {
            antAlgorithm.Iteration();
        }
        ShowSolution();
        pheromoneController.MakeConnections(antAlgorithm);
        antController.MakeConnections(antAlgorithm);
        VisibilityCheck();
    }

    void VisibilityCheck()
    {

        if (!pheromoneToggle.isOn)
        {
            ShowPheromones(false);
        }
        else
        {
            ShowPheromones(true);
        }
        for (int i = 0; i < antToggles.Count; i++)
        {
            if (!antToggles[i].GetComponent<Toggle>().isOn)
                ShowAnt(i, false);
            else
                ShowAnt(i, true);
        }

    }

    void ShowPheromones(bool flag)
    {
        pheromoneController.HideConnections(flag);
    }
    void ShowAnt(int id, bool flag)
    {
        antController.SetConnectionVisibility(id, flag);
    }
    void ShowAnts(bool flag)
    {
        antController.SetConnectionsVisibility(flag);
    }


    void About()
    {
        if (!aboutCanvas.enabled)
            aboutCanvas.enabled = true;
        else
            aboutCanvas.enabled = false;

    }
    void EnterMainMode()
    {
        startCanvasPrefab.GetComponent<Canvas>().enabled = false;
    }
}
