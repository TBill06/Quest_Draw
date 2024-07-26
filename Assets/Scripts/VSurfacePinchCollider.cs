using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

public class VSurfacePinchCollider : MonoBehaviour
{
    public Hand hand;
    public Material tubeMaterial; 
    public GameObject capsule;
    public GameObject edge;
    public float filterFrequency = 90.0f;
    public float minCutoff = 1.0f;
    public float beta = 10f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector2> vector2Filter;
    private ProceduralTube currentTube;
    private bool wasPinching = false;
    private bool createNewTube = false;
    private Vector3 midPoint;
    private Vector3 indexDirection;
    private float distance;
    private Rigidbody rb;
    private Vector3 edgePoint;

    void Start()
    {
        vector2Filter = new OneEuroFilter<Vector2>(filterFrequency, minCutoff, beta, dcutoff);
        rb = capsule.GetComponent<Rigidbody>();
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
            
            midPoint = (pose1.position + pose2.position) / 2;
            midPoint -= indexDirection * 0.04f;

            indexDirection = (pose1.position - pose2.position).normalized;
            distance = Vector3.Distance(pose1.position, pose2.position);

            float length = (distance / 2) + 0.05f;
            edgePoint = midPoint + (indexDirection * length);

            // capsule.transform.position = midPoint;
            // capsule.transform.rotation = Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0)
            rb.MovePosition(midPoint);
            rb.MoveRotation(Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0));
            capsule.transform.localScale = new Vector3(0.01f, length, 0.01f);
        }
        wasPinching = currentlyPinching;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cap"))
        {
            Debug.Log("Collided Enter with cap");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cap"))
        {
            if(createNewTube)
            {
                GameObject tubeObject = new GameObject("Tube");
                vector2Filter = new OneEuroFilter<Vector2>(filterFrequency, minCutoff, beta, dcutoff);
                currentTube = tubeObject.AddComponent<ProceduralTube>();
                currentTube.material = tubeMaterial;
                createNewTube = false;
            }
            Vector3 point = other.ClosestPoint(edgePoint);
            Vector3 normal = -(edgePoint - point).normalized;
            Vector3 offsetPoint = point + normal * 0.01f;
            Vector2 point2D = new Vector2(offsetPoint.x, offsetPoint.y);
            Vector2 filterPoint = vector2Filter.Filter(point2D);
            Vector3 finalPoint = new Vector3(filterPoint.x, filterPoint.y, offsetPoint.z);

            currentTube.AddPoint(finalPoint);
            // Debug.Log("R---Point: "+point+" Normal: "+normal+" Offset Point: "+offsetPoint+" Final Point: "+finalPoint);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cap"))
        {
            Debug.Log("Collided Exit with cap");
            createNewTube = true;
        }
    }
}
