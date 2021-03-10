using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform centerOfMass;

    //Les Roues.
    public Transform wheelFrontLeftModel, wheelFrontRightModel, wheelRearLeftModel, wheelRearRightModel;
    public WheelCollider wheelFrontLeftCollider, wheelFrontRightCollider, wheelRearLeftCollider, wheelRearRightCollider;
    
    //Input
    public float horizontalInput;
    public float verticalInput;
    
    //vehicule Values
    public float maxSteerAngle = 42f;
    public float motorForce = 500f;


    private void Start()
    {
        rb.centerOfMass = centerOfMass.localPosition;
    }

    // Update is called once per frame (Physics + Temps)
    void FixedUpdate()
    {
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    void Steer()
    {
        wheelFrontLeftCollider.steerAngle = horizontalInput * maxSteerAngle;
        wheelFrontRightCollider.steerAngle = horizontalInput * maxSteerAngle;
    }

    void Accelerate()
    {
        wheelRearLeftCollider.motorTorque = verticalInput * motorForce;
        wheelRearRightCollider.motorTorque = verticalInput * motorForce;
    }

    void UpdateWheelPoses()
    {
        updateWheelPose(wheelFrontLeftCollider, wheelFrontLeftModel);
        updateWheelPose(wheelFrontRightCollider, wheelFrontRightModel);
        updateWheelPose(wheelRearLeftCollider, wheelRearLeftModel);
        updateWheelPose(wheelRearRightCollider, wheelRearRightModel);
    }

    Vector3 pos;
    Quaternion quat;

    void updateWheelPose(WheelCollider col,Transform tr)
    {//Fait correspondre la pos des roues (model) avec leurs colliders).

        Vector3 pos = tr.position;
        Quaternion quat = tr.rotation;

        col.GetWorldPose(out pos, out quat);

        tr.position = pos;
        tr.rotation = quat;
    }

    public void Reset()
    {
        horizontalInput = 0;
        verticalInput = 0;
    }

}
