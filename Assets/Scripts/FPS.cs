using UnityEngine;

public class FPS : MonoBehaviour
{
    float deltaTime = 0.0f;

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
}