using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a physical board when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in physical surface condition.
// Don't draw with both controllers at the same time.
// It uses the physical board's BoxCollider and do raycasting to draw on the board.
// Parameters: tubeMaterial.
public class PSurfaceControllerV2 : MonoBehaviour
{
    public Material tubeMaterial;
    private ProceduralTube currentTube;
    private bool createNewTube = true;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private bool isFirstPoint = true;
    private Vector3 rotationCheck, midPoint, downDirection, controllerTipPosition, edgePoint;
    private float length;
    private Vector3 lastDrawPoint;
    private BoxCollider boxCollider;
    private bool hasHitOnce = false;
    private float rayLength, rayLengthMax;

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
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(activeController);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(activeController);

            // Set the rotation offset based on the active controller
            if (activeController == OVRInput.Controller.LTouch)
            {
                rotationCheck = new Vector3(45, 0, -5);
            }
            else
            {
                rotationCheck = new Vector3(45, 0, 5);
            }

            // Calculate ray parameters
            downDirection = controllerRotation * Quaternion.Euler(rotationCheck) * new Vector3(-0.01f, -1, 0);
            controllerTipPosition = controllerPosition + downDirection * 0.095f;

            midPoint = (controllerTipPosition + controllerPosition) / 2;
            midPoint -= downDirection * 0.04f;

            length = Vector3.Distance(controllerTipPosition, controllerPosition);
            length += 0.01f;
            edgePoint = midPoint - (downDirection * length);

            // Set ray parameters
            Ray ray = new Ray(edgePoint, downDirection);
            rayLength = length * 2.0f;
            rayLengthMax = length * 2.1f;
            float currentRayLength = hasHitOnce ? rayLengthMax : rayLength;

            // Raycast to the board
            if(boxCollider.Raycast(ray, out RaycastHit hit, currentRayLength))
            {
                frames = 0;
                hasHitOnce = true;
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
                UpdateLine(hit.point, hit.normal);
            }
            else
            {
                FinishDrawing();
            }
        }
        else
        {
            FinishDrawing();
        }
    }

    void FinishDrawing()
    {
        if (startedDrawing)
        {
            frames++;
            if (frames > 10) { finishedDrawing = true; hasHitOnce = false; }
        }
    }

    // Updates the line by adding points to the tube
    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 drawPoint = point + normal * 0.015f;
        if (!isFirstPoint)
        {
            drawPoint = Vector3.Lerp(lastDrawPoint, drawPoint, 0.5f);
            lastDrawPoint = drawPoint;
        }
        else
        {
            lastDrawPoint = drawPoint;
            isFirstPoint = false;
        }
        currentTube.AddPoint(drawPoint);
    }

    // Method to get the Board Object
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
                    Transform wallChild = anchor.transform.GetChild(0);
                    Transform wallGrandChild = wallChild.GetChild(0);
                    boxCollider = wallGrandChild.GetComponent<BoxCollider>();
                    break;
                }
            }
        }
    }
}
