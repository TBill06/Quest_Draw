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
public class TestRB : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial; 
    public GameObject board;
    public GameObject capsule;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private ProceduralTube currentTube;
    private bool wasPinching = false;
    private bool createNewTube = false;
    private Vector3 midPoint;
    private Vector3 indexDirection;
    private float distance;
    private Vector3 edgePoint;
    private BoxCollider boxCollider;
    private MRUKAnchor boardObject;
    private LineRenderer lineRenderer;

    void Start()
    {
        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
        if(board != null)
        {
            boxCollider = board.GetComponent<BoxCollider>();
        }

        GameObject c1 = new GameObject("Line");
        lineRenderer = c1.AddComponent<LineRenderer>();
        lineRenderer.material = tubeMaterial;
        lineRenderer.startWidth = 0.001f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        bool currentlyPinching = hand.GetIndexFingerIsPinching();
        if(currentlyPinching)
        {
            if(!wasPinching)
            {
                createNewTube = true;
            }

            Pose pose1, pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandIndex1, out pose2);

            Vector3 filter1 = vector3Filter.Filter(pose1.position);
            Vector3 filter2 = vector3Filter.Filter(pose2.position);
            
            midPoint = (filter1 + filter2) / 2;
            indexDirection = (pose1.position - pose2.position).normalized;

            midPoint -= indexDirection * 0.04f;
            distance = Vector3.Distance(pose1.position, pose2.position);

            float length = (distance / 2) + 0.055f;
            edgePoint = midPoint - (indexDirection * length);

            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0);
            capsule.transform.localScale = new Vector3(0.008f, length, 0.008f);

            Ray ray = new Ray(edgePoint, indexDirection);
            if(boxCollider != null)
            {
                if(boxCollider.Raycast(ray, out RaycastHit hit, length*2f))
                {
                    if(createNewTube)
                    {
                        GameObject tubeObject = new GameObject("Tube");
                        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
                        currentTube = tubeObject.AddComponent<ProceduralTube>();
                        currentTube.material = tubeMaterial;
                        createNewTube = false;
                    }
                    UpdateLine(hit.point, hit.normal);   
                    lineRenderer.SetPosition(0, edgePoint); 
                    lineRenderer.SetPosition(1, hit.point);
                }
                else
                {
                    createNewTube = true;
                }
            }
        }
        wasPinching = currentlyPinching;
    }

    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 offsetPoint = point + normal * 0.01f;
        Debug.Log("R-- Hit point: "+point+" Normal: "+normal+" Offset point: "+offsetPoint);
        currentTube.AddPoint(offsetPoint);
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
