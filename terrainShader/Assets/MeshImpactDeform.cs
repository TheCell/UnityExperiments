using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshImpactDeform : MonoBehaviour
{
    private MeshFilter terrainMesh;

    private void updateMeshData()
    {
        terrainMesh = GetComponent<MeshFilter>();
    }

    private GameObject createSphereAt(Vector3 point)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "createdSphere";
        sphere.GetComponent<Transform>().localScale = new Vector3(0.2f, 0.2f, 0.2f);
        sphere.GetComponent<Transform>().position = point;
        setMaterialColor(new Color(1.0f, 0.7f, 0.0f), getMaterial(sphere));
        return sphere;
    }

    private void setMaterialColor(Color color, Material materialToSet)
    {
        materialToSet.color = color;
    }

    private Material getMaterial(GameObject objectForMaterial)
    {
        Material debugMaterial = objectForMaterial.GetComponent<MeshRenderer>().material;
        if (debugMaterial == null)
        {
            debugMaterial = new Material(Shader.Find("Standard"));
        }

        return debugMaterial;
    }

    private void displayImpact(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            createSphereAt(contact.point);
        }
    }

    public void impact(Collision collision)
    {
        updateMeshData();
        displayImpact(collision);
    }

    void OnCollisionEnter(Collision collision)
    {
        print("collision impact at " + collision.contacts[0].point);
        impact(collision);
    }
}
