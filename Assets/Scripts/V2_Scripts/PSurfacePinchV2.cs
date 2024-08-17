using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a physical board when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in physical surface condition.
// It uses the OneEuroFilter to filter the hand position data.
// It uses the physical board's BoxCollider and do raycasting to draw on the board.
// Parameters: Hand, tubeMaterial.
public class PSurfacePinchV2 : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Material tubeMaterial;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private OneEuroFilter<Vector3> vector3Filter2;
    private Hand hand;
    private ProceduralTube currentTube;
    private bool createNewTube = true;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private Vector3 midPoint, direction, edgePoint;
    private float length;
    private BoxCollider boxCollider;
    private Vector3 prevPose1, prevPose2, prevPose3, prevPose4;
    private bool hasHitOnce = false;
    private float rayLength, rayLengthMax;

    public bool startedDrawing
    {
        get { return _startedDrawing; }
        set { _startedDrawing = value; }
    }

    public bool finishedDrawing
    {
        get { return _finishedDrawing; }
        set { _finishedDrawing = value; }
    }

    void Start()
    {
        // Set the hand to use
        int left = PlayerPrefs.GetInt("left");
        if (left == 1)
            hand = leftHand;
        else
            hand = rightHand;
    }

    void Update()
    {
        // Check if the script should run and reset the variables
        if (!ScriptManager.shouldRun)
        {
            vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
            vector3Filter2 = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
            startedDrawing = false;
            finishedDrawing = false;
            createNewTube = true;
            frames = 0;
            return;
        }
            
        bool currentlyPinching = hand.GetIndexFingerIsPinching();
        if(currentlyPinching)
        {
            // Get the hand poses
            Pose pose1, pose2, pose3, pose4;
            bool pose1Valid = hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            bool pose2Valid = hand.GetJointPose(HandJointId.HandIndex1, out pose2);
            bool pose3Valid = hand.GetJointPose(HandJointId.HandThumbTip, out pose3);
            bool pose4Valid = hand.GetJointPose(HandJointId.HandThumb1, out pose4);

            // Check and update the previous poses
            if (pose1Valid && pose2Valid && pose3Valid && pose4Valid)
            {
                prevPose1 = pose1.position;
                prevPose2 = pose2.position;
                prevPose3 = pose3.position;
                prevPose4 = pose4.position;
            }
            else
            {
                pose1.position = prevPose1;
                pose2.position = prevPose2;
                pose3.position = prevPose3;
                pose4.position = prevPose4;
            }
            
            // Calculate ray parameters
            // Uses the mid point between the index and thumb fingers tip and 1st joint to calculate the ray.
            Vector3 indexMid = (pose1.position + pose2.position) / 2;
            Vector3 indexDirection = (pose1.position - pose2.position).normalized;
            Vector3 thumbMid = (pose3.position + pose4.position) / 2;
            Vector3 thumbDirection = (pose3.position - pose4.position).normalized;

            midPoint = (indexMid + thumbMid) / 2;
            direction = (indexDirection + thumbDirection) / 2;
            
            midPoint -= direction * 0.04f;
            length = Vector3.Distance(pose1.position, pose2.position);

            length += 0.01f;
            edgePoint = midPoint - (direction * length);

            // Filter the data
            edgePoint = vector3Filter2.Filter(edgePoint);
            direction = vector3Filter.Filter(direction);

            // Set ray parameters
            Ray ray = new Ray(edgePoint, direction);
            rayLength = length * 2.0f;
            rayLengthMax = length * 2.1f;
            float currentRayLength = hasHitOnce ? rayLengthMax : rayLength;

            // Raycast to the board
            if(boxCollider.Raycast(ray, out RaycastHit hit, currentRayLength))
            {
                frames = 0;
                hasHitOnce = true;
                if(createNewTube)
                {
                    createNewTube = false;
                    startedDrawing = true;
                    GameObject tubeObject = new GameObject("Tube");
                    tubeObject.tag = "Tube";
                    currentTube = tubeObject.AddComponent<ProceduralTube>();
                    currentTube.material = tubeMaterial;
                }
                UpdateLine(hit.point, hit.normal);   
            }
            else
            {
                FinishDrawing();
            }
        }
        else
        {
            FinishDrawing();
        }
    }

    void FinishDrawing()
    {
        if (startedDrawing)
        {
            frames++;
            if (frames > 10) { finishedDrawing = true; hasHitOnce = false; }
        }
    }

    // Adds filtered point to the tube
    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 offsetPoint = point + normal * 0.015f;
        currentTube.AddPoint(offsetPoint);
    }
    
    // Method to get the Board Object
    public void OnSceneInitialized()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room != null)
        {
            List<MRUKAnchor> anchors = room.Anchors;
            foreach (MRUKAnchor anchor in anchors)
            {
                if (anchor.Label.ToString() == "WALL_ART")
                {
                    Transform wallChild = anchor.transform.GetChild(0);
                    Transform wallGrandChild = wallChild.GetChild(0);
                    boxCollider = wallGrandChild.GetComponent<BoxCollider>();
                    break;
                }
            }
        }
    }

}
