using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Surfaces;
using Unity.ProceduralTube;

// This script draws on a virtual quad when user points with their index finger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// If pinching before the virtual surface, raycasting will be used, if piching through, offset will be used.
// Open the palm (more specifically the thumb and middle finger) to stop pointing and hence stop drawing.
// Parameters: Hand, tubeMaterial.
public class VSurfacePoint : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial;
    private bool drawnByPoint = false;
    private bool isDrawing = false;
    private bool indexPointerPoseDetected = false;
    private ProceduralTube currentTube;
    private MeshRenderer meshRenderer;
    private GameObject quad;
    private ColliderSurface colliderSurface;
    private Vector3 colliderSurfacePosition;
    private Vector3 minBoundary;
    private Vector3 maxBoundary;

    void Start()
    {
        quad = GameObject.Find("Quad");
        colliderSurface = quad.GetComponent<ColliderSurface>();
        minBoundary = colliderSurface.Bounds.min;
        maxBoundary = colliderSurface.Bounds.max;
        colliderSurfacePosition = colliderSurface.Transform.position;
    }

    void Update()
    {
        if(indexPointerPoseDetected)
        {
            Pose pose1, pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandIndex1, out pose2);
            Vector3 indexDirection = (pose1.position - pose2.position).normalized;
            
            Ray ray = new Ray(pose1.position, indexDirection);
            SurfaceHit hit;
            if(colliderSurface.Raycast(ray, out hit, 0.02f) || pose1.position.z < colliderSurfacePosition.z)
            {
                if(IsWithinBounds(pose1.position, colliderSurface.Bounds.min, colliderSurface.Bounds.max))
                {
                    if(!isDrawing)
                    {
                        StartDrawing();
                    }
                    else if(isDrawing)
                    {
                        UpdateLine(pose1.position, hit);
                    }
                }
                else
                {
                    StopDrawing();
                }
            }
            else
            {
                StopDrawing();
            }
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
        drawnByPoint = true;

        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        meshRenderer = tubeObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tubeMaterial;
    }

    // Updates the line by adding points to the tube
    void UpdateLine(Vector3 point, SurfaceHit hit)
    {
        Vector3 hitPoint;
        if(point.z < colliderSurfacePosition.z)
        {
            hitPoint = new Vector3(point.x, point.y, colliderSurfacePosition.z+0.009f);
            Debug.Log("Added point from the back: "+hitPoint);
        }
        else
        {
            hitPoint = new Vector3(hit.Point.x, hit.Point.y, colliderSurfacePosition.z+0.009f);
            Debug.Log("Added point from the front: "+hitPoint);
        }
        currentTube.AddPoint(hitPoint);
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }

    // Method to check if a point is within the bounds of a surface (not checking z-axis)
    bool IsWithinBounds(Vector3 point, Vector3 minBoundary, Vector3 maxBoundary)
    {
        bool withinBounds = point.x >= minBoundary.x && point.x <= maxBoundary.x &&
            point.y >= minBoundary.y && point.y <= maxBoundary.y;
        return withinBounds;
    }

    // Public method to set the index finger pose detected
    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
    }

    // Public method to get the drawn by point status
    public bool DrawnByPoint
    {
        get { return drawnByPoint; }
    }

}
