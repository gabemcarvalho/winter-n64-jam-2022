using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    enum State
    {
        OnPlayer
    }

    private State state;

    [SerializeField] private float hoverHeight = 1.8f;
    [SerializeField] private float distanceBehind = 0.5f;
    [SerializeField] private float distanceBeside = 0.5f;
    [SerializeField] private float lerpAmount = 5.0f;

    private int playerLayer;

    [SerializeField] private Transform followTransform;

    private void Awake()
    {
        state = State.OnPlayer;

        playerLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        switch (state)
        {
            case State.OnPlayer:
                Vector3 targetPosition = followTransform.position + Vector3.up * hoverHeight;
                targetPosition -= followTransform.forward * distanceBehind;
                targetPosition += Vector3.Cross(Vector3.up, followTransform.forward) * distanceBeside;
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount * Time.deltaTime);
                break;
        }
    }
}
