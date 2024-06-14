using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction;
using Unity.ProceduralTube;

public class TouchDrawRayCasting : MonoBehaviour
{
    public Hand hand;
    public LayerMask planeLayer;
    private bool isDrawing = false;
    private bool shouldDraw = false;
    private bool wasPinching = false;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private List<Vector3> points = new List<Vector3>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public float tubeRadius = 0.007f;
    public int tubeSegments = 64;
    public Material tubeMaterial;
    public float minDistance = 0.007f;
    private Vector3 planePosition;
    private float planeWidth;
    private float planeHeight;

    void Start()
    {
        planePosition = this.transform.position;
        planeWidth = this.transform.localScale.x;
        planeHeight = this.transform.localScale.y;
    }
    
    void Update()
    {
        bool isPinching = hand.GetIndexFingerIsPinching();
        Pose pose1;
        Pose pose2;
        hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
        hand.GetJointPose(HandJointId.HandThumbTip, out pose2);
        Vector3 drawpoint = (pose1.position + pose2.position) / 2;
        Ray ray = new(drawpoint, hand.transform.forward);
        RaycastHit hit;

        if ((isPinching && !wasPinching))
        {
            shouldDraw = true;
            if (Physics.Raycast(ray, out hit, 0.02f, planeLayer) || drawpoint.z > planePosition.z)
            {
                StartDrawing();
            }
        }
        else if (!isPinching && wasPinching)
        {
            StopDrawing();
            shouldDraw = false;
        }
        if (isDrawing || shouldDraw)
        {
            UpdateLine();
        }
        wasPinching = isPinching;
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
        points = new List<Vector3>();
    }
    
    void UpdateLine()
    {
        if (shouldDraw)
        {
            Pose pose1;
            Pose pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandThumbTip, out pose2);
            Vector3 drawpoint = (pose1.position + pose2.position) / 2;

            Debug.Log("DrawPoint: "+drawpoint);
            Ray ray = new(drawpoint, hand.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 0.02f, planeLayer))
            {
                if(!isDrawing)
                {
                    StartDrawing();
                }
                points.Add(hit.point);
                Debug.Log("Points count: "+points.Count);
                Vector3 hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z-tubeRadius);
                currentTube.AddPoint(hitPoint);
                Debug.Log("Raycast hit point: "+hit.point);
                Debug.Log("Raycast hit distance: "+hit.distance);
                Debug.Log("Raycast hit collider: "+hit.collider.name);
            }
            else if (drawpoint.z > planePosition.z)
            {
                Vector3 hitPoint = new Vector3(drawpoint.x, drawpoint.y, planePosition.z-tubeRadius);
                if(!isDrawing)
                {
                    StartDrawing();
                }
                currentTube.AddPoint(hitPoint);
                Debug.Log("Added point from back: "+hitPoint);
            }
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }
}
