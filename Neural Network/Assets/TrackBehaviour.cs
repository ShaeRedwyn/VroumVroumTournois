using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBehaviour : MonoBehaviour
{

    public CheckpointManager checkpointManager;
    public GameObject track;

    public List<GameObject> finishOrder = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Agent>().lastCheckpoint == Manager.instance.LastCheckpointTrack.transform)
        {
            finishOrder.Add(other.gameObject);
        }
    }
}
