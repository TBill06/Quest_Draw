using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

public class HandDrawing : MonoBehaviour
{
    // Reference to the Hand component
    public Hand hand;
    public float tubeRadius = 0.007f;
    public int tubeSegments = 64;
    public Material tubeMaterial;
    public float minDistance = 0.007f;

    private bool isDrawing = false;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

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

    void StartDrawing()
    {
        // Set the drawing state to true
        isDrawing = true;

        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        meshRenderer = tubeObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tubeMaterial;

        currentTube.tubeRadius = tubeRadius;
        currentTube.tubeSegments = tubeSegments;
        currentTube.minDistance = minDistance;
    }

    void UpdateLine()
    {
        if (isDrawing)
        {
            Pose pose1;
            Pose pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandThumbTip, out pose2);
            Vector3 drawpoint = (pose1.position + pose2.position) / 2;

            currentTube.AddPoint(drawpoint);
            Debug.Log("Drawing point: " + drawpoint);
            Debug.Log("Tube radius: " + tubeRadius);
            Debug.Log("Min distance: " + minDistance);
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }
}