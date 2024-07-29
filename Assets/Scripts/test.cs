using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Meta.XR.MRUtilityKit;
using Unity.ProceduralTube;
using System.IO;
using System.Text;
using UnityEngine.Serialization;
public class Test : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial;
    public Material lineMaterial;
    // public GameObject cubeIns;
    private bool sceneInitialized = false;
    private MRUKAnchor wall;
    private MRUKAnchor wallArt;
    private LineRenderer lineRenderer;
    private LineRenderer lineRenderer2;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private LineRenderer lineRendererXWallArt;
    private LineRenderer lineRendererYWallArt;
    private LineRenderer lineRendererZWallArt;
    private LineRenderer lineRendererXWall;
    private LineRenderer lineRendererYWall;
    private LineRenderer lineRendererZWall;

    void Start()
    {
        // Initialize LineRenderers for wallArt
        lineRendererXWallArt = CreateLineRenderer("LineRendererXWallArt", Color.red);
        lineRendererYWallArt = CreateLineRenderer("LineRendererYWallArt", Color.green);
        lineRendererZWallArt = CreateLineRenderer("LineRendererZWallArt", Color.blue);

        // Initialize LineRenderers for wall
        lineRendererXWall = CreateLineRenderer("LineRendererXWall", Color.red);
        lineRendererYWall = CreateLineRenderer("LineRendererYWall", Color.green);
        lineRendererZWall = CreateLineRenderer("LineRendererZWall", Color.blue);
    }

    public void OnSceneInitialized()
    {
        Debug.Log("Scene is ready!");
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room != null)
        {
            Debug.Log("Current room is " + room.name);
            List<MRUKAnchor> anchors = room.Anchors;
            foreach (MRUKAnchor anchor in anchors)
            {
                if (anchor.Label.ToString() == "WALL_ART")
                {
                    wallArt = anchor;
                    // // wallArt.ParentAnchor = wall;
                    // // Debug.Log("Wall Transform: " + wall.transform.position+" "+wall.transform.rotation.eulerAngles+" "+wall.transform.localScale);
                    // Debug.Log("Wall Anchor PlaneRect: "+wall.PlaneRect.Value+" VolumeBounds: "+wall.VolumeBounds.Value+" PlaneBoundary2D: "+wall.PlaneBoundary2D);
                    Debug.Log("WallArt Transform: " + wallArt.transform.position+" "+wallArt.transform.rotation.eulerAngles+" "+wallArt.transform.localScale);
                    Debug.Log("WallArt Anchor PlaneBoundary2D: "+wallArt.PlaneBoundary2D); 

                    UpdateLineRenderer(lineRendererXWallArt, wallArt.transform.position, wallArt.transform.position + wallArt.transform.right);
                    UpdateLineRenderer(lineRendererYWallArt, wallArt.transform.position, wallArt.transform.position + wallArt.transform.up);
                    UpdateLineRenderer(lineRendererZWallArt, wallArt.transform.position, wallArt.transform.position + wallArt.transform.forward);

                    UpdateLineRenderer(lineRendererXWall, wall.transform.position, wall.transform.position + wall.transform.right);
                    UpdateLineRenderer(lineRendererYWall, wall.transform.position, wall.transform.position + wall.transform.up);
                    UpdateLineRenderer(lineRendererZWall, wall.transform.position, wall.transform.position + wall.transform.forward);
                }
            }

            sceneInitialized = true;

        }

        GameObject c2 = new GameObject("LineRenderer");
        lineRenderer = c2.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = 2;
    }

    private LineRenderer CreateLineRenderer(string name, Color color)
    {
        GameObject lineObject = new GameObject(name);
        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.005f;
        lr.endWidth = 0.005f;
        lr.material = lineMaterial;
        lr.positionCount = 2;
        lr.startColor = color;
        lr.endColor = color;
        return lr;
    }

    private void UpdateLineRenderer(LineRenderer lr, Vector3 start, Vector3 end)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    void Update()
    {
        if (!sceneInitialized)
        {
            return;
        }
        if (hand.GetIndexFingerIsPinching())
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
            RaycastHit hit;
            if (wallArt.Raycast(ray,0.1f,out hit))
            {
                lineRenderer.SetPosition(0, pinchPoint);
                lineRenderer.SetPosition(1, hit.point);
                Debug.Log("Hit point: "+hit.point);
            }
            // if (wall.Raycast(ray, 0.1f, out hit))
            // {
            //     lineRenderer.SetPosition(0, pinchPoint);
            //     lineRenderer.SetPosition(1, hit.point);
            //     Debug.Log("Hit point: "+hit.point);
            //     if(!isDrawing)
            //     {
            //         StartDrawing();
            //     }
            //     else if(isDrawing)
            //     {
            //         UpdateLine(pinchPoint, hit);
            //     }
            // }
            // else
            // {
            //     StopDrawing();
            // }
        }
        // else
        // {
        //     StopDrawing();
        // }
    }
    
    // Initializes a new tube object
    // void StartDrawing()
    // {
    //     // isDrawing = true;

    //     GameObject tubeObject = new GameObject("Tube");
    //     currentTube = tubeObject.AddComponent<ProceduralTube>();
    //     meshRenderer = tubeObject.AddComponent<MeshRenderer>();
    //     meshRenderer.material = tubeMaterial;
    // }

    //  // Updates the line by adding points to the tube
    // void UpdateLine(Vector3 point, RaycastHit hit)
    // {
    //     Vector3 hitPoint;
    //     hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z+0.009f);
    //     Debug.Log("Added point from the front: "+hitPoint);
    //     currentTube.AddPoint(hitPoint);
    // }

    // // Stops drawing
    // void StopDrawing()
    // {
    //     // isDrawing = false;
    // }
}