using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction.Input;

public class CanvasScript : MonoBehaviour
{
    public OVRCameraRig cameraRig; // Assign your OVRCameraRigInteraction in the inspector
    public GameObject canvasObject; // Assign your Canvas GameObject in the inspector
    private RectTransform canvasRect;
    private Vector2 localPointerPosition;
    private Texture2D tex;
    public RawImage rawImage;
    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        canvasRect = canvasObject.GetComponent<RectTransform>();
        tex = new Texture2D((int)canvasRect.rect.width, (int)canvasRect.rect.height);
        // Initialize the texture with a white color
        Color[] fillColor = new Color[tex.width * tex.height];
        for (int i = 0; i < fillColor.Length; ++i)
        {
            fillColor[i] = Color.white;
        }
        tex.SetPixels(fillColor);
        tex.Apply();
        rawImage.texture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        if (hand == null) 
        { 
            Debug.Log("Hand is null"); 
        }
        if (hand.GetIndexFingerIsPinching())
        {
            Camera camera = cameraRig.centerEyeAnchor.GetComponent<Camera>();
            Vector2 screenPosition = camera.WorldToScreenPoint(hand.transform.position);
            bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, camera, out localPointerPosition);

            if (!isInside)
            {
                Debug.Log("Pointer is outside the canvas");
                return;
            }

            int x = Mathf.Clamp((int)localPointerPosition.x, 0, (int)canvasRect.rect.width);
            int y = Mathf.Clamp((int)localPointerPosition.y, 0, (int)canvasRect.rect.height);

            // Add debug logs
            Debug.Log("Viewport position: " + screenPosition);
            Debug.Log("Local pointer position: " + localPointerPosition);
            Debug.Log("x: " + x + ", y: " + y);

            tex.SetPixel(x, y, Color.black); // Change this to the color you want to draw with
            tex.Apply();
        }
    }
}
