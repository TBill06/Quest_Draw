using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3d condition.
// The script uses the OneEuroFilter to filter the hand position data.
// Parameters: Hand, tubeMaterial.
public class PinchDrawingV2 : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Material tubeMaterial;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private Hand hand;
    private ProceduralTube currentTube;
    private bool isDrawing = false;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;

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
        int left = PlayerPrefs.GetInt("left");
        if (left == 1)
            hand = leftHand;
        else
            hand = rightHand;
    }

    void Update()
    {
        if (!ScriptManager.shouldRun)
        {
            startedDrawing = false;
            finishedDrawing = false;
            frames = 0;
            return;
        }
            
        if (hand.GetIndexFingerIsPinching())
        {
            frames = 0;
            if (!isDrawing)
            {
                StartDrawing();
                startedDrawing = true;
            }
            UpdateLine();
        }
        else
        {
            if (startedDrawing)
            {
                frames++;
                if (frames > 200) { finishedDrawing = true; }
            }
            StopDrawing();
        }
    }

    // Initializes a new tube object
    void StartDrawing()
    {
        isDrawing = true;
        
        // Initialize the filter and the tube object
        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency,minCutoff,beta,dcutoff);
        GameObject tubeObject = new GameObject("Tube");
        tubeObject.tag = "Tube";
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        currentTube.material = tubeMaterial;

    }

    // Updates the line by adding points to the tube
    void UpdateLine()
    {
        if (isDrawing)
        {
            // Get the hand positions
            Pose pose1;
            Pose pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandThumbTip, out pose2);

            // Filter the hand positions
            Vector3 filter1 = vector3Filter.Filter(pose1.position);
            Vector3 filter2 = vector3Filter.Filter(pose2.position);
            Vector3 drawpoint = (filter1 + filter2) / 2;
            
            // Add point to the tube
            currentTube.AddPoint(drawpoint);
        }
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }
}