using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class RoomPrefabGenerator : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Generating room prefab");
        string json = File.ReadAllText(Application.persistentDataPath + "/room_final.json");
        GameObjectData data = JsonUtility.FromJson<GameObjectData>(json);
        CreateGameObject(data, null);
    }

    void CreateGameObject(GameObjectData data, GameObject parent)
    {
        GameObject obj = new GameObject(data.name);
        obj.transform.position = data.position;
        obj.transform.rotation = data.rotation;
        obj.transform.localScale = data.scale;

        if (parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        foreach (GameObjectData childData in data.children)
        {
            CreateGameObject(childData, obj);
        }
        if (obj.name == "Mesh")
        {
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        }
    }

    
}