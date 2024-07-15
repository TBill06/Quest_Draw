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
    public float filterFrequency = 120.0f;
    public float minCutoff = 1.0f;
    public float beta = 0f;
    public float dcutoff = 1.0f;

    private OneEuroFilter<Vector3> vector3Filter;
    private ProceduralTube currentTube;
    private MeshRenderer meshRenderer;

    void Start()
    {
        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
    }

    void FixedUpdate()
    {
        if(hand.GetIndexFingerIsPinching())
        {
            Pose pose1, pose2;
            hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
            hand.GetJointPose(HandJointId.HandIndex1, out pose2);
            Debug.Log("Pose1 & Pose2: "+pose1.position+" "+pose2.position);
            Vector3 filter1 = vector3Filter.Filter(pose1.position);
            Vector3 filter2 = vector3Filter.Filter(pose2.position);
            Debug.Log("Filter1 & Filter2: "+filter1+" "+filter2);
            Vector3 indexDirection = (pose1.position - pose2.position).normalized;
            Vector3 midPoint = (pose1.position + pose2.position) / 2;
            midPoint += indexDirection * 0.01f;
            float distance = Vector3.Distance(pose1.position, pose2.position)/2;
            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0);
            Rigidbody rb = capsule.GetComponent<Rigidbody>();
            rb.MovePosition(midPoint);
            rb.MoveRotation(Quaternion.LookRotation(indexDirection) * Quaternion.Euler(90, 0, 0));
            capsule.transform.localScale = new Vector3(0.01f, (distance / 2)+0.02f, 0.01f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cap"))
        {
            GameObject tubeObject = new GameObject("Tube");
            vector3Filter = new OneEuroFilter<Vector3>(filterFrequency, minCutoff, beta, dcutoff);
            currentTube = tubeObject.AddComponent<ProceduralTube>();
            meshRenderer = tubeObject.AddComponent<MeshRenderer>();
            meshRenderer.material = tubeMaterial;
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (!hand.GetIndexFingerIsPinching())
        {
            capsule.transform.position = new Vector3(0, 0, 0);
            Rigidbody rb = capsule.GetComponent<Rigidbody>();
            rb.MovePosition(new Vector3(0, 0, 0));
        }
        if (collision.gameObject.CompareTag("Cap"))
        {
            // Debug.Log("Capsule position: " + capsule.transform.position+" "+capsule.transform.rotation+" "+capsule.transform.localScale);
            // Debug.Log("This Collider info:" + this.GetComponent<Collider>().transform.position + " " + this.GetComponent<Collider>().transform.rotation + " " + this.GetComponent<Collider>().transform.localScale + " " + this.GetComponent<Collider>().name);
            // Debug.Log("Other Collider info:" + collision.collider.transform.position + " " + collision.collider.transform.rotation + " " + collision.collider.transform.localScale + " " + collision.collider.name);
            // Debug.Log("Other Collider bounds: " + collision.collider.bounds+" "+collision.collider.bounds.center+" "+collision.collider.bounds.size);
            // Debug.Log("Collision point: " + collision.contacts[0].point);
            // Debug.Log("Collision normal: " + collision.contacts[0].normal);
            Vector3 point = collision.contacts[0].point;
            Vector3 normal = collision.contacts[0].normal;
            Vector3 offsetPoint = point + normal * 0.01f;
            currentTube.AddPoint(offsetPoint);
            // Debug.Log("Tube point added"+offsetPoint);
        }
    }
}
