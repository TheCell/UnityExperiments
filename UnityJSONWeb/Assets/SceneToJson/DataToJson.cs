using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//using UnityEditor;

public class DataToJson : MonoBehaviour
{
    [SerializeField] string jsonFilePathAndFile;
    float lastWriteTime = 0f;

    private void Start()
    {
    }

    private void Update()
    {
        if (lastWriteTime + 3 < Time.realtimeSinceStartup)
        {
            WriteJson();
            lastWriteTime = Time.realtimeSinceStartup;
        }
    }

    private void WriteJson()
    {
        //EditorJsonUtility.ToJson()
        string jsonRepresentation = "";
        VertexData randomData = GetRandomVertexData();
        jsonRepresentation = JsonUtility.ToJson(randomData);
        //Debug.Log(jsonRepresentation);
        WriteString(jsonRepresentation);
        
    }

    private VertexData GetRandomVertexData(int seed = -1)
    {
        VertexData vertexData = new VertexData();
        System.Random rnd;
        if (seed < 0)
        {
            seed = UnityEngine.Random.Range(0, 1337);
        }

        rnd = new System.Random(seed);
        vertexData.seed = seed;
        vertexData.time = Time.realtimeSinceStartup;
        vertexData.objectName = "gameobject " + rnd.Next();
        vertexData.vertexPoints = rnd.Next(1000);
        vertexData.basicPosition = new Vector3((float) rnd.NextDouble(), (float) rnd.NextDouble(), (float) rnd.NextDouble());

        vertexData.vertxindices = new int[vertexData.vertexPoints];
        vertexData.temperatures = new float[vertexData.vertexPoints];
        vertexData.vertexPositions = new Vector3[vertexData.vertexPoints];
        for (int i = 0; i < vertexData.vertexPoints; i++)
        {
            vertexData.vertxindices[i] = rnd.Next();
            vertexData.temperatures[i] = (float) rnd.NextDouble();
            vertexData.vertexPositions[i] = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }
        
        return vertexData;
    }

    private void WriteString(string data)
    {
        if (jsonFilePathAndFile.Contains(".json"))
        {
            StreamWriter writer = new StreamWriter(jsonFilePathAndFile, false);
            writer.Write(data);
            writer.Close();
            //Debug.Log("file written");
        }
        else
        {
            Debug.LogError("no Filepath with file");
        }
    }
}

public class VertexData
{
    public int seed;
    public float time;
    public string objectName;
    public int vertexPoints;
    public Vector3 basicPosition;
    public int[] vertxindices;
    public float[] temperatures;
    public Vector3[] vertexPositions;
}
