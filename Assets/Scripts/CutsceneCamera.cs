using UnityEngine;
using System;

public class CutsceneCamera : MonoBehaviour
{
    public static Action<Vector3, Vector3, Vector3, float> EventChangeCutsceneCamera;

    private Camera camera;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float duration;
    private float timer;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        duration = 0.0f;
        timer = 0.0f;

        EventChangeCutsceneCamera += ChangeCamera;
    }

    private void OnDestroy()
    {
        EventChangeCutsceneCamera -= ChangeCamera;
    }

    void Update()
    {
        if (timer >= duration) return;

        camera.transform.position = startPosition + timer / duration * (endPosition - startPosition);

        timer += Time.deltaTime;
    }

    public void ChangeCamera(Vector3 rotation, Vector3 newStartPosition, Vector3 newEndPosition, float newDuration)
    {
        camera.transform.rotation = Quaternion.Euler(rotation);
        camera.transform.position = newStartPosition;

        startPosition = newStartPosition;
        endPosition = newEndPosition;
        duration = newDuration;
        timer = 0.0f;
    }
}
