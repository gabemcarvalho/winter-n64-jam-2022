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
    [SerializeField] float yoffset = 2.0f;

    private float angle = 0;
    [SerializeField] float speed;
    
    void Awake()
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
        
        miniGameCam.transform.LookAt(treePos.position + Vector3.up * yoffset * treePos.localScale.y);
    }

    private void UpdatePosition()
    {
        miniGameCam.transform.position = treePos.position + Vector3.up * yoffset * treePos.localScale.y + distance * treePos.localScale.y * new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
    }

    public void FixOnTree(Transform treetransform)
    {
        miniGameCam = GetComponent<Camera>();
        treePos = treetransform;
        UpdatePosition();
    }


    void RotateAroundTree(bool left)
    {
        if (treePos == null) return;

        if (left)
        {
            angle += speed * Time.deltaTime;
        }
        else
        {
            angle -= speed * Time.deltaTime;
        }

        UpdatePosition();
    }
    
}
