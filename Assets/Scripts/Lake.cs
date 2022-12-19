using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lake : MonoBehaviour
{
    private int playerLayer;
    private int decorationLayer;

    public string icePrefabResource = "IcePlatform";
    private GameObject icePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        decorationLayer = LayerMask.NameToLayer("Decoration");

        icePrefab = Resources.Load(icePrefabResource) as GameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == decorationLayer)
        {
            Instantiate(icePrefab, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            player.OnFellInLake();
        }

        if (other.gameObject.layer == decorationLayer)
        {
            Vector3 icePosition = other.transform.position;
            icePosition.y = transform.position.y + 0.18f;
            Instantiate(icePrefab, icePosition, Quaternion.identity);
            Destroy(other.gameObject);
        }
    }
}
