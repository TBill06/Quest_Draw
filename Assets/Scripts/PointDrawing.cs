using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user points with their index finger.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3d condition.
// Open the palm (more specifically the thumb and middle finger) to stop pointing and hence stop drawing.
// Parameters: Hand, tubeMaterial.
public class PointDrawing : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial;

    private bool indexPointerPoseDetected = false;
    private bool isDrawing = false;
    private ProceduralTube currentTube;

    void Update()
    {
        if (indexPointerPoseDetected)
        {
            if (!isDrawing)
            {
                StartDrawing();
            }
            UpdateLine();
        }
        else
        {
            StopDrawing();
        }
    }

    // Public method to set the index finger pose detected
    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
    }

    // Initializes a new tube object
    void StartDrawing()
    {
        isDrawing = true;

        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        currentTube.material = tubeMaterial;
    }

    // Updates the line by adding points to the tube
    void UpdateLine()
    {
        if (isDrawing)
        {
            // Get the position of the index finger tip
            Pose pose1;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);

            // Add the point to the tube
            currentTube.AddPoint(pose1.position);
        }
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }
}