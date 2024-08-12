using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user points with their index finger.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3d condition.
// Open the palm (more specifically the thumb and middle finger) to stop pointing and hence stop drawing.
// The script uses the OneEuroFilter to filter the hand position data.
// Parameters: Hand, tubeMaterial.
public class PointDrawingV2 : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Material tubeMaterial;
    public GameObject poseDetector_L;
    public GameObject poseDetector_R;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;
    
    private OneEuroFilter<Vector3> vector3Filter;
    private Hand hand;
    private bool indexPointerPoseDetected = false;
    private bool createNewTube = true;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private ProceduralTube currentTube;
    private Vector3 prevPose1;

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
        // Set the hand and pose to use
        int left = PlayerPrefs.GetInt("left");
        if (left == 1)
        {
            hand = leftHand;
            poseDetector_L.SetActive(true);
            poseDetector_R.SetActive(false);
        }
        else
        {
            hand = rightHand;
            poseDetector_L.SetActive(false);
            poseDetector_R.SetActive(true);
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
            
        if (indexPointerPoseDetected)
        {
            frames = 0;
            if (createNewTube)
            {
                createNewTube = false;
                startedDrawing = true;
                GameObject tubeObject = new GameObject("Tube");
                tubeObject.tag = "Tube";
                vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
                currentTube = tubeObject.AddComponent<ProceduralTube>();
                currentTube.material = tubeMaterial;
            }
            UpdateLine();
        }
        else
        {
            if (startedDrawing)
            {
                frames++;
                if (frames > 20) { finishedDrawing = true; }
            }
        }
    }

    // Public method to set the index finger pose detected
    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
    }

    // Updates the line by adding points to the tube
    void UpdateLine()
    {
        // Get the position of the index finger tip
        Pose pose1;
        bool pose1Valid = hand.GetJointPose(HandJointId.HandIndexTip, out pose1);

        // Check and update the previous poses
        if (pose1Valid)
        {
            prevPose1 = pose1.position;
        }
        else
        {
            Debug.Log("Using Previous poses");
            pose1.position = prevPose1;
        }

        // Filter the hand position
        Vector3 filteredPoint = vector3Filter.Filter(pose1.position);

        // Add the point to the tube
        currentTube.AddPoint(filteredPoint);
    }
}