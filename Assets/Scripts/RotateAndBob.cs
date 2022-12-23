using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndBob : MonoBehaviour
{
    [SerializeField] private Transform indicatorTransform;
    [SerializeField] public float bobPeriod = 1.0f;
    [SerializeField] public float bobAmplitude = 0.1f;
    [SerializeField] public float rotationSpeed = 1.0f;

    private float bobTime;
    private void Start()
    {
        ResetBobTime();
    }

    void Update()
    {
        bobTime += Time.deltaTime;
        indicatorTransform.localPosition = new Vector3(0, -bobAmplitude * Mathf.Sin(3.14159f / bobPeriod * bobTime), 0);
        indicatorTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void ResetBobTime()
    {
        bobTime = 0;
    }
}
