using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketCamera : MonoBehaviour
{

    [SerializeField] Camera camera;


    void LateUpdate()
    { 
            Vector2 mousePos = PlayerController.GetGameCameraMousePosition();
            Vector3 aimPoint = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.0f));
            transform.LookAt(aimPoint);

    }
    
}
