using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<Agent>())
        {
            //verifier si tu as pas grillé un checkpoint
            if (other.transform.parent.GetComponent<Agent>().lastCheckpoint == transform)
            {
                if (Manager.instance.needFinishCamera == false)
                {
                    Debug.Log("called");
                    Manager.instance.needFinishCamera = true;
                }
                
            }
        }
    }
}
