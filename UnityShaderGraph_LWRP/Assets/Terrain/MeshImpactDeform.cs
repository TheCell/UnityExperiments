using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshImpactDeform : MonoBehaviour
{
    private TerrainGenerator terrainGenerator;
    private Vector3[] terrainVertices;
    private int xVerticesCount;
    private int yVerticesCount;
    private Dictionary<int, float> impactStampIndexAndStrength; // HashMap in Java
    private bool ScriptIsActive = false;
    public float impactStrength = 1.0f;
    public int impactSize = 3;

    public void Start()
    {
        ScriptIsActive = true;
    }

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
        //displayImpact(collision);
        applyImpact(collision);
        updateMeshData();
    }

    private void applyImpact(Collision collision)
    {
        Vector3 collisionPoint = collision.contacts[0].point;
        int closestIndexes = findClosestVertexIndex(collisionPoint);
        setImpactSize(closestIndexes);
        applyImpactToSize(collision.impulse, closestIndexes);
    }

    private int findClosestVertexIndex(Vector3 point)
    {
        Vector3 terrainOffset = gameObject.transform.position;
        // x,z is the unity plane. find closest vertex in array
        return findClosestXIndex(point - terrainOffset) + (findClosestZIndex(point - terrainOffset) * this.xVerticesCount);
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

    // create a dictionary with all indices that are affected, scaling the outward impact strength down
    private void setImpactSize(int index)
    {
        this.impactStampIndexAndStrength = new Dictionary<int, float>();
        this.impactStampIndexAndStrength[index] = this.impactStrength;

        // for every previous entry we expand it top, right, bottom, left in a cross pattern.
        // duplicates are ignored
        for (int i = 0; i < this.impactSize; i++)
        {
            Dictionary<int, float> temporaryDictionary = new Dictionary<int, float>(this.impactStampIndexAndStrength);
            Dictionary<int, float>.Enumerator enumerator = this.impactStampIndexAndStrength.GetEnumerator();

            while(enumerator.MoveNext())
            {
                KeyValuePair<int, float> currentPair = enumerator.Current;
                addPairsAsCrossIfNotExists(currentPair, temporaryDictionary);
            }

            this.impactStampIndexAndStrength = temporaryDictionary;
        }
    }

    private void applyImpactToSize(Vector3 impulse, int middlePointIndex)
    {
        foreach(KeyValuePair<int, float> kvPair in this.impactStampIndexAndStrength)
        {
            if (isAppropriateIndex(kvPair.Key, middlePointIndex))
            {
                Vector3 impactVector = kvPair.Value * impulse;
                this.terrainVertices[kvPair.Key] = this.terrainVertices[kvPair.Key] - impactVector;
            }
        }
    }

    private void addPairsAsCrossIfNotExists(KeyValuePair<int, float> middlePoint, Dictionary<int, float> dictionary)
    {
        // index to the right
        int otherIndex = middlePoint.Key + 1;
        if (!dictionary.ContainsKey(otherIndex))
        {
            dictionary[otherIndex] = getReducedImpactStrength(middlePoint.Value);
        }
        
        // index to the left
        otherIndex = middlePoint.Key - 1;
        if (!dictionary.ContainsKey(otherIndex))
        {
            dictionary[otherIndex] = getReducedImpactStrength(middlePoint.Value);
        }
        
        // index to the top
        otherIndex = middlePoint.Key - this.xVerticesCount;
        if (!dictionary.ContainsKey(otherIndex))
        {
            dictionary[otherIndex] = getReducedImpactStrength(middlePoint.Value);
        }
        
        // index to the bottom
        otherIndex = middlePoint.Key + this.xVerticesCount;
        if (!dictionary.ContainsKey(otherIndex))
        {
            dictionary[otherIndex] = getReducedImpactStrength(middlePoint.Value);
        }
    }

    private float getReducedImpactStrength(float currentStrength)
    {
        return currentStrength * 0.7f;
    }

    private bool isAppropriateIndex(int index, int middlepointIndex)
    {
        bool isAppropriate = true;

        // check array bound
        if (index > this.terrainVertices.Length -1 || index < 0)
        {
            isAppropriate = false;
        }

        int relativeMiddlePointIndex = middlepointIndex % this.xVerticesCount;
        
        // detect edge oversteps on the left side
        if (index % this.xVerticesCount > relativeMiddlePointIndex + this.impactSize)
        {
            isAppropriate = false;
        }

        // detect edge oversteps on the right side
        if (index % this.xVerticesCount < relativeMiddlePointIndex - this.impactSize)
        {
            isAppropriate = false;
        }

        return isAppropriate;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!ScriptIsActive)
        {
            return;
        }

        if (collision.gameObject.tag.Equals("terrainDoesNotDestroy"))
        {
            print("ignoring bullets until performance fixed");
            collision.gameObject.SetActive(false);
        }
        else
        {
            impact(collision);
            Destroy(collision.gameObject);
        }
    }
}
