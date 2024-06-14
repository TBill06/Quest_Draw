using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction;
using Unity.ProceduralTube;

public class TouchDrawPointer : MonoBehaviour
{
    public Hand hand;
    public LayerMask planeLayer;
    private bool isDrawing = false;
    private bool shouldDraw = false;
    private bool indexPointerPoseDetected = false;
    private bool wasPosing = false;
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
        Pose pose1;
        hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
        Ray ray = new(pose1.position, hand.transform.forward);
        RaycastHit hit;

        if ((indexPointerPoseDetected && !wasPosing))
        {
            shouldDraw = true;
            if (Physics.Raycast(ray, out hit, 0.02f, planeLayer) || pose1.position.z > planePosition.z)
            {
                StartDrawing();
            }
        }
        else if (!indexPointerPoseDetected && wasPosing)
        {
            StopDrawing();
            shouldDraw = false;
        }
        if (isDrawing || shouldDraw)
        {
            UpdateLine();
        }
        wasPosing = indexPointerPoseDetected;
    }

    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
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
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);

            Debug.Log("DrawPoint: "+pose1.position);
            Ray ray = new(pose1.position, hand.transform.forward);
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
            else if (pose1.position.z > planePosition.z)
            {
                Vector3 hitPoint = new Vector3(pose1.position.x, pose1.position.y, planePosition.z-tubeRadius);
                if(!isDrawing)
                {
                    StartDrawing();
                }
                currentTube.AddPoint(hitPoint);
                Debug.Log("Added point from back: "+hitPoint);
            }
            else
            {
                if(isDrawing)
                {
                    StopDrawing();
                }
            }

        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }
}