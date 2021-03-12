using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Agent : MonoBehaviour , IComparable<Agent>
{
    public enum Team { Red, Green, Bleu, Yellow };
    public Team myTeam;
    

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

    public int points;

    
    public TextMeshPro text;
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

        nextCheckpoint = Manager.instance.currentCheckPointManager.firstCheckpoint;
        lastCheckpoint = null;
        nextCheckpointDist = (transform.position - nextCheckpoint.position).magnitude;
        UpdateColor();
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
                
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        UpdateColor();

        RespawnOnCheckpoint();        
    }

    

    void InputUpdate()
    {
        inputs[0] = RaySensor(transform.position + Vector3.up * 0.2f,transform.forward,5);
        inputs[1] = RaySensor(transform.position + Vector3.up * 0.2f, transform.right, 2f);
        inputs[2] = RaySensor(transform.position + Vector3.up * 0.2f, -transform.right, 2f);
        inputs[3] = RaySensor(transform.position + Vector3.up *0.2f, transform.forward + transform.right, 3f);
        inputs[4] = RaySensor(transform.position + Vector3.up * 0.2f, transform.forward - transform.right, 3f);

        inputs[5] = (float)Math.Tanh(rb.velocity.magnitude * 0.05f);
        inputs[6] = (float)Math.Tanh(rb.angularVelocity.y * 0.1f);
        inputs[7] = 1;
    }
    public void UpdateColor()
    {
        switch (myTeam)
        {
            case Team.Bleu:
                text.color = Color.blue;
                break;
            case Team.Red:
                text.color = Color.red;
                break;
            case Team.Green:
                text.color = Color.green;
                break;
            case Team.Yellow:
                text.color = Color.yellow;
                break;
            default:

                break;
        }
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

    public void RespawnOnCheckpoint()    
    {
        freezeTime = 0;

        if (transform.position.y < -10)
        {
            if(lastCheckpoint != null)
            {
                transform.position = lastCheckpoint.transform.position + new Vector3(0f,1f,0f) ;
            }
            else if(lastCheckpoint == null)
            {
                transform.position = Vector3.zero;
            }

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            //Debug.Log("Car Respawn");
        }
    }


    public Renderer render;
    public Material redMaterial;
    public Material greenMaterial;
    public Material bleuMaterial;
    public Material yellowMaterial;

    public void SetTeamRedMaterial()
    {
        render.material = redMaterial;
        myTeam = Team.Red;
        Manager.instance.redTeam.Add(gameObject);
    }

    public void SetTeamYellowMaterial()
    {
        render.material = yellowMaterial;
        myTeam = Team.Yellow;
        Manager.instance.yellowTeam.Add(gameObject);
    }

    public void SetTeamBleuMaterial()
    {
        render.material = bleuMaterial;
        myTeam = Team.Bleu;
        Manager.instance.blueTeam.Add(gameObject);
    }


    public void SetTeamGreenMaterial()
    {
        render.material = greenMaterial;
        myTeam = Team.Green;
        Manager.instance.greenTeam.Add(gameObject);
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
