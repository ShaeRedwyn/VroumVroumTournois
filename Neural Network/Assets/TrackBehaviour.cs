using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBehaviour : MonoBehaviour
{
    private BoxCollider boxCol;

    public List<GameObject> finishOrder = new List<GameObject>();

    public GameObject lastCheckpointTrack;
    public CheckpointManager checkpointManager;
    public GameObject track;
    public Camera finishCamera;
    public Manager manager;
    public int maxPoints = 100;

    public int numberOfFinishedCar = 0;
    //public List<GameObject> finishOrder = new List<GameObject>();

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Agent>().lastCheckpoint == Manager.instance.LastCheckpointTrack.transform)
        {
            GameObject car = other.transform.parent.gameObject;
            finishOrder.Add(car);
            car.GetComponent<Agent>().points += maxPoints;
            maxPoints--;
            car.GetComponent<Agent>().needToStop = true;

            if(numberOfFinishedCar < 10)
            {
                manager.timingRank[numberOfFinishedCar].text = manager.minutes + " : " + manager.seconds;
                numberOfFinishedCar++;
            }

           
        }
    }

   

    public void ResetTrack()
    {
        for (int i = 0; i < numberOfFinishedCar; i++)
        {
            manager.timingRank[i].text = "00:00:00";
        }

        for (int i = 0; i < Manager.instance.blueTeam.Count; i++)
        {
            Manager.instance.bluePoints += Manager.instance.blueTeam[i].GetComponent<Agent>().points;
        }
        for (int i = 0; i < Manager.instance.redTeam.Count; i++)
        {
            Manager.instance.redPoints += Manager.instance.redTeam[i].GetComponent<Agent>().points;
        }
        for (int i = 0; i < Manager.instance.blueTeam.Count; i++)
        {
            Manager.instance.greenPoints += Manager.instance.greenTeam[i].GetComponent<Agent>().points;
        }
        for (int i = 0; i < Manager.instance.yellowTeam.Count; i++)
        {
            Manager.instance.yellowPoints += Manager.instance.yellowTeam[i].GetComponent<Agent>().points;
        }

        maxPoints = 100;
        numberOfFinishedCar = 0;
        Manager.instance.currentTime = 0;
        Manager.instance.minutes = "0";
        Manager.instance.seconds = "0";


        finishOrder.Clear();
    }

        
    
}
