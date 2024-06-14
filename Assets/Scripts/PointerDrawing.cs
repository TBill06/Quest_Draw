using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

public class PointerDrawing : MonoBehaviour
{
    public Hand hand;
    public float tubeRadius = 0.007f;
    public int tubeSegments = 64;
    public Material tubeMaterial;
    public float minDistance = 0.007f;

    private bool indexPointerPoseDetected = false;
    private bool isDrawing = false;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

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

    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
    }

    void StartDrawing()
    {
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
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            currentTube.AddPoint(pose1.position);
            Debug.Log("Drawing point: " + pose1.position);
            Debug.Log("Tube radius: " + tubeRadius);
            Debug.Log("Min distance: " + minDistance);
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }
}