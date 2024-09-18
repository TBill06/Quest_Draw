using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Meta.XR.MRUtilityKit;

public class MenuPosition : MonoBehaviour
{
    public GameObject leftHandAnchor;
    public GameObject rightHandAnchor;

    // OnVirtualSceneInitialized is used to place the board and menu in virtual and no surface draw scenes.
    // Its called in the AnchorPrefabSpawner script of the virtual and nu surface scenes.
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
                    var position = anchor.transform.position;
                    Vector3 forward = anchor.transform.forward;
                    Vector3 left = -anchor.transform.right;
                    var rotation = anchor.transform.rotation;

                    // Set transform of the menu
                    Vector3 newPosition = position + (forward * 0.15f) + (left * 2f);
                    transform.position = newPosition;
                    transform.rotation *= rotation;
                    break;
                }
            }
        }
    }

    // OnPhysicalSceneInitialized is used to place the board and menu in physical draw scene.
    // Its called in the AnchorPrefabSpawner script of the physical draw scene.
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
                    break;
                }
            }
        }
    }

    // Awake is used to disable hand tracking based on the user's preference in the drawing scenes.
    void Awake()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "Setup")
        {
            int left = PlayerPrefs.GetInt("left");
            if (left == 1)
            {
                rightHandAnchor.SetActive(false);
            }
            else
            {
                leftHandAnchor.SetActive(false);
            }
        }
    }
}
