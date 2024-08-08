using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class MenuPosition : MonoBehaviour
{
    public void OnVirtualSceneInitialized()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room != null)
        {
            List<MRUKAnchor> anchors = room.Anchors;
            foreach (MRUKAnchor anchor in anchors)
            {
                if (anchor.Label.ToString() == "WALL_ART")
                {
                    Transform wallChild = anchor.transform.GetChild(0);
                    var scale = wallChild.localScale;
                    var position = anchor.transform.position;
                    var rotation = anchor.transform.rotation;

                    // Set transform of the menu
                    transform.position = new Vector3(position.x-1.3f, position.y, position.z+0.15f);
                    transform.rotation *= rotation;
                    Transform transformChild = transform.GetChild(0);
                    transformChild.localScale = scale;
                }
            }
        }
    }

    public void OnPhysicalSceneInitialized()
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

                    // Set transform of the menu
                    transform.position = position;
                    transform.rotation *= rotation;
                }
            }
        }

    }
}
