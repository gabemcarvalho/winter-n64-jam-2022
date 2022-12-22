using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCamera : MonoBehaviour
{
    private Camera camera;

    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float amplitude;
    [SerializeField] private float speed;

    private float time;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        time = 0.0f;
    }

    void Update()
    {
        time += Time.unscaledTime;
        camera.transform.position = position;
        camera.transform.rotation = Quaternion.Euler(rotation + Vector3.up * amplitude * Mathf.Sin(time * speed));
    }
}
