using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a physical board when user points with their index finger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in physcial surface condition.
// It uses the OneEuroFilter to filter the hand position data.
// It uses the physcial board's MRUKAnchor and do raycasting to draw on the board.
// Open the palm (more specifically the thumb and middle finger) to stop pointing and hence stop drawing.
// Parameters: Hand, tubeMaterial.
public class PSurfacePointV2 : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Material tubeMaterial;
    public GameObject poseDetector_L;
    public GameObject poseDetector_R;
    public GameObject capsule;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private Hand hand;
    private ProceduralTube currentTube;
    private bool createNewTube = true;
    private bool indexPointerPoseDetected = false;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private Vector3 midPoint, indexDirection, edgePoint;
    private float length;
    private MRUKAnchor boardObject;
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
            
        if(indexPointerPoseDetected)
        {
            // Get hand poses
            frames = 0;
            Pose pose1, pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandIndex1, out pose2);

            // Check and update the previous poses
            if (pose1.position != Vector3.zero && pose2.position != Vector3.zero)
            {
                prevPose1 = pose1.position;
                prevPose2 = pose2.position;
            }
            else
            {
                Debug.Log("Using previous poses");
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

            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0);
            capsule.transform.localScale = new Vector3(0.005f, length, 0.005f);

            // Set ray parameters
            Ray ray = new Ray(edgePoint, indexDirection);
            rayLength = length * 2.0f;
            rayLengthMax = length * 3.0f;
            float currentRayLength = hasHitOnce ? rayLengthMax : rayLength;

            // Raycast to the board
            if(boardObject.Raycast(ray, currentRayLength, out RaycastHit hit))
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
        }
        else
        {
            if (startedDrawing)
            {
                frames++;
                if (frames > 20) { finishedDrawing = true; hasHitOnce = false; }
            }
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

    // Public method to set the index finger pose detected
    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
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
                    boardObject = anchor;
                    break;
                }
            }
        }
    }

}
