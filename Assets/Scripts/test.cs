using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Meta.XR.MRUtilityKit;
using Unity.ProceduralTube;
using System.IO;
using System.Text;
using UnityEngine.Serialization;
public class Test : MonoBehaviour
{
    public bool ready = false;
    public AnchorPrefabSpawner anchorPrefabSpawner;
    private IEnumerator WaitForAnchorPrefabSpawner()
    {
        while (anchorPrefabSpawner == null || anchorPrefabSpawner.AnchorPrefabSpawnerObjects.Count == 0) 
        {
            yield return null;
        }
        SetPrefabs();
    }
    void Start()
    {
        // StartCoroutine(WaitForAnchorPrefabSpawner());
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        Debug.Log("MRUK room: " + room);
        if(ready)
        {
            string json = SaveScene(SerializationHelpers.CoordinateSystem.Unity);
            File.WriteAllText(Application.persistentDataPath + "/room_final.json", json);
            Debug.Log("Room data saved in " + Application.persistentDataPath + "/room_final.json");
            Debug.Log("Room data: " + json);
        }
        else
        {
            Debug.Log("Not ready");
        }
    }

    private void SetPrefabs()
    {
        var allSpawnerObjects = anchorPrefabSpawner.AnchorPrefabSpawnerObjects;
        foreach (var spawnerObject in allSpawnerObjects)
        {
            Debug.Log(spawnerObject.Key + " " + spawnerObject.Value);
        }
    }

    public void SetReady (bool ready)
    {
        ready = true;
    }

    public string SaveScene(SerializationHelpers.CoordinateSystem coordinateSystem)
    {
        return SerializationHelpers.Serialize(coordinateSystem, false);
    }
        
}