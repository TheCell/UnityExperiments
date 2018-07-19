using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour
{
    public static GameControl control;

    private void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 30), "ein Label" + Path.DirectorySeparatorChar + "playerInfo.dat");
        if(GUI.Button(new Rect(10, 40, 550, 30), "Save Object to " + Application.persistentDataPath))
        {
            save();
        }
        if(GUI.Button(new Rect(10, 80, 550, 30), "Load Object to " + Application.persistentDataPath))
        {
            load();
        }
    }

    public void save()
    {
        print("File saving started");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat");

        PlayerData data = new PlayerData();
        data.objectMesh = gameObject.GetComponent<Mesh>();

        bf.Serialize(file, data);
        file.Close();

        print("File saved to " + Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat");
    }

    public void load()
    {
        print("File loading started");

        if(File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData) bf.Deserialize(file);
            file.Close();

            Mesh gameobjMesh = gameObject.GetComponent<Mesh>();
            gameobjMesh = data.objectMesh;

            print("File loaded from " + Application.persistentDataPath + Path.DirectorySeparatorChar + "playerInfo.dat");
        }
    }
}

[Serializable]
class PlayerData
{
    public float name;
    public Mesh objectMesh;
}