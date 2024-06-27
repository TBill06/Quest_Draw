using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Surfaces;
using Unity.ProceduralTube;

// This script draws on a virtual quad when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// Don't draw with both controllers at the same time.
// Parameters: tubeMaterial.
public class VSurfaceController : MonoBehaviour
{
    public Material leftTubeMaterial;
    public Material rightTubeMaterial;
    private bool isDrawing = false;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private GameObject quad;
    private ColliderSurface colliderSurface;
    private Vector3 colliderSurfacePosition;
    private Vector3 minBoundary;
    private Vector3 maxBoundary;
    // private LineRenderer lineRenderer;

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
    }

    void Update()
    {
        OVRInput.Controller activeController = OVRInput.Controller.None;
        Material material = null;
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            activeController = OVRInput.Controller.RTouch;
            material = rightTubeMaterial;
        }
        else if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
        {
            activeController = OVRInput.Controller.LTouch;
            material = leftTubeMaterial;
        }
        if (activeController != OVRInput.Controller.None)
        {
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(activeController);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(activeController);
            Vector3 downDirection = controllerRotation * Quaternion.Euler(40,0,0) * new Vector3(-0.01f, -1, 0);
            Vector3 controllerTipPosition = controllerPosition + downDirection * 0.095f;

            Ray ray = new Ray(controllerTipPosition, downDirection);
            SurfaceHit hit;
            if(colliderSurface.Raycast(ray, out hit, 0.02f) || controllerTipPosition.z < colliderSurfacePosition.z)
            {
                if(IsWithinBounds(controllerTipPosition, colliderSurface.Bounds.min, colliderSurface.Bounds.max))
                {
                    if(!isDrawing)
                    {
                        StartDrawing(material);
                    }
                    else if(isDrawing)
                    {
                        UpdateLine(controllerTipPosition, hit);
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
    void StartDrawing(Material material)
    {
        isDrawing = true;

        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        meshRenderer = tubeObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
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
}
