using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBehaviour : MonoBehaviour
{
    public GameObject lastCheckpointTrack;
    public CheckpointManager checkpointManager;
    public GameObject track;
    public Camera finishCamera;

    //public List<GameObject> finishOrder = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Agent>().lastCheckpoint == Manager.instance.LastCheckpointTrack.transform)
        {
            GameObject car = other.transform.parent.gameObject;
            finishOrder.Add(car);
            car.GetComponent<Agent>().needToStop = true;
        }
    }
}
