using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCamera : MonoBehaviour
{
    Camera miniGameCam;
    [SerializeField] Transform treePos;
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    // Start is called before the first frame update
    void Awake()
    {
        miniGameCam = GetComponent<Camera>();
        x = -9f;
        y = 1f;
        z = -1f;

    }

    private void Update()
    {
        miniGameCam.transform.position = treePos.position + new Vector3(x, y, z);
        miniGameCam.transform.LookAt(treePos.position);
    }

   
}
