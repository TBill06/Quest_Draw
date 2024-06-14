using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Oculus.Interaction;

public class PlaneDrawing : MonoBehaviour
{
    public Hand hand;
    public RayInteractor rayInteractor;
    public float tubeRadius = 0.1f;
    public int tubeSegments = 64;
    public Material tubeMaterial;
    public float minDistance = 0.1f;
    public LayerMask planeLayer;

    private bool isDrawing = false;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private List<Vector3> points = new List<Vector3>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;

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
        lineRenderer = tubeObject.AddComponent<LineRenderer>();
        lineRenderer.material = tubeMaterial;
        lineRenderer.startWidth = tubeRadius;
        lineRenderer.endWidth = tubeRadius;
        lineRenderer.positionCount = 0;

        points = new List<Vector3>();
        points.Clear();
    }
    
    void UpdateLine()
    {
        if (isDrawing)
        {
            Vector3 cursorPosition = rayInteractor.CollisionInfo.Value.Point;
            cursorPosition.z = 9.9f;

            Debug.Log("cursorPosition: " + cursorPosition);
            points.Add(cursorPosition);

            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }
}
