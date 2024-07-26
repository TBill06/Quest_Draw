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
    private bool isDrawing = false;
    private ProceduralTube currentTube;

    void Update()
    {
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

        GameObject tubeObject = new GameObject("Tube");
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
            Vector3 downDirection = controllerRotation * Quaternion.Euler(45,5,0) * new Vector3(-0.01f, -1, 0);
            Vector3 drawpoint = controllerPosition + downDirection * 0.095f;
            currentTube.AddPoint(drawpoint);
        }
    }

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
    }
}