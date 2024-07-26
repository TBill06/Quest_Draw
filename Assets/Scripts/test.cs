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
    public GameObject cubeIns;
    private bool sceneInitialized = false;
    private MRUKAnchor wall;
    private MRUKAnchor parent;
    private LineRenderer lineRenderer;
    private LineRenderer lineRenderer2;
    private ProceduralTube currentTube;
    private Mesh currentTubeMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

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
                Debug.Log("Anchor: " + anchor.Label);
                if (anchor.Label.ToString() == "WALL_ART")
                {
                    parent = anchor;
                    parent.ParentAnchor = wall;
                    Debug.Log("Wall Transform: " + wall.transform.position+" "+wall.transform.rotation.eulerAngles+" "+wall.transform.localScale);
                    Debug.Log("WallArt Transform: " + parent.transform.position+" "+parent.transform.rotation.eulerAngles+" "+parent.transform.localScale);
                    Transform wallChild = parent.transform.GetChild(0);
                    Debug.Log("WallArt Child Transform: " + wallChild.position+" "+wallChild.rotation.eulerAngles+" "+wallChild.localScale);
                    Transform wallGrandChild = wallChild.GetChild(0);
                    Debug.Log("WallArt GrandChild Transform: " + wallGrandChild.position+" "+wallGrandChild.rotation.eulerAngles+" "+wallGrandChild.localScale);
                    Debug.Log("WallArt GrandChild Collider: " + wallGrandChild.GetComponent<BoxCollider>().bounds);

                    // GameObject cube = Instantiate(cubeIns, parent.transform.position, parent.transform.rotation);
                    // cube.transform.localScale = parent.transform.localScale;
                    // Debug.Log("Cube Transform: " + cube.transform.position+" "+cube.transform.rotation.eulerAngles+" "+cube.transform.localScale);
                    // Transform cubeChild = cube.transform.GetChild(0);
                    // Debug.Log("Cube Child Transform Before: " + cubeChild.position+" "+cubeChild.rotation.eulerAngles+" "+cubeChild.localScale);
                    // cubeChild.position = wallChild.position;
                    // cubeChild.rotation = wallChild.rotation;
                    // cubeChild.localScale = wallChild.localScale;
                    // Debug.Log("Cube Child Transform After: " + cubeChild.position+" "+cubeChild.rotation.eulerAngles+" "+cubeChild.localScale);
                    // Transform cubeGrandChild = cubeChild.GetChild(0);
                    // Debug.Log("Cube GrandChild Transform Before: " + cubeGrandChild.position+" "+cubeGrandChild.rotation.eulerAngles+" "+cubeGrandChild.localScale);
                    // Debug.Log("Cube GrandChild Collider: " + cubeGrandChild.GetComponent<Collider>().bounds);
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
            if (wall.Raycast(ray,0.1f,out hit))
            {
                lineRenderer.SetPosition(0, pinchPoint);
                lineRenderer.SetPosition(1, hit.point);
                Debug.Log("Hit point: "+hit.point);
            }
            if (parent.Raycast(ray,0.1f,out hit))
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
        else
        {
            StopDrawing();
        }
    }
    
    // Initializes a new tube object
    void StartDrawing()
    {
        // isDrawing = true;

        GameObject tubeObject = new GameObject("Tube");
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        meshRenderer = tubeObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tubeMaterial;
    }

     // Updates the line by adding points to the tube
    void UpdateLine(Vector3 point, RaycastHit hit)
    {
        Vector3 hitPoint;
        hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z+0.009f);
        Debug.Log("Added point from the front: "+hitPoint);
        currentTube.AddPoint(hitPoint);
    }

    // Stops drawing
    void StopDrawing()
    {
        // isDrawing = false;
    }
}