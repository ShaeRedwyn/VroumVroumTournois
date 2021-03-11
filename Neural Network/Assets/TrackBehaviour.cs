using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBehaviour : MonoBehaviour
{
    public List<GameObject> finishOrder = new List<GameObject>();

    public GameObject lastCheckpointTrack;
    public CheckpointManager checkpointManager;
    public GameObject track;
    public Camera finishCamera;

    public int maxPoints = 100;
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
        }
    }

    public void ResetTrack()
    {
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


        finishOrder.Clear();
    }

        
    
}
