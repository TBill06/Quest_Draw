using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class MenuPosition : MonoBehaviour
{
    public void OnSceneInitialized()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room != null)
        {
            List<MRUKAnchor> anchors = room.Anchors;
            foreach (MRUKAnchor anchor in anchors)
            {
                if (anchor.Label.ToString() == "WALL_ART")
                {
                    var position = anchor.transform.position;
                    var rotation = anchor.transform.rotation;

                    // Set the position and rotation of the menu
                    transform.position = new Vector3(position.x+1.0f, position.y, position.z);
                    transform.rotation = rotation;
                    Debug.Log("Menu position set to " + position+ " and rotation set to " + rotation);
                }
            }
        }
    }
}
