using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public int populationSize;
    public float trainingDuration = 30;
    public float mutationRate = 5f;

    public Agent agentPrefab;
    public Transform agentGroup;

    Agent agent;

    List<Agent> agents = new List<Agent>();

    public int selectionNumbers;
    
    //Tous les tracks sont dans un gameobject en enfant.
    //public List<GameObject> stadiumList = new List<GameObject>();
    //public List<GameObject> chosenList = new List<GameObject>();
    [SerializeField] private GameObject stadiumParent;
    public int numberOfTracks;
    //public GameObject LastCheckpointTrack;
    //public GameObject currentTrack;

    public CameraController cameraController;

    public string[] firstNames;
    public string[] adjectiveNames;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        //Tirage();
    }

    void Start()
    {
        StartCoroutine(InitCoroutine());
    }
    //void Tirage()
    //{
    //    foreach (Transform child in stadiumParent.transform)
    //    {
    //        stadiumList.Add(child.gameObject);
    //    }

    //    for (int i = 0; i < numberOfTracks; i++)
    //    {
    //        int randomNumber = Random.Range(0, stadiumList.Count);
    //        Debug.Log(randomNumber);
    //        GameObject chosenTrack = stadiumList[randomNumber];
    //        chosenList.Add(chosenTrack);
    //        stadiumList.Remove(stadiumList[randomNumber]);
    //    }

    //    LastCheckpointTrack = currentTrack.GetComponent<TrackBehaviour>().lastCheckpointTrack;
    //}
    
    void NewGeneration()
    {
        agents.Sort();
        AddOrRemoveAgent();
        Mutate();
        Reset();
        SetMaterials();
    }
    void SetMaterials()
    {
        agents[0].SetFirstMaterial();
        for (int i = 1; i < agents.Count/2; i++)
        {
            agents[i].SetDefaultMaterial();
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
            agents[i].SetMutatedMaterial();
            selectedNumbers++;
            if(selectedNumbers == selectionNumbers)
            {
                selectedNumbers = 0;
            }
        }
    }

    void AddOrRemoveAgent()
    {
        if (agents.Count !=populationSize)
        {
            int dif = populationSize - agents.Count;

            if (dif >0)
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
       agent = Instantiate(agentPrefab, Vector3.zero,Quaternion.identity, agentGroup);
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
        yield return new WaitForSeconds(trainingDuration);
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        NewGeneration();
        Focus();
        yield return new WaitForSeconds(trainingDuration);
        StartCoroutine(Loop());
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
                    selectFirstName = Random.Range(0, firstNames.Length);
                    selectAdjectiveName = Random.Range(0, adjectiveNames.Length);
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
}
