using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using System;

public class Decoratable : MonoBehaviour
{
    public MeshCollider meshCollider;

    [SerializeField] private float decorationDistance = 0.1f;

    private Mesh mesh;
    List<Color32> colourList;

    private int decorationLayer;

    List<Decoration> decorations;

    public RequirementBlock[] requirements;
    [NonSerialized] public bool completed;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        
        colourList = (new Color32[mesh.vertexCount]).ToList();
        ResetVertexColours();

        decorationLayer = LayerMask.NameToLayer("Decoration");

        decorations = new List<Decoration>();

        completed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != decorationLayer) return;

        ContactPoint contact = collision.GetContact(0);

        if (collision.gameObject.tag != "SnowBall")
        {
            Decoration deco = collision.gameObject.GetComponent<Decoration>();
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
            deco.stuck = true;
            rigidbody.detectCollisions = false;
            Destroy(rigidbody);

            collision.transform.position = contact.point - contact.normal * decorationDistance;
            decorations.Add(deco);
        }
        else {

            Ray ray = new Ray(contact.point - contact.normal * 0.01f, contact.normal);
            RaycastHit hit;
            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Check if the raycast hit a triangle
                if (hit.triangleIndex >= 0)
                {
                    ColourTriangle(hit.triangleIndex, new Color32(255, 255, 255, 255));
                }
            }
            
            Destroy(collision.gameObject);
        }

        completed = CheckRequirement();
    }

    public void ResetVertexColours()
    {
        Color32 transparent = new Color32(0, 0, 0, 0);
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
            if (i % 3 == 0) triColour = new Color32((byte)UnityEngine.Random.Range(0.0f, 255.0f), (byte)UnityEngine.Random.Range(0.0f, 255.0f), (byte)UnityEngine.Random.Range(0.0f, 255.0f), (byte)UnityEngine.Random.Range(0.0f, 255.0f));

            colourList[i] = triColour;
        }

        mesh.colors32 = colourList.ToArray();
    }

    public void ColourTriangle(int triIndex, Color32 col)
    {
        colourList[triIndex * 3 + 0] = col;
        colourList[triIndex * 3 + 1] = col;
        colourList[triIndex * 3 + 2] = col;

        mesh.colors32 = colourList.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void Move(Vector3 newPosition) {

        Vector3 offset = newPosition - transform.parent.position;
        transform.parent.position = newPosition;
        foreach (Decoration decoration in decorations) {

            decoration.transform.position = decoration.transform.position + offset;
        }
    }

    public bool CheckRequirement() {

        Dictionary<string, int> amounts = new Dictionary<string, int>();
        
        foreach (Decoration decoration in decorations)
        {
            string key = decoration.decorationReference.displayName;
            if (amounts.ContainsKey(key))
            {
                amounts[key] = amounts[key] + 1;
            }
            else {
                amounts.Add(key,1);
            }
           
        }

        foreach (RequirementBlock requirementBlock in requirements) {

            if (requirementBlock.requirement == RequirementBlock.Type.GREATER_THAN) {
                if (!amounts.ContainsKey(requirementBlock.decorationInfo.displayName)) return false;
                if (amounts[requirementBlock.decorationInfo.displayName] <= requirementBlock.amount) return false;
                continue;
            }
            else if (requirementBlock.requirement == RequirementBlock.Type.LESS_THAN)
            {

                if (!amounts.ContainsKey(requirementBlock.decorationInfo.displayName) && requirementBlock.amount > 0 ) continue;
                if (!amounts.ContainsKey(requirementBlock.decorationInfo.displayName) ) return false;
                if (amounts[requirementBlock.decorationInfo.displayName] >= requirementBlock.amount) return false;
                continue;
            }
            else if (requirementBlock.requirement == RequirementBlock.Type.EQUAL_TO)
            {
                if (!amounts.ContainsKey(requirementBlock.decorationInfo.displayName) && requirementBlock.amount == 0) continue;
                if (!amounts.ContainsKey(requirementBlock.decorationInfo.displayName)) return false;
                if (amounts[requirementBlock.decorationInfo.displayName] != requirementBlock.amount) return false;
                continue;
            }
        }

        
        return true;
    }
    
}
