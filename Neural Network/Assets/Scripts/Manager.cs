using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class Manager : MonoBehaviour
{
    //Training
    public int fastTraining;
    public int trainingBeforeRun;

    //Canvas Stuff
    [SerializeField] private TextMeshProUGUI yellowScore;
    [SerializeField] private TextMeshProUGUI blueScore;
    [SerializeField] private TextMeshProUGUI redScore;
    [SerializeField] private TextMeshProUGUI greenScore;

    public int numberOfTraining = 0;

    public CheckpointManager currentCheckPointManager;

    public GameObject LastCheckpointTrack;

    public GameObject currentTrack;

    public List<GameObject> stadiumList = new List<GameObject>();
    public List<GameObject> chosenList = new List<GameObject>();

    public bool needFinishCamera = false;

    public static Manager instance;

    public int populationSize;
    public float trainingDuration = 30;
    public float mutationRate = 5f;

    public Agent agentPrefab;
    public Transform agentGroup;

    Agent agent;

    List<Agent> agents = new List<Agent>();
    public int selectionNumbers;

    //Team Stuff
    public List<GameObject> blueTeam = new List<GameObject>();
    public List<GameObject> redTeam = new List<GameObject>();
    public List<GameObject> greenTeam = new List<GameObject>();
    public List<GameObject> yellowTeam = new List<GameObject>();

    public int redPoints;
    public int bluePoints;
    public int greenPoints;
    public int yellowPoints;

    //Tous les tracks sont dans un gameobject en enfant.
    //public List<GameObject> stadiumList = new List<GameObject>();
    //public List<GameObject> chosenList = new List<GameObject>();
    [SerializeField] private GameObject stadiumParent;
    public int numberOfTracks;
    //public GameObject LastCheckpointTrack;
    //public GameObject currentTrack;

    //Camera Stuff
    public CameraController cameraController;
    public Camera cameraMain;
    public Camera finishCamera;

    public string[] firstNames;
    public string[] adjectiveNames;

    public TextMeshProUGUI[] rankTextMesh;
    public TextMeshProUGUI[] timingRank;

    public float MaxTime;
    public float currentTime;
    [HideInInspector] public string minutes;
    [HideInInspector] public string seconds;

    public TextMeshProUGUI timerText;
    // Start is called before the first frame update
    void Awake()
    {
        greenScore.text = null;
        redScore.text = null;
        blueScore.text = null;
        yellowScore.text = null;

        
        instance = this;
        Tirage();
    }

    void Start()
    {
        InitialisationTrack();
        StartCoroutine(InitCoroutine());
        
        currentTime = 0;
        
    }

    void InitialisationTrack()
    {
        currentTrack = chosenList[chosenList.Count - 1];
        currentTrack.SetActive(true);
        currentCheckPointManager = currentTrack.GetComponent<TrackBehaviour>().checkpointManager;
        LastCheckpointTrack = currentTrack.GetComponent<TrackBehaviour>().lastCheckpointTrack;
        finishCamera = currentTrack.GetComponent<TrackBehaviour>().finishCamera;
        finishCamera.enabled = false;
    }

    private void Update()
    {
        if (!needFinishCamera)
        {
            ReFocus();
        }
        else
        {
            cameraMain.enabled = false;
            finishCamera.enabled = true;
        }

        if (currentTrack.GetComponent<TrackBehaviour>().numberOfFinishedCar <10)
        {
            StartCoroutine(UpdateLeaderboard());
        }    

       

        currentTime += Time.deltaTime;

        minutes = ((int)currentTime / 60).ToString();
        seconds = (Math.Round(currentTime, 1, MidpointRounding.AwayFromZero) % 60).ToString();

        timerText.text = minutes + " : " + seconds;
    }

    void Tirage()
    {
            foreach (Transform child in stadiumParent.transform)
            {
                stadiumList.Add(child.gameObject);
            }

            for (int i = 0; i < numberOfTracks; i++)
            {
                int randomNumber = UnityEngine.Random.Range(0, stadiumList.Count);
                Debug.Log(randomNumber);
                GameObject chosenTrack = stadiumList[randomNumber];
                chosenList.Add(chosenTrack);
                stadiumList.Remove(stadiumList[randomNumber]);
                
            }
    }



        void NewGeneration()
        {
            agents.Sort();
            AddOrRemoveAgent();
            Mutate();
            Reset();
            SetMaterials(); //À CHANGER, appeler au début du tournoi 

            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].GetComponent<Agent>().needToStop = false;
            
            }
        }

        void SetMaterials()
        {
            for (int i = 0; i < agents.Count; i+=4)
            {
                agents[i].SetTeamRedMaterial();
                agents[i + 1].SetTeamBleuMaterial();
                agents[i + 2].SetTeamGreenMaterial();
                agents[i + 3].SetTeamYellowMaterial();
            }
        }

        void Reset()
        {
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].ResetAgent();
            }
        }

        void Mutate()
        {
            int selectedNumbers = 0;
            for (int i = agents.Count / 2; i < agents.Count; i++)
            {
                agents[i].net.CopyNet(agents[selectedNumbers].net);
                agents[i].net.Mutate(mutationRate);
                
                selectedNumbers++;
                if (selectedNumbers == selectionNumbers)
                {
                    selectedNumbers = 0;
                }
            }
        }

        void AddOrRemoveAgent()
        {
            if (agents.Count != populationSize)
            {
                int dif = populationSize - agents.Count;

                if (dif > 0)
                {
                    //ajouter agent.
                    for (int i = 0; i < dif; i++)
                    {
                        AddAgent();
                    }
                }
                else
                {
                    //supp agent.
                    for (int i = 0; i < dif; i++)
                    {
                        RemoveAgent();
                    }
                }
            }
        }
        void AddAgent()
        {
            agent = Instantiate(agentPrefab, Vector3.zero, Quaternion.identity, agentGroup);
            agent.net = new NeuralNetwork(agentPrefab.net.layers);

            agents.Add(agent);
        }

        void RemoveAgent()
        {
            Destroy((agents[agents.Count - 1]).transform);
            agents.RemoveAt(agents.Count - 1);
        }

        public void ReFocus()
        {
            agents.Sort();
            Focus();
        }

        public void End()
        {
            StopAllCoroutines();
            StartCoroutine(Loop());
        }
        public void ResetNets()
        {
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].net = new NeuralNetwork(agentPrefab.net.layers);
            }

            End();
        }
        public void Save()
        {
            List<NeuralNetwork> nets = new List<NeuralNetwork>();

            for (int i = 0; i < agents.Count; i++)
            {
                nets.Add(agents[i].net);
            }

            DataManager.instance.Save(nets);
        }

        public void Load()
        {
            Data data = DataManager.instance.Load();

            if (data != null)
            {
                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].net = data.nets[i];

                }
            }

            End();
        }


        void Focus()
        {
            NeuralNetworkViewer.instance.agent = agents[0];
            NeuralNetworkViewer.instance.RefreshAxons();
            cameraController.target = agents[0].transform;
        }

        void InitNeuralNetworkViewer()
        {
            NeuralNetworkViewer.instance.Init(agents[0]);
        }

        IEnumerator InitCoroutine()
        {
            NewGeneration();
            InitNeuralNetworkViewer();
            InitAgentName();
            Focus();
       
            yield return new WaitForSeconds(2f);
            End();
            
            yield return new WaitForSeconds(trainingDuration);
            StartCoroutine(Loop());
        }

        IEnumerator Loop()
        {
        #region
            //Reset du Score des Equipes pour la Run
            greenScore.text = null;
            redScore.text = null;
            blueScore.text = null;
            yellowScore.text = null;
            numberOfTraining++;
        #endregion

         if(numberOfTraining < fastTraining)
         {
                yield return new WaitForSeconds(2f);
                End();
         }

            cameraMain.enabled = true;
            currentTime = MaxTime;
            needFinishCamera = false;

            currentTrack.GetComponent<TrackBehaviour>().ResetTrack();
            currentTrack.GetComponent<TrackBehaviour>().numberOfFinishedCar = 0;
            ResetTeam();
            NewGeneration();
            Focus();
            currentTrack.GetComponent<TrackBehaviour>().finishOrder = new List<GameObject>();
            yield return new WaitForSeconds(trainingDuration);
            numberOfTraining++;

            if(numberOfTraining > trainingBeforeRun)
            {
                StartCoroutine(Run());
            }
            else
            {
            StartCoroutine(Loop());
            }

        }
        
        IEnumerator Run()
        {
            cameraMain.enabled = true;
            currentTime = MaxTime;
            needFinishCamera = false;

            currentTrack.GetComponent<TrackBehaviour>().ResetTrack();
            currentTrack.GetComponent<TrackBehaviour>().numberOfFinishedCar = 0;
            ResetTeam();
            NewGeneration();
            Focus();
            currentTrack.GetComponent<TrackBehaviour>().finishOrder = new List<GameObject>();
            yield return new WaitForSeconds (trainingDuration);

            //Set le Score
            greenScore.text = greenPoints.ToString();
            redScore.text = redPoints.ToString();
            blueScore.text = bluePoints.ToString();
            yellowScore.text = yellowPoints.ToString();
            yield return new WaitForSeconds(10f);
            StartCoroutine(NextTrack());

        }

        IEnumerator NextTrack()
        {
            numberOfTraining = 0;

            currentTrack.SetActive(false);
            stadiumList.Remove(currentTrack);
            InitialisationTrack();



            NewGeneration();
            InitNeuralNetworkViewer();
            InitAgentName();
            Focus();

            yield return new WaitForSeconds(2f);
            End();

            yield return new WaitForSeconds(trainingDuration);
            StartCoroutine(Loop());
        }

        void ResetTeam()
        {
            blueTeam.Clear();
            redTeam.Clear();
            greenTeam.Clear();
            yellowTeam.Clear();
            

        for (int i = 0; i < timingRank.Length; i++)
            {
                timingRank[i].text = "00:00:00";
            }
        }

        // sert à attribuer un nom à la voiture
        void InitAgentName()
        {
            for (int i = 0; i < agents.Count; i++)
            {

                bool isFirstNameAlreadyIn;
                bool isAdjectiveAlreadyIn;
                bool isFullNameAlreadyIn;
                int selectFirstName;
                int selectAdjectiveName;
                if (agents[i].fullName == string.Empty)
                {
                    do
                    {
                        isFullNameAlreadyIn = false;
                        selectFirstName = UnityEngine.Random.Range(0, firstNames.Length);
                        selectAdjectiveName = UnityEngine.Random.Range(0, adjectiveNames.Length);
                        foreach (Agent agent in agents)
                        {
                            isFirstNameAlreadyIn = false;
                            isAdjectiveAlreadyIn = false;
                            if (agent.fullName != string.Empty)
                            {
                                if (agent.firstName == firstNames[selectFirstName])
                                {
                                    isFirstNameAlreadyIn = true;
                                }
                                if (agent.adjectiveName == adjectiveNames[selectAdjectiveName])
                                {
                                    isAdjectiveAlreadyIn = true;
                                }

                                if (isAdjectiveAlreadyIn && isFirstNameAlreadyIn)
                                {
                                    isFullNameAlreadyIn = true;
                                }
                            }
                        }

                    } while (isFullNameAlreadyIn == true);


                    agents[i].firstName = firstNames[selectFirstName];
                    agents[i].adjectiveName = adjectiveNames[selectAdjectiveName];
                    agents[i].fullName = firstNames[selectFirstName] + " " + adjectiveNames[selectAdjectiveName];
                    agents[i].text.text = agents[i].fullName;
                }
            }
        }
    
    IEnumerator UpdateLeaderboard()
    {
        agents.Sort();
        for (int i = 0; i < 10; i++)
        {
            rankTextMesh[i].text = agents[i].fullName + " " + ((int)agents[i].fitness).ToString();

            if (agents[i].myTeam == Agent.Team.Bleu)
            {
                rankTextMesh[i].color = Color.blue;
            }
            if (agents[i].myTeam == Agent.Team.Red)
            {
                rankTextMesh[i].color = Color.red;
            }
            if (agents[i].myTeam == Agent.Team.Yellow)
            {
                rankTextMesh[i].color = Color.yellow;
            }
            if (agents[i].myTeam == Agent.Team.Green)
            {
                rankTextMesh[i].color = Color.green;
            }

        }

        yield return null;
    }

}

