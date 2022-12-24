using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiniGameCamera : MonoBehaviour
{

    public static Action<bool> EventRotateAroundTree;

    Camera miniGameCam;
    Transform treePos;
    [SerializeField] float distance = 10f;

    private float angle = 0;
    [SerializeField] float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
        miniGameCam = GetComponent<Camera>();

        EventRotateAroundTree += RotateAroundTree;
        
        
    }

    private void OnDestroy()
    {
        EventRotateAroundTree -= RotateAroundTree;
    }

    private void Update()
    {
        if (treePos == null) return; 
        
        miniGameCam.transform.LookAt(treePos.position +  Vector3.up);
        
    }

    public void FixOnTree(Transform treetransform) {
        miniGameCam = GetComponent<Camera>();
        treePos = treetransform;
        Debug.Log(treePos + "tree pos");
        Debug.Log(miniGameCam);
        miniGameCam.transform.position = treePos.position + distance * new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

    }


    void RotateAroundTree(bool left) {
        if (treePos == null) return;

        if (left)
        {

            angle += speed * Time.deltaTime;
        }
        else {
            angle -= speed * Time.deltaTime;
        }

        miniGameCam.transform.position = treePos.position + distance * new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

    }
    
}
