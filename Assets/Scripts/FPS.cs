using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;

public class FPS : MonoBehaviour
{
    float deltaTime = 0.0f;
    float logInterval = 10.0f;

    void Start()
    {
        StartCoroutine(LogHeadsetPositionPeriodically());
    }

    IEnumerator LogHeadsetPositionPeriodically()
    {
        while (true)
        {
            GameObject headset = GameObject.Find("CenterEyeAnchor");
            if (headset != null)
            {
                Debug.Log("Headset position: " + headset.transform.position + " Rotation: " + headset.transform.rotation.eulerAngles + " Scale: " + headset.transform.localScale);
            }
            yield return new WaitForSeconds(logInterval);
        }
    }

    void Update()
    {
        // Calculate the time it took to complete the last frame and update deltaTime
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        // Calculate FPS
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // Format the string to display FPS
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        // Set style and position for the text
        GUIStyle style = new GUIStyle();
        int w = Screen.width, h = Screen.height;
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;

        // Draw the text on the screen
        GUI.Label(rect, text, style);
    }

    public void OnSceneInitialized()
    {
        Debug.Log("Scene is ready!");
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room != null)
        {
            Debug.Log("Current room is " + room.name + " Position: " + room.transform.position + " Rotation: " + room.transform.rotation.eulerAngles + " Scale: " + room.transform.localScale);
            List<MRUKAnchor> anchors = room.Anchors;
            foreach (MRUKAnchor anchor in anchors)
            {
                if (anchor.Label.ToString() == "WALL_ART")
                {
                    Debug.Log("Wall Anchor: " + anchor.Label + " Position: " + anchor.transform.position + " Rotation: " + anchor.transform.rotation.eulerAngles + " Scale: " + anchor.transform.localScale);
                }
                if (anchor.Label.ToString() == "FLOOR")
                {
                    Debug.Log("Floor Anchor: " + anchor.Label + " Position: " + anchor.transform.position + " Rotation: " + anchor.transform.rotation.eulerAngles + " Scale: " + anchor.transform.localScale);
                }
            }
        }
    }
}