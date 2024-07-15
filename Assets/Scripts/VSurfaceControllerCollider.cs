using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus.Interaction.Surfaces;
using Unity.ProceduralTube;

// This script draws on a virtual quad when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// Don't draw with both controllers at the same time.
// Parameters: tubeMaterial.
public class VSurfaceControllerCollider : MonoBehaviour
{
    public Material leftTubeMaterial;
    public Material rightTubeMaterial;
    public Vector3 rotationCheck;
    public GameObject capsule;
    private ProceduralTube currentTube;
    private MeshRenderer meshRenderer;
    public Material tubeMaterial;
    private OVRInput.Controller activeController;

    void Update()
    {
        activeController = OVRInput.Controller.None;
        tubeMaterial = null;
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            activeController = OVRInput.Controller.RTouch;
            tubeMaterial = rightTubeMaterial;
        }
        else if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
        {
            activeController = OVRInput.Controller.LTouch;
            tubeMaterial = leftTubeMaterial;
        }
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
        {
            GameObject visual = GameObject.Find("OVRCameraRigInteraction/OVRCameraRig/OVRInteractionComprehensive/OVRControllers/LeftController/OVRControllerVisual");
            if(visual.activeSelf)
            {
                visual.SetActive(false);
            }
            else
            {
                visual.SetActive(true);
            }

        }
        else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            GameObject visual = GameObject.Find("OVRCameraRigInteraction/OVRCameraRig/OVRInteractionComprehensive/OVRControllers/RightController/OVRControllerVisual");
            if(visual.activeSelf)
            {
                visual.SetActive(false);
            }
            else
            {
                visual.SetActive(true);
            }
        }
        if (activeController != OVRInput.Controller.None)
        {
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(activeController);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(activeController);
            Vector3 downDirection = controllerRotation * Quaternion.Euler(rotationCheck) * new Vector3(-0.01f, -1, 0);
            Vector3 controllerTipPosition = controllerPosition + downDirection * 0.095f;
            
            Vector3 midPoint = (controllerTipPosition + controllerPosition) / 2;
            midPoint += downDirection * 0.01f;
            float distance = Vector3.Distance(controllerTipPosition, controllerPosition)/2;
            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(downDirection) * Quaternion.Euler(90, 0, 0);
            Rigidbody rb = capsule.GetComponent<Rigidbody>();
            rb.MovePosition(midPoint);
            rb.MoveRotation(Quaternion.LookRotation(downDirection) * Quaternion.Euler(90, 0, 0));
            capsule.transform.localScale = new Vector3(0.01f, (distance / 2)+0.02f, 0.01f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cap"))
        {
            GameObject tubeObject = new GameObject("Tube");
            currentTube = tubeObject.AddComponent<ProceduralTube>();
            meshRenderer = tubeObject.AddComponent<MeshRenderer>();
            meshRenderer.material = tubeMaterial;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (activeController == OVRInput.Controller.None)
        {
            capsule.transform.position = new Vector3(0, 0, 0);
            Rigidbody rb = capsule.GetComponent<Rigidbody>();
            rb.MovePosition(new Vector3(0, 0, 0));
            capsule.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
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
