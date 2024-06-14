using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class ControllerPen : MonoBehaviour
{
    public Material material;
    private bool visual = true;
    void Update()
    {
        // if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        // {
            
        //     Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        //     Quaternion rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        //     Vector3 upInControllerSpace = new Vector3(0, -1, 0);
        //     Vector3 upInWorldSpace = rightControllerRotation * upInControllerSpace;
        //     Vector3 bottomPoint = rightControllerPosition + upInWorldSpace * 0.09f;
            
        //     GameObject point = new GameObject();
        //     point.transform.position = bottomPoint;
        //     point.transform.rotation = rightControllerRotation;
        //     MeshFilter meshFilter = point.AddComponent<MeshFilter>();
        //     meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        //     point.AddComponent<MeshRenderer>();
        //     point.GetComponent<MeshRenderer>().material = material;

        //     point.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

        // }
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
        {
            Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            Quaternion leftControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            Debug.Log("LCR: "+ leftControllerRotation);
            Vector3 euler = leftControllerRotation.eulerAngles;
            Debug.Log("Euler: "+ euler);

            // GameObject point = new GameObject();
            // point.transform.position = leftControllerPosition;
            // point.transform.rotation = leftControllerRotation;
            // MeshFilter meshFilter = point.AddComponent<MeshFilter>();
            // meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Plane.fbx");
            // point.AddComponent<MeshRenderer>();
            // point.GetComponent<MeshRenderer>().material = material;
            // point.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

            GameObject newpoint = new GameObject();
            Vector3 downDirection = leftControllerRotation * Quaternion.Euler(40,0,0) * Vector3.down;
            newpoint.transform.position = leftControllerPosition + downDirection * 0.095f;
            Debug.Log("Point 2 Position: "+ newpoint.transform.position);
            Debug.Log("Point 2 Rotation: "+ newpoint.transform.rotation.eulerAngles);
            MeshFilter newmeshFilter = newpoint.AddComponent<MeshFilter>();
            newmeshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            newpoint.AddComponent<MeshRenderer>();
            newpoint.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        }
        if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
        {
            visual = !visual;
            GameObject child = this.transform.Find("OVRControllerVisual").gameObject;
            if (child != null)
            {
                child.SetActive(visual);
            }
        }
    }
}