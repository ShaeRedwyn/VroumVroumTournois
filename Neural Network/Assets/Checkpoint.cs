using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform nextCheckpoint;
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.parent.GetComponent<Agent>())
        {
            //verifier si tu as pas grillé un checkpoint
            if (other.transform.parent.GetComponent<Agent>().nextCheckpoint == transform)
            {
                //je te transmets le second checkpoint.
                other.transform.parent.GetComponent<Agent>().CheckpointReached(nextCheckpoint);
            }
        }
        
        
    }
}
