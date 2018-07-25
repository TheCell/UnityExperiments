using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshImpactDeform : MonoBehaviour
{
    private TerrainGenerator terrainGenerator;
    private Vector3[] terrainVertices;
    private int xVerticesCount;
    private int yVerticesCount;
    public float impactStrength = 10.0f;

    private void updateMeshData()
    {
        if (this.terrainGenerator != null)
        {
            this.terrainGenerator.setVertices(terrainVertices);
        }
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
        initializeVariables();
        displayImpact(collision);
        applyImpact(collision);
        updateMeshData();
    }

    private void applyImpact(Collision collision)
    {
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 collisionNormal = collision.contacts[0].normal;
        int closestIndexes = findClosestVertexIndex(collisionPoint);
        if (closestIndexes == -1)
        {
            print("vertex index not found");
            return;
        }
        Vector3 imapctVector = collisionNormal * this.impactStrength;
        Vector3 impactVertex = this.terrainVertices[closestIndexes];

        impactVertex = impactVertex + imapctVector;
        impactVertex.y = 180;
        this.terrainVertices[closestIndexes] = impactVertex;
    }

    private int findClosestVertexIndex(Vector3 point)
    {
        // x,z is the unity plane. find closest vertex in array
        // return findClosestXIndex(point) + (findClosestZIndex(point) * this.xVerticesCount);
        Debug.Log("temporary slow function, something else seems to be off");

        int vertexAmount = this.xVerticesCount * this.yVerticesCount;
        int index = -1;
        float currentClosestDistance = float.MaxValue;

        for (int i = 0; i < vertexAmount; i++)
        {
            float pointDistanceToCurrentVertex = 
                Mathf.Abs(this.terrainVertices[i].x - point.x)
                + Mathf.Abs(this.terrainVertices[i].y - point.y)
                + Mathf.Abs(this.terrainVertices[i].z - point.z);
            if (pointDistanceToCurrentVertex < currentClosestDistance)
            {
                currentClosestDistance = pointDistanceToCurrentVertex;
                index = i;
            }
        }

        return index;
    }

    private int findClosestXIndex(Vector3 point)
    {
        int index = -1;
        float currentClosestDistance = float.MaxValue;

        for (int i = 0; i < this.xVerticesCount; i++)
        {
            float pointDistanceToCurrentVertex = Mathf.Abs(this.terrainVertices[i].x - point.x);
            if (pointDistanceToCurrentVertex < currentClosestDistance)
            {
                index = i;
                currentClosestDistance = pointDistanceToCurrentVertex;
            }
        }

        return index;
    }

    private int findClosestZIndex(Vector3 point)
    {
        Debug.Log("this is still not working. seems to have the wrong scale??");
        int index = -1;
        float currentClosestDistance = float.MaxValue;

        for (int i = 0; i < this.yVerticesCount; i++)
        {
            float pointDistanceToCurrentVertex = Mathf.Abs(this.terrainVertices[i * this.xVerticesCount].z - point.z);
            if (pointDistanceToCurrentVertex < currentClosestDistance)
            {
                index = i;
                currentClosestDistance = pointDistanceToCurrentVertex;
            }
        }

        return index;
    }

    private bool initializeVerticeData()
    {
        if (this.terrainGenerator != null)
        {
            this.terrainVertices = terrainGenerator.getVertices();
            this.xVerticesCount = terrainGenerator.getXVerticesCount();
            this.yVerticesCount = terrainGenerator.getYVerticesCount();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void initializeVariables()
    {
        this.terrainGenerator = GetComponent<TerrainGenerator>();
        if (this.terrainGenerator != null)
        {
            if (!initializeVerticeData())
            {
                print("Data could not be loaded, something went wrong with the terrainGenerator");
            }
        }
        else
        {
            print("this collision Object has no terrainGenerator script");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("collision impact at " + collision.contacts[0].point);
        impact(collision);
        Destroy(collision.gameObject);
    }


}
