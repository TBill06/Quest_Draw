using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3d condition.
// Don't draw with both controllers at the same time.
// Parameters: tubeMaterial.
public class ControllerDrawing : MonoBehaviour
{
    public Material leftTubeMaterial;
    public Material rightTubeMaterial;
    private Vector3 rotationCheck;
    private bool _isDrawing = false;
    private ProceduralTube currentTube;
    private Vector3 lastDrawPoint;
    private bool isFirstPoint = true;

    public bool isDrawing
    {
        get { return _isDrawing; }
        set { _isDrawing = value; }
    }

    void Update()
    {
        Debug.Log("ControllerDrawing");
        OVRInput.Controller activeController = OVRInput.Controller.None;
        Material material = null;
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            activeController = OVRInput.Controller.RTouch;
            material = rightTubeMaterial;
        }
        else if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
        {
            activeController = OVRInput.Controller.LTouch;
            material = leftTubeMaterial;
        }
        if (activeController != OVRInput.Controller.None)
        {
            if (!isDrawing)
            {
                StartDrawing(material);
            }
            UpdateLine(activeController);
        }
        else
        {
            StopDrawing();
        }
    }

    // Initializes a new tube object
    void StartDrawing(Material material)
    {
        isDrawing = true;
        isFirstPoint = true;

        GameObject tubeObject = new GameObject("Tube");
        tubeObject.tag = "Tube";
        currentTube = tubeObject.AddComponent<ProceduralTube>();
        currentTube.material = material;
    }

    // Updates the line by adding points to the tube
    void UpdateLine(OVRInput.Controller controller)
    {
        if (isDrawing)
        {
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controller);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(controller);

            // Draw a point 0.095m below the controller (at the tip)
            if (controller == OVRInput.Controller.LTouch)
            {
                rotationCheck = new Vector3(45, 0, -5);
            }
            else
            {
                rotationCheck = new Vector3(45, 0, 5);
            }
            Vector3 downDirection = controllerRotation * Quaternion.Euler(rotationCheck) * new Vector3(-0.01f, -1, 0);
            Vector3 drawpoint = controllerPosition + downDirection * 0.095f;
            
            if (!isFirstPoint)
            {
                drawpoint = Vector3.Lerp(lastDrawPoint, drawpoint, 0.5f);
                lastDrawPoint = drawpoint;
            }
            else
            {
                lastDrawPoint = drawpoint;
                isFirstPoint = false;
            }
            currentTube.AddPoint(drawpoint);
        }
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }
}