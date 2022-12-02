using UnityEngine;

public class Billboard : MonoBehaviour
{    
    void Update()
    {
        Vector3 target = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        transform.LookAt(target, Vector3.up);
    }
}
