using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Vector3 cameraLocalPosition;
    public Vector3 localTargetLookAtPosition;

    public float positionlerpSpeed = 0.01f;
    public float lookLerpSpeed = 0.01f;
    
    Vector3 wantedPos;
    Quaternion wantedRotation;


    // Update is called once per frame
    void Update()
    {
        //Pour la position.
        //transform une position locale en world position.
        wantedPos = target.TransformPoint(cameraLocalPosition);
        wantedPos.y = cameraLocalPosition.y + target.position.y;
        transform.position = Vector3.Lerp(transform.position, wantedPos, positionlerpSpeed);

        //pour la rotation.
        wantedRotation = Quaternion.LookRotation(target.TransformPoint(localTargetLookAtPosition) - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, lookLerpSpeed);
    }
}
