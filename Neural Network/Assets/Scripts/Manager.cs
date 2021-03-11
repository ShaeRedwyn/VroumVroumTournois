﻿using System.Collections;
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
    
    //Tous les tracks sont dans un gameobject en enfant.
    public List<GameObject> stadiumList = new List<GameObject>();
    public List<GameObject> chosenList = new List<GameObject>();
    [SerializeField] private GameObject stadiumParent;
    public int numberOfTracks;
    public GameObject LastCheckpointTrack;

    public CameraController cameraController;

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
    void Tirage()
    {
        foreach (Transform child in stadiumParent.transform)
        {
            stadiumList.Add(child.gameObject);
        }

        for (int i = 0; i < numberOfTracks; i++)
        {
            int randomNumber = Random.Range(0, stadiumList.Count);
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
    }
    void SetMaterials()
    {
        for (int i = 0; i < 32; i++)
        {
            agents[i].SetGoldMaterial();
        }

        for (int i = 32; i < 65; i++)
        {
            agents[i].SetSilverMaterial();
        }

        for (int i = 65; i < 99; i++)
        {
            agents[i].SetBronzeMaterial();
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
        for (int i = agents.Count / 2; i < agents.Count; i++)
        {
            agents[i].net.CopyNet(agents[i-agents.Count/2].net);
            agents[i].net.Mutate(mutationRate);
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

}
