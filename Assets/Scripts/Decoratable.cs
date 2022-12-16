using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;

public class Decoratable : MonoBehaviour
{
    public MeshCollider meshCollider;

    private Mesh mesh;
    List<Color32> colourList;

    private int decorationLayer;
    private int decoratableLayer;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        colourList = (new Color32[mesh.vertexCount]).ToList();
        ResetVertexColours();

        decorationLayer = LayerMask.NameToLayer("Decoration");
        decoratableLayer = LayerMask.NameToLayer("Decoratable");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != decorationLayer) return;

        ContactPoint contact = collision.GetContact(0);
        
        // Cast a ray from the contact point in the direction of the collision normal
        Ray ray = new Ray(contact.point - contact.normal * 0.01f, contact.normal);
        RaycastHit hit;
        if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Check if the raycast hit a triangle
            if (hit.triangleIndex >= 0)
            {
                Debug.Log($"Triangle {hit.triangleIndex} hit!");

                ColourTriangle(hit.triangleIndex, new Color32(255, 255, 255, 255));
            }
        }

        Destroy(collision.gameObject);
    }

    public void ResetVertexColours()
    {
        Color32 transparent = new Color32(255, 0, 0, 0);// new Color32(0, 0, 0, 0);
        for (int i = 0; i < colourList.Count; i++)
        {
            colourList[i] = transparent;
        }

        mesh.colors32 = colourList.ToArray();
    }

    public void RandomVertexColours()
    {
        Color32 triColour = new Color32(0, 0, 0, 0);
        for (int i = 0; i < colourList.Count; i++)
        {
            if (i % 3 == 0) triColour = new Color32((byte)Random.Range(0.0f, 255.0f), (byte)Random.Range(0.0f, 255.0f), (byte)Random.Range(0.0f, 255.0f), (byte)Random.Range(0.0f, 255.0f));

            colourList[i] = triColour;
        }

        mesh.colors32 = colourList.ToArray();
    }

    public void ColourTriangle(int triIndex, Color32 col)
    {
        colourList[triIndex * 3 + 0] = col;
        colourList[triIndex * 3 + 1] = col;
        colourList[triIndex * 3 + 2] = col;

        mesh.colors32 = colourList.ToArray(); // doesn't work??
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
