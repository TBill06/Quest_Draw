using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a virtual board when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// It uses the OneEuroFilter to filter the hand position data.
// It uses the virtual board's collider and do raycasting to draw on the board.
// Parameters: Hand, tubeMaterial.
public class VSurfacePinchV2 : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Material tubeMaterial;
    public GameObject board;
    public GameObject capsule;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private Hand hand;
    private ProceduralTube currentTube;
    private bool createNewTube = true;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private Vector3 midPoint, indexDirection, edgePoint;
    private float length;
    private BoxCollider boxCollider;
    private Vector3 prevPose1, prevPose2;
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
            
        // Get the board's collider
        if(board != null)
        {
            Transform boardChild = board.transform.GetChild(0);
            boxCollider = boardChild.GetComponent<BoxCollider>();
        }
    }

    void Update()
    {
        // Check if the script should run and reset the variables
        if (!ScriptManager.shouldRun)
        {
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
            frames = 0;
            Pose pose1, pose2;
            bool pose1Valid = hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            bool pose2Valid = hand.GetJointPose(HandJointId.HandIndex1, out pose2);

            // Check and update the previous poses
            if (pose1Valid && pose2Valid)
            {
                prevPose1 = pose1.position;
                prevPose2 = pose2.position;
            }
            else
            {
                Debug.Log("Using Previous poses");
                pose1.position = prevPose1;
                pose2.position = prevPose2;
            }

            // Calculate capsule parameters
            midPoint = (pose1.position + pose2.position) / 2;
            indexDirection = (pose1.position - pose2.position).normalized;
            
            midPoint -= indexDirection * 0.04f;
            length = Vector3.Distance(pose1.position, pose2.position);

            length += 0.01f;
            edgePoint = midPoint - (indexDirection * length);

            // Set ray parameters
            Ray ray = new Ray(edgePoint, indexDirection);
            rayLength = length * 2.0f;
            rayLengthMax = length * 2.5f;
            float currentRayLength = hasHitOnce ? rayLengthMax : rayLength;

            // Raycast to the board
            if(boxCollider.Raycast(ray, out RaycastHit hit, currentRayLength))
            {
                hasHitOnce = true;
                if(createNewTube)
                {
                    createNewTube = false;
                    startedDrawing = true;
                    GameObject tubeObject = new GameObject("Tube");
                    tubeObject.tag = "Tube";
                    vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
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
            if (frames > 20) { finishedDrawing = true; hasHitOnce = false; }
        }
    }

    // Adds filtered point to the tube
    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 offsetPoint = point + normal * 0.015f;
        Vector3 point3D = new Vector3(offsetPoint.x, offsetPoint.y, offsetPoint.z);
        Vector3 filterPoint = vector3Filter.Filter(point3D);
        currentTube.AddPoint(filterPoint);
    }
}
