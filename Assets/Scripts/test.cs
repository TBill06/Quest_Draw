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
    public Material lineMaterial;
    private bool sceneInitialized = false;
    private MRUKAnchor wall;
    private MRUKAnchor wallArt;
    public GameObject wallArtObject;
    private LineRenderer lineRenderer;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRendererXWallArt;
    private LineRenderer lineRendererYWallArt;
    private LineRenderer lineRendererZWallArt;
    private LineRenderer lineRendererXWall;
    private LineRenderer lineRendererYWall;
    private LineRenderer lineRendererZWall;

    void Awake()
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
                    wallArt.ParentAnchor = wall;
                    if (wall != null)
                    {
                        Debug.Log("Wall Anchor PlaneBoundary2D: "+wall.PlaneBoundary2D);
                        Debug.Log("Wall Transform: " + wall.transform.position+" "+wall.transform.rotation.eulerAngles+" "+wall.transform.localScale);
                        UpdateLineRenderer(lineRendererXWall, wall.transform.position, wall.transform.position + wall.transform.right);
                        UpdateLineRenderer(lineRendererYWall, wall.transform.position, wall.transform.position + wall.transform.up);
                        UpdateLineRenderer(lineRendererZWall, wall.transform.position, wall.transform.position + wall.transform.forward);
                    }
                    Debug.Log("WallArt Transform: " + wallArt.transform.position+" "+wallArt.transform.rotation.eulerAngles+" "+wallArt.transform.localScale);
                    wallArtObject.transform.position = wallArt.transform.position;
                    wallArtObject.transform.rotation = wallArt.transform.rotation;
                    GameObject child = wallArtObject.transform.GetChild(0).gameObject;

                    // Calculate the scale based on PlaneBoundary2D
                    if (wallArt.PlaneBoundary2D.Count >= 4)
                    {
                        Vector2 point1 = wallArt.PlaneBoundary2D[0];
                        Vector2 point2 = wallArt.PlaneBoundary2D[1];
                        Vector2 point3 = wallArt.PlaneBoundary2D[2];
                        Vector2 point4 = wallArt.PlaneBoundary2D[3];

                        float width = Vector2.Distance(point1, point2);
                        float height = Vector2.Distance(point2, point3);

                        child.transform.localScale = new Vector3(width, height, wallArtObject.transform.localScale.z);
                    }
                    else
                    {
                        Debug.LogWarning("PlaneBoundary2D does not have enough points to calculate scale.");
                        child.transform.localScale = wallArt.transform.localScale;
                    }
                    Debug.Log("WallArtObject Transform: " + wallArtObject.transform.position+" "+wallArtObject.transform.rotation.eulerAngles+" "+wallArtObject.transform.localScale);
                    

                    UpdateLineRenderer(lineRendererXWallArt, wallArt.transform.position, wallArt.transform.position + wallArt.transform.right);
                    UpdateLineRenderer(lineRendererYWallArt, wallArt.transform.position, wallArt.transform.position + wallArt.transform.up);
                    UpdateLineRenderer(lineRendererZWallArt, wallArt.transform.position, wallArt.transform.position + wallArt.transform.forward);
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

        GameObject c3 = new GameObject("LineRenderer");
        lineRenderer2 = c3.AddComponent<LineRenderer>();
        lineRenderer2.startWidth = 0.005f;
        lineRenderer2.endWidth = 0.005f;
        lineRenderer2.material = lineMaterial;
        lineRenderer2.positionCount = 2;
    }

    private LineRenderer CreateLineRenderer(string name, Color color)
    {
        GameObject lineObject = new GameObject(name);
        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        Material standard = new Material(Shader.Find("Sprites/Default"));
        lr.material = standard;
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
            GameObject headset = GameObject.Find("CenterEyeAnchor");
            if (headset != null)
            {
                Debug.Log("Headset position: " + headset.transform.position + " Rotation: " + headset.transform.rotation.eulerAngles + " Scale: " + headset.transform.localScale);
            }
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
                Debug.Log("Hit point Wall Art: "+hit.point);
            }
            // if (wall.Raycast(ray,0.1f,out hit))
            // {
            //     lineRenderer2.SetPosition(0, pinchPoint);
            //     lineRenderer2.SetPosition(1, hit.point);
            //     Debug.Log("Hit point Wall: "+hit.point);
            // }
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