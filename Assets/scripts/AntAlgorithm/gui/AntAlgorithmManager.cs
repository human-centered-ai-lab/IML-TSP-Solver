/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithmManager is dragged in to the Scene*/

using AntAlgorithm.tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;

public class AntAlgorithmManager : Singleton<AntAlgorithmManager>
{
    private static int defaultStepValue = 1;
    private static int defaultIterationValue = 1;
    private static int defaultAlphaValue = 1;
    private static int defaultBetaValue = 2;
    private static int defaultNumAntsValue = 51;

    public Canvas rightCanvas;
    public Canvas leftCanvas;
    public GameObject startCanvasPrefab;
    public Canvas aboutCanvas;
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

    private List<Button> animationButtons;
    private List<Button> focusButtons;
    private List<GameObject> antToggles;
    private GameObject antToggle;
    private string currentFile = null;
    private bool animateAllActive = false;
    private AntAlgorithms.AntAlgorithmChooser antAlgorithmChooser;
    private AntAlgorithms.AntAlgorithm antAlgorithm;
    private TSPImporter tspImporter;
    private int numOfFilesNotSA = 3;
    private int numOfAnts;

    public List<City> Cities { get; private set; }
    public List<Ant> Ants { get; private set; }
    public Pheromone Pheromones { get; private set; }

    // Clears all the lists
    private void ClearLists()
    {
        DestroyDynamicUIElemets();
        animationButtons.Clear();
        focusButtons.Clear();
        antToggles.Clear();
        Cities.Clear();
        Ants.Clear();
    }

    // Destroys GameObjects in the Scene
    private void DestroyDynamicUIElemets()
    {
        for (int i = 0; i < numOfAnts; i++)
        {
            Destroy(antToggles[i].gameObject);
            Destroy(animationButtons[i].gameObject);
            Destroy(focusButtons[i].gameObject);

        }
    }

    void Start()
    {
        startCanvasPrefab = Instantiate(Resources.Load("Prefabs/StartCanvas") as GameObject);
        animationButtons = new List<Button>();
        focusButtons = new List<Button>();
        antToggles = new List<GameObject>();
        Cities = new List<City>();
        Ants = new List<Ant>();
        tspImporter = new TSPImporter();

        //button onclick listener
        editPheromonesButton.onClick.AddListener(EditPheromones);
        reloadButton.onClick.AddListener(Reload);
        stepButton.onClick.AddListener(AlgoStep);
        animateAllButton.onClick.AddListener(RunAllAntsAnimation);
        iterationButton.onClick.AddListener(AlgoIteration);
        aboutButton.onClick.AddListener(About);
        aboutCanvas.GetComponentInChildren<Button>().onClick.AddListener(About);
        rightCanvas.GetComponent<CanvasHider>().Hide();
        startCanvasPrefab.GetComponent<Canvas>().enabled = true;
        startCanvasPrefab.GetComponent<Canvas>().GetComponentInChildren<Button>().onClick.AddListener(EnterMainMode);

        pheromoneToggle.onValueChanged.AddListener((isSelected) =>
        {
            if (!isSelected)
            {
                ShowPheromones(false);
                return;
            }
            ShowPheromones(true);
        });

        // defaults for input fields
        iterationInputField.text = "" + defaultIterationValue;
        stepInputField.text = "" + defaultStepValue;
        alphaInputField.text = "" + defaultAlphaValue;
        betaInputField.text = "" + defaultBetaValue;
        numAntsInputField.text = "" + defaultNumAntsValue;


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

    // for WebGl init
    private IEnumerator InitWebGL(TSPImporter tsp)
    {
        while (!tsp.loadingComplete)
            yield return new WaitForSeconds(0.1f);
        Init();
    }

    // shows editcanvas for pheromones
    void EditPheromones()
    {
        pheromoneController.ShowEditCanvas();
    }

    // basic initialization of the algorithm
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
        LoadAntToggles(numOfAnts);
        VisibilityCheck();
    }

    // ReloadButton routine
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

    // sets pheromones
    void SetPheromones(int a, int b, float value)
    {
        antAlgorithm.Pheromones.SetPheromone(a, b, value);
    }

    // shows the solution of the algorith in the text field
    void ShowSolution()
    {
        string solution = "Best iteration: " + antAlgorithm.BestIteration + "\nBest tour: " + antAlgorithm.GetBestTour(1) + "\nBest tour length: " + antAlgorithm.TourLength.ToString("F2");
        infoText.text = solution;
        iterationText.text = "Current iteration: " + antAlgorithm.CurrentIteration + "\nCurrent step: " + antAlgorithm.AlgStep;
    }

    // load ant toggles
    void LoadAntToggles(int numOfAnts)
    {
        antToggle = Resources.Load("Prefabs/AntToggle") as GameObject;
        for (int i = 0; i < numOfAnts; i++)
        {
            antToggles.Add(Instantiate(antToggle));
            antToggles[i].transform.SetParent(antScrollView.transform, false);
            antToggles[i].name = "AntToggle" + i;
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

    // show animations routine
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

    // stops animation
    void StopAntAnimation(int id)
    {
        focusButtons[id].interactable = false;
        antController.StopAnimation(id);
        animationButtons[id].GetComponentInChildren<Text>().text = "ANIM";
    }

    // starts animations
    void StartAntAnimation(int id)
    {
        focusButtons[id].interactable = true;
        animationButtons[id].GetComponentInChildren<Text>().text = "STOP";

        List<Button> buttons = new List<Button>
            {
                animationButtons[id],
                focusButtons[id]
            };

        antController.Animate(id, buttons);
    }

    // runs all animations
    void RunAllAntsAnimation()
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

    // stops all animations
    void StopAllAnimations()
    {
        animateAllButton.GetComponentInChildren<Text>().text = "animate all ants";
        for (int i = 0; i < numOfAnts; i++)
            StopAntAnimation(i);
        animateAllActive = false;
    }

    // focus an ant
    void FocusAnt(int id)
    {
        antController.FocusAnt(id, focusButtons[id]);
    }
    //listeners
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

    //handles listeners
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

    // algo step routine
    void AlgoStep()
    {
        StopAllAnimations();
        int steps = Int32.Parse(stepInputField.text);
        pheromoneController.ClearConnections();
        for (int i = 0; i < steps; i++)
        {
            antAlgorithm.Step();
        }
        pheromoneController.MakeConnections(antAlgorithm);
        antController.MakeConnections(antAlgorithm);

        ShowSolution();
        VisibilityCheck();
    }

    // algo iteration routine
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

    // visibility check
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

    // about canvas routine
    void About()
    {
        if (!aboutCanvas.enabled)
            aboutCanvas.enabled = true;
        else
            aboutCanvas.enabled = false;

    }

    // enters main mode
    void EnterMainMode()
    {
        startCanvasPrefab.GetComponent<Canvas>().enabled = false;
    }
}
