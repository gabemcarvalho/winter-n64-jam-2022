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

    [SerializeField] private PlayerController player;

    private void Awake()
    {
        state = State.OnPlayer;

        playerLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        if (!player.canMove) return;

        switch (state)
        {
            case State.OnPlayer:
                Vector3 targetPosition = player.transform.position + Vector3.up * hoverHeight;
                targetPosition -= player.transform.forward * distanceBehind;
                targetPosition += Vector3.Cross(Vector3.up, player.transform.forward) * distanceBeside;
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount * Time.deltaTime);
                break;
        }
    }
}
