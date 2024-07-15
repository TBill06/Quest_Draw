using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3d condition.
// Parameters: Hand, tubeMaterial.
public class HandDrawingTest : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial;
    public float filterFrequency = 120.0f;
    public float minCutoff = 1.0f;
    public float beta = 5f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private ProceduralTube currentTube;
    private MeshRenderer meshRenderer;
    private bool isDrawing = false;

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
        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency,minCutoff,beta,dcutoff);
        
        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        meshRenderer = tubeObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tubeMaterial;

    }

    // Updates the line by adding points to the tube
    void UpdateLine()
    {
        if (isDrawing)
        {
            Pose pose1;
            Pose pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandThumbTip, out pose2);
            Debug.Log("Pose1 & Pose2: "+pose1.position+" "+pose2.position);
            Vector3 filter1 = vector3Filter.Filter(pose1.position);
            Vector3 filter2 = vector3Filter.Filter(pose2.position);
            Debug.Log("Filter1 & Filter2: "+filter1+" "+filter2);
            Vector3 drawpoint = (filter1 + filter2) / 2;
            Debug.Log("Draw point: "+drawpoint);

            currentTube.AddPoint(drawpoint);
        }
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }
}