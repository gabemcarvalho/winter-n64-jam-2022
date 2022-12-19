using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    public float maxScale = 1.0f;
    public float growTime = 0.3f;
    public float staticTime = 1.0f;
    public float shrinkTime = 3.0f;

    private float time;
    
    void Awake()
    {
        time = 0.0f;
    }

    void Update()
    {
        time += Time.deltaTime;

        float scale = maxScale;
        if (time < growTime)
        {
            scale = maxScale * time / growTime;
        }
        else if (time > growTime + staticTime)
        {
            if (time > growTime + staticTime + shrinkTime)
            {
                Destroy(gameObject);
                return;
            }

            scale = maxScale * (1.0f - (time - growTime - staticTime) / shrinkTime);
        }

        transform.localScale = new Vector3(scale, scale, scale);
    }
}
