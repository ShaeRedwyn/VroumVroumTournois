using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform lastCheckpoint;

    public Transform nextCheckpoint;

    private void Awake()
    {
        lastCheckpoint = gameObject.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Agent>())
        {
            //verifier si tu as pas grillé un checkpoint
            if (other.transform.parent.GetComponent<Agent>().nextCheckpoint == transform)
            {
                //je te transmets le second checkpoint.
                other.transform.parent.GetComponent<Agent>().CheckpointReached(nextCheckpoint);
                other.transform.parent.GetComponent<Agent>().lastCheckpoint = gameObject.transform;
            }
        }        
        
    }
}
