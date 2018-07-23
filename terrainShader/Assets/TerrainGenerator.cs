﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int tileCountX = 200;
    public int tileCountZ = 200;
    public float width = 100.0f;
    public float length = 100.0f;
    public float height = 70.0f;
    public float worldHighlightScale = 0.03f;
    public int worldSeed;

    private bool displayDebug = false;
    private System.Random worldSeedGenerator = new System.Random();
    private MeshCollider objectMeshCollider;
    private Vector3[] meshvertices;
    private Vector2[] uv;
    private int verticesForX;
    private int verticesForY;

    // Use this for initialization
    void Start ()
    {
        if (displayDebug)
        {
            print("worldSeed " + worldSeed);
            print("tileCountX " + tileCountX + " tileCountY " + tileCountZ);
        }
        
        // init Worldseed and randomgen
        if (worldSeed == 0)
        {
            worldSeed = Random.Range(1, int.MaxValue);
        }
        worldSeedGenerator = new System.Random(worldSeed);
        
        initializeTerrain();
    }

    private int[] trianglesFromTiles(int tileCountX, int tileCountZ, bool showDebugLogs = false)
    {
        // every tile is built by 2 triangles
        int triangleAmount = tileCountX * tileCountZ * 2;
        if (showDebugLogs)
        {
            print("triangleAmount " + triangleAmount);
        }
        int vertexPointsX = tileCountX + 1;
        int[] trianglePoints = new int[triangleAmount * 3];
        int trianglePointIndex = 0;
        int xCounter = 0;
        int yCounter = 0;

        for (int i = 0; i < triangleAmount; i++)
        {
            if (showDebugLogs)
            {
                print("trianglePointIndex " + trianglePointIndex);
            }

            if (i % 2 == 0)
            {
                trianglePoints[trianglePointIndex] = xCounter + yCounter * vertexPointsX; // x0y0
                trianglePoints[trianglePointIndex + 1] = xCounter + (yCounter + 1) * vertexPointsX; // x0y1
                trianglePoints[trianglePointIndex + 2] = (xCounter + 1) + yCounter * vertexPointsX; // x1y0
                if (showDebugLogs)
                {
                    print("triangleTop " + (xCounter + yCounter * vertexPointsX)
                        + " " + (xCounter + (yCounter + 1) * vertexPointsX)
                        + " " + ((xCounter + 1) + yCounter * vertexPointsX));
                }
            }
            else
            {
                trianglePoints[trianglePointIndex] = (xCounter + 1) + yCounter * vertexPointsX; // x1y0
                trianglePoints[trianglePointIndex + 1] = xCounter + (yCounter + 1) * vertexPointsX; // x0y1
                trianglePoints[trianglePointIndex + 2] = (xCounter + 1) + (yCounter + 1) * vertexPointsX; // x1y1
                if (showDebugLogs)
                {
                    print("triangleBottom " + ((xCounter + 1) + yCounter * vertexPointsX)
                        + " " + (xCounter + (yCounter + 1) * vertexPointsX)
                        + " " + ((xCounter + 1) + (yCounter + 1) * vertexPointsX));
                }

                xCounter++;
                if (xCounter + 1 >= vertexPointsX)
                {
                    yCounter++;
                    xCounter = 0;
                }
            }

            trianglePointIndex = trianglePointIndex + 3;
        }

        return trianglePoints;
    }

    Vector3[] getMeshNormals(int vertexAmount)
    {
        Vector3[] normals = new Vector3[vertexAmount];
        for (int i = 0; i < vertexAmount; i++)
        {
            normals[i] = -Vector3.forward;
        }
        return normals;
    }

    private void initializeTerrain()
    {
        // prepare Object for the terrain Mesh
        MeshFilter meshFilter = null;
        Mesh mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // create a meshCollider and save for later use
        setupMeshCollider();

        // generate the vertices and uv maps
        this.verticesForX = (tileCountX + 1);
        this.verticesForY = (tileCountZ + 1);
        int vertexAmount = verticesForX * verticesForY;
        meshvertices = new Vector3[vertexAmount];
        uv = new Vector2[vertexAmount];
        int xVertexNumber = 0;
        int zVertexNumber = 0;
        int perlinStartValueX = worldSeedGenerator.Next(0, 10000);
        int perlinStartValueY = worldSeedGenerator.Next(0, 10000);

        for (int i = 0; i < vertexAmount; i++)
        {
            float xPosition = xVertexNumber * (width / (tileCountX + 1));
            float zPosition = zVertexNumber * (length / (tileCountZ + 1));
            float xValuePerlin = perlinStartValueX + xVertexNumber * worldHighlightScale;
            float yValuePerlin = perlinStartValueY - zVertexNumber * worldHighlightScale;
            float yPosition = Mathf.PerlinNoise(xValuePerlin, yValuePerlin) * height;

            meshvertices[i] = new Vector3(xPosition, yPosition, zPosition);
            uv[i] = new Vector2(xPosition, zPosition);
            if (displayDebug)
            {
                GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugSphere.name = "debugSphere";
                debugSphere.GetComponent<Transform>().position =
                    new Vector3(
                        transform.position.x + xPosition,
                        transform.position.y + 0,
                        transform.position.z + zPosition);
                debugSphere.GetComponent<Transform>().localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Material debugMaterial = new Material(Shader.Find("Standard"));
                debugMaterial.color = new Color(1.0f, 0.7f, 0.0f);
                debugSphere.GetComponent<MeshRenderer>().material = debugMaterial;
            }

            xVertexNumber++;
            if (xVertexNumber > tileCountX)
            {
                xVertexNumber = 0;
                zVertexNumber++;
            }
        }

        mesh.vertices = meshvertices;
        mesh.uv = uv;

        // setup the triangles from the vertices
        mesh.triangles = trianglesFromTiles(tileCountX, tileCountZ, displayDebug);
        mesh.normals = getMeshNormals(verticesForX * verticesForY);

        // update collision Mesh
        this.objectMeshCollider.sharedMesh = mesh;
    }

    private void setupMeshCollider()
    {
        this.objectMeshCollider = GetComponent<MeshCollider>();
        if (this.objectMeshCollider == null)
        {
            gameObject.AddComponent<MeshCollider>();
            this.objectMeshCollider = gameObject.GetComponent<MeshCollider>();
        }
    }

    /*
    // returns an array with amountOfPoints*amountOfPoints values range from 0.0f to 1.0f
    float[,] getRandomWorldPoints(int amountOfPoints)
    {
        float[,] xyPoints = new float[amountOfPoints, amountOfPoints];
        for(int x = 0, y = 0; x < amountOfPoints; x++)
        {
            if (x >= amountOfPoints)
            {
                x = 0;
                y++;
            }
            xyPoints[x,y] = (1.0f * worldSeedGenerator.Next(1, int.MaxValue)) / int.MaxValue;
        }

        return xyPoints;
    }
    */

    /*
    // maps continous values to a linear discrete scale
    int valueMap(int discreteFrom, int discreteTo, 
        float continuousFrom, float continuousTo, float continuousCurrentValue)
    {
        int mappedValue = -1;
        float continousStepSize = (continuousTo - continuousFrom) / (discreteTo - discreteFrom);
        float continousStepSizeHalfed = continousStepSize / 2;
        int discreteStepSize = 1; // linear scale

        int i = 0;
        do
        {
            if (continuousCurrentValue < (continuousFrom + continousStepSize * i + continousStepSizeHalfed))
            {
                print("continuousCurrentValue < (continuousFrom + continousStepSize * i + continousStepSizeHalfed) "
                    + continuousCurrentValue + " < " + continuousFrom + " " + continousStepSize + " * " + i + " " + continousStepSizeHalfed
                    + " " + "mappedValue: " + discreteFrom + i * discreteStepSize);
                mappedValue = discreteFrom + i * discreteStepSize;
                i = (discreteTo - discreteFrom); // break do while
            }
            i++;
        }
        while (i < (discreteTo - discreteFrom));
        
        return mappedValue;
    }
    */
}
