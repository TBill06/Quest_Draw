using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public List<GameObjectData> children = new List<GameObjectData>();
}

public class RoomDataSerializer : MonoBehaviour
{
    public GameObject room;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving room data");
            GameObjectData data = GetGameObjectData(room);

            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            File.WriteAllText(Application.persistentDataPath + "/room_final.json", json);
            Debug.Log("Room data saved in " + Application.persistentDataPath + "/room_final.json");
        }
    }

    GameObjectData GetGameObjectData(GameObject obj)
    {
        GameObjectData data = new GameObjectData
        {
            name = obj.name,
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            scale = obj.transform.localScale
        };
        foreach (Transform child in obj.transform)
        {
            if (child.name == "Volume(Clone)" || child.name == "PlaneMesh(Clone)")
            {
                GameObjectData childData = GetGameObjectData(child.gameObject);
                data.children.Add(childData);
                foreach (Transform grandchild in child)
                {
                    if (grandchild.name == "Parent")
                    {
                        childData.children.Add(GetGameObjectData(grandchild.gameObject));
                        foreach (Transform greatGrandchild in grandchild)
                        {
                            if (greatGrandchild.name == "Mesh")
                            {
                                childData.children[childData.children.Count - 1].children.Add(GetGameObjectData(greatGrandchild.gameObject));
                            }
                        }
                    }
                }
            }
        }
        return data;
    }
}
