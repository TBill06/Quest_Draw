using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a virtual board when user points with their index finger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// It uses the OneEuroFilter to filter the hand position data.
// It uses the virtual board's collider and do raycasting to draw on the board.
// Open the palm (more specifically the thumb and middle finger) to stop pointing and hence stop drawing.
// Parameters: Hand, tubeMaterial.
public class VSurfacePointV2 : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial; 
    public GameObject board;
    public GameObject capsule;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector2> vector2Filter;
    private ProceduralTube currentTube;
    private bool wasPointing = false;
    private bool createNewTube = false;
    private bool indexPointerPoseDetected = false;
    private Vector3 midPoint;
    private Vector3 indexDirection;
    private float distance;
    private Vector3 edgePoint;
    private BoxCollider boxCollider;
    private MRUKAnchor boardObject;

    void Start()
    {
        vector2Filter = new OneEuroFilter<Vector2>(filterFrequency, minCutoff, beta, dcutoff);
        if(board != null)
        {
            boxCollider = board.GetComponent<BoxCollider>();
        }
    }

    void Update()
    {
        if(indexPointerPoseDetected)
        {
            if(!wasPointing)
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

            float length = (distance / 2) + 0.05f;
            edgePoint = midPoint - (indexDirection * length);

            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0);
            capsule.transform.localScale = new Vector3(0.008f, length, 0.008f);

            Ray ray = new Ray(edgePoint, indexDirection);
            if(boxCollider.Raycast(ray, out RaycastHit hit, length*2))
            {
                if(createNewTube)
                {
                    GameObject tubeObject = new GameObject("Tube");
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
        wasPointing = indexPointerPoseDetected;
    }

    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 offsetPoint = point + normal * 0.01f;
        Vector2 point2D = new Vector2(offsetPoint.x, offsetPoint.y);
        Vector2 filterPoint = vector2Filter.Filter(point2D);
        Vector3 finalPoint = new Vector3(filterPoint.x, filterPoint.y, offsetPoint.z);
        currentTube.AddPoint(finalPoint);
    }

    // Public method to set the index finger pose detected
    public void SetIndexPointerPoseDetected(bool detected)
    {
        indexPointerPoseDetected = detected;
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
                    Transform wallChild = boardObject.transform.GetChild(0);
                    Transform wallGrandChild = wallChild.GetChild(0);
                    boxCollider = wallGrandChild.GetComponent<BoxCollider>();
                }
            }
        }
    }

}
