using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Surfaces;
using Unity.ProceduralTube;

/** Deprecated, Use V2 **/

// This script draws on a virtual quad when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// If pinching before the virtual surface, raycasting willbe used, if piching through, offset will be used.
// Parameters: Hand, tubeMaterial.
public class VSurfacePinch : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial;
    private bool isDrawing = false;
    private ProceduralTube currentTube;
    private GameObject quad;
    // private LineRenderer lineRenderer;
    // private LineRenderer lineRenderer2;
    // private LineRenderer lineRenderer3;
    // public Material quadMaterial;
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

        // GameObject c1 = new GameObject("LineRenderer");
        // lineRenderer = c1.AddComponent<LineRenderer>();
        // lineRenderer.startWidth = 0.01f;
        // lineRenderer.endWidth = 0.01f;
        // lineRenderer.material = tubeMaterial;
        // lineRenderer.positionCount = 2;

        // GameObject c2 = new GameObject("LineRenderer2");
        // lineRenderer2 = c2.AddComponent<LineRenderer>();
        // lineRenderer2.startWidth = 0.01f;
        // lineRenderer2.endWidth = 0.01f;
        // lineRenderer2.material = quadMaterial;
        // lineRenderer2.positionCount = 2;

        // GameObject c3 = new GameObject("LineRenderer3");
        // lineRenderer3 = c3.AddComponent<LineRenderer>();
        // lineRenderer3.startWidth = 0.01f;
        // lineRenderer3.endWidth = 0.01f;
        // lineRenderer3.positionCount = 2;
    }

    void Update()
    {
        if(hand.GetIndexFingerIsPinching())
        {
            Pose pose1, pose2, thumbPose, thumbBasePose;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandIndex1, out pose2);
            hand.GetJointPose(HandJointId.HandThumbTip, out thumbPose);
            hand.GetJointPose(HandJointId.HandThumb1, out thumbBasePose);
            Vector3 indexDirection = (pose1.position - pose2.position).normalized;
            Vector3 thumbDirection = (thumbPose.position - thumbBasePose.position).normalized;
            Vector3 pinchPoint = (pose1.position + thumbPose.position) / 2;
            Vector3 pinchDirection = (indexDirection + thumbDirection) / 2;
            
            Ray ray = new Ray(pinchPoint, pinchDirection);
            SurfaceHit hit;
            if(colliderSurface.Raycast(ray, out hit, 0.02f) || pinchPoint.z < colliderSurfacePosition.z)
            {
                if(IsWithinBounds(pinchPoint, colliderSurface.Bounds.min, colliderSurface.Bounds.max))
                {
                    if(!isDrawing)
                    {
                        StartDrawing();
                    }
                    else if(isDrawing)
                    {
                        UpdateLine(pinchPoint, hit);
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

        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        currentTube.material = tubeMaterial;
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

    // Checks if a point is within the bounds of a surface (not checking z-axis)
    bool IsWithinBounds(Vector3 point, Vector3 minBoundary, Vector3 maxBoundary)
    {
        bool withinBounds = point.x >= minBoundary.x && point.x <= maxBoundary.x &&
            point.y >= minBoundary.y && point.y <= maxBoundary.y;
        return withinBounds;
    }

}
