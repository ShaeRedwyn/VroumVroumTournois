using UnityEngine;
using System;

public class Agent : MonoBehaviour , IComparable<Agent>
{
    public NeuralNetwork net;
    public CarController carController;

    public float fitness;
    public float distanceTraveled;

    public Rigidbody rb;

    float[] inputs;

    public Transform nextCheckpoint;
    public Transform lastCheckpoint;

    public float nextCheckpointDist;

    public bool needToStop = false;
    //Name
    public string fullName;
    public string firstName;
    public string adjectiveName;

    public void ResetAgent()
    {
        needToStop = false;
        fitness = 0;
        distanceTraveled = 0;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        inputs = new float[net.layers[0]];

        carController.Reset();

        nextCheckpoint = CheckpointManager.Instance.firstCheckpoint;

        nextCheckpointDist = (transform.position - nextCheckpoint.position).magnitude;
    }

    private void FixedUpdate()
    {
        if (!needToStop)
        {
            InputUpdate();
            OutputUpdate();
            FitnessUpdate();
        }
        else
        {
            rb.velocity -= rb.velocity * 0.001f;
            rb.angularVelocity -= rb.velocity *0.001f;
            if (rb.velocity.magnitude < 1)
            {
                carController.Reset();
                Debug.Log("ici");
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
       
        
        
    }
    void InputUpdate()
    {
        inputs[0] = RaySensor(transform.position + Vector3.up * 0.2f,transform.forward,4);
        inputs[1] = RaySensor(transform.position + Vector3.up * 0.2f, transform.right, 1.5f);
        inputs[2] = RaySensor(transform.position + Vector3.up * 0.2f, -transform.right, 1.5f);
        inputs[3] = RaySensor(transform.position + Vector3.up *0.2f, transform.forward + transform.right, 2f);
        inputs[4] = RaySensor(transform.position + Vector3.up * 0.2f, transform.forward - transform.right, 2f);

        inputs[5] = (float)Math.Tanh(rb.velocity.magnitude * 0.05f);
        inputs[6] = (float)Math.Tanh(rb.angularVelocity.y * 0.1f);
        inputs[7] = 1;
    }

    RaycastHit hit;
    float range = 4f;
    public  LayerMask layerMask;
    float RaySensor(Vector3 pos, Vector3 direction, float lenght)
    {
        if (Physics.Raycast(pos, direction,out hit, lenght * range, layerMask))
        {
            Debug.DrawRay(pos, direction*hit.distance,Color.Lerp(Color.red, Color.green, (range * lenght - hit.distance) / (range * lenght)));
            return (range * lenght - hit.distance) / (range * lenght);
        }
        else
        {
            Debug.DrawRay(pos, direction * range * lenght, Color.red);
            return 0;
        }    
    }

    void OutputUpdate()
    {
        net.FeedForward(inputs);

        carController.horizontalInput = net.neurons[net.layers.Length - 1][0];
        carController.verticalInput = net.neurons[net.layers.Length - 1][1];
        
    }

    float currentDistance;
    void FitnessUpdate()
    {
        currentDistance = distanceTraveled + (nextCheckpointDist - (transform.position-nextCheckpoint.position).magnitude);

        if (fitness < currentDistance)
        {
            fitness = currentDistance;
        }
    }

    public void CheckpointReached(Transform checkpoint)
    {
        distanceTraveled += nextCheckpointDist;
        nextCheckpoint = checkpoint;
        nextCheckpointDist = (transform.position-checkpoint.position).magnitude;
    }


    public Renderer render;
    public Material firstMaterial;
    public Material defaultMaterial;
    public Material mutatedMaterial;
    public void SetFirstMaterial()
    {
        render.material = firstMaterial;
    }

    public void SetMutatedMaterial()
    {
        render.material = mutatedMaterial;
    }

    public void SetDefaultMaterial()
    {
        render.material = defaultMaterial;
    }

    public int CompareTo(Agent other)
    {
        if (fitness < other.fitness)
        {
            return 1;
        }

        if (fitness > other.fitness)
        {
            return -1;
        }

        return 0;
    }
}
