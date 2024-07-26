using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3D condition.
// Parameters: Hand, tubeMaterial.
public class PinchDrawing : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial;

    private bool isDrawing = false;
    private ProceduralTube currentTube;

    void Update()
    {
        if (hand.GetIndexFingerIsPinching())
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
            // Get the hand positions
            Pose pose1;
            Pose pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandThumbTip, out pose2);
            Vector3 drawpoint = (pose1.position + pose2.position) / 2;

            // Add the point to the tube
            currentTube.AddPoint(drawpoint);
        }
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }
}