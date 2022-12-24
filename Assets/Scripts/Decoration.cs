using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    [SerializeField] public float lifetime = 5.0f;
    [SerializeField] public DecorationInfo decorationReference;

    public bool stuck; 

    void Awake()
    {
        stuck = false;
    }


    void Update()
    {
        if (!stuck)
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
