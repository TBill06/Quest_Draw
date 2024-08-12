using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;

// This script draws in 3D space when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw tubes in 3D space. Ideal to use for our draw in 3d condition.
// Don't draw with both controllers at the same time.
// Parameters: tubeMaterial.
public class ControllerDrawingV2 : MonoBehaviour
{
    public Material tubeMaterial;
    private Vector3 rotationCheck;
    private bool createNewTube = true;
    private bool _finishedDrawing = false;
    private bool _startedDrawing = false;
    private int frames = 0;
    private ProceduralTube currentTube;
    private Vector3 lastDrawPoint;
    private bool isFirstPoint = true;

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
        
        // Set the active controller
        OVRInput.Controller activeController = OVRInput.Controller.None;
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            activeController = OVRInput.Controller.RTouch;
        }
        else if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
        {
            activeController = OVRInput.Controller.LTouch;
        }

        // Check if the controller is active
        if (activeController != OVRInput.Controller.None)
        {
            frames = 0;
            if (createNewTube)
            {
                createNewTube = false;
                startedDrawing = true;
                isFirstPoint = true;

                GameObject tubeObject = new GameObject("Tube");
                tubeObject.tag = "Tube";
                currentTube = tubeObject.AddComponent<ProceduralTube>();
                currentTube.material = tubeMaterial;
                
            }
            UpdateLine(activeController);
        }
        else
        {
            if (startedDrawing)
            {
                frames++;
                if (frames > 20) { finishedDrawing = true; }
            }
        }
    }

    // Updates the line by adding points to the tube
    void UpdateLine(OVRInput.Controller controller)
    {
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controller);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(controller);

        // Set the rotation offset based on the active controller
        if (controller == OVRInput.Controller.LTouch)
        {
            rotationCheck = new Vector3(45, 0, -5);
        }
        else
        {
            rotationCheck = new Vector3(45, 0, 5);
        }

        // Draw a point 0.095m below the controller (at the tip)
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