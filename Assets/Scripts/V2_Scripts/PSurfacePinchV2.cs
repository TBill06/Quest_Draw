using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a physical board when user pinches with their index and thumb fingers.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in physical surface condition.
// It uses the OneEuroFilter to filter the hand position data.
// It uses the physical board's MRUKAnchor and do raycasting to draw on the board.
// Parameters: Hand, tubeMaterial.
public class PSurfacePinchV2 : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;
    public Material tubeMaterial; 
    public GameObject capsule;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector2> vector2Filter;
    private Hand hand;
    private ProceduralTube currentTube;
    private bool wasPinching = false;
    private bool createNewTube = false;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private Vector3 midPoint, indexDirection, edgePoint;
    private float distance, length;
    private MRUKAnchor boardObject;

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
            
        vector2Filter = new OneEuroFilter<Vector2>(filterFrequency, minCutoff, beta, dcutoff);
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
            
        bool currentlyPinching = hand.GetIndexFingerIsPinching();
        if(currentlyPinching)
        {
            frames = 0;
            if(!wasPinching)
            {
                createNewTube = true;
            }

            Pose pose1, pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandIndex1, out pose2);
            
            midPoint = (pose1.position + pose2.position) / 2;
            indexDirection = (pose1.position - pose2.position).normalized;
            
            midPoint -= indexDirection * 0.04f;
            distance = Vector3.Distance(pose1.position, pose2.position);

            length = (distance / 2) + 0.05f;
            edgePoint = midPoint - (indexDirection * length);

            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0);
            capsule.transform.localScale = new Vector3(0.008f, length, 0.008f);

            Ray ray = new Ray(edgePoint, indexDirection);
            if(boardObject.Raycast(ray, length*2.1f, out RaycastHit hit))
            {
                if(createNewTube)
                {
                    startedDrawing = true;
                    GameObject tubeObject = new GameObject("Tube");
                    tubeObject.tag = "Tube";
                    vector2Filter = new OneEuroFilter<Vector2>(filterFrequency, minCutoff, beta, dcutoff);
                    currentTube = tubeObject.AddComponent<ProceduralTube>();
                    currentTube.material = tubeMaterial;
                    createNewTube = false;
                }
                UpdateLine(hit.point, hit.normal);      
            }
            else
            {
                createNewTube = true;
            }
        }
        else
        {
            if (startedDrawing)
            {
                frames++;
                if (frames > 200) { finishedDrawing = true; }
            }
        }
        wasPinching = currentlyPinching;
    }

    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 offsetPoint = point + normal * 0.02f;
        Vector2 point2D = new Vector2(offsetPoint.x, offsetPoint.y);
        Vector2 filterPoint = vector2Filter.Filter(point2D);
        Vector3 finalPoint = new Vector3(filterPoint.x, filterPoint.y, offsetPoint.z);
        currentTube.AddPoint(finalPoint);
    }
    
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
