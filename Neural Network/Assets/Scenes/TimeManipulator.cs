using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManipulator : MonoBehaviour
{
    [Range(0.5f, 20f)] public float timeScale = 1;

    private float baseTimeScale;
    private float baseFixedUpdateRate;
    void Start()
    {
        baseTimeScale = Time.timeScale;
        baseFixedUpdateRate = Time.fixedDeltaTime * 1f;
    }

    private void Update()
    {
        Time.timeScale = timeScale * baseTimeScale;
        Time.fixedDeltaTime = baseFixedUpdateRate * (1 / timeScale);
    }
}
