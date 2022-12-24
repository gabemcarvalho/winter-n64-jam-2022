using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    [SerializeField] public float lifetime = 5.0f;

    [SerializeField] DecorationInfo decorationReference;

    public bool stuck; 

    void Start()
    {
        stuck = false;
    }


    void Update()
    {


        if (!stuck)
        {
            life();
        }
        
        
    }

    public void life()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
