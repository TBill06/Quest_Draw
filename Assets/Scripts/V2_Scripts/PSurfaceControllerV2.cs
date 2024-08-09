using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a physcial board when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in physcial surface condition.
// Don't draw with both controllers at the same time.
// It uses the physcial board's MRUKAnchor and do raycasting to draw on the board.
// Parameters: tubeMaterial.
public class PSurfaceControllerV2 : MonoBehaviour
{
    public Material leftTubeMaterial;
    public Material rightTubeMaterial;
    public GameObject capsule;
    private ProceduralTube currentTube;
    private bool isDrawing = false;
    private bool _startedDrawing = false;
    private bool _finishedDrawing = false;
    private int frames = 0;
    private bool isFirstPoint = true;
    private Vector3 rotationCheck, midPoint, downDirection, controllerTipPosition, edgePoint;
    private float distance, length;
    private Vector3 lastDrawPoint;
    private MRUKAnchor boardObject;

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
        if (!ScriptManager.shouldRun)
        {
            startedDrawing = false;
            finishedDrawing = false;
            frames = 0;
            return;
        }
            
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
            frames = 0;
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(activeController);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(activeController);

            if (activeController == OVRInput.Controller.LTouch)
            {
                rotationCheck = new Vector3(45, 0, -5);
            }
            else
            {
                rotationCheck = new Vector3(45, 0, 5);
            }
            downDirection = controllerRotation * Quaternion.Euler(rotationCheck) * new Vector3(-0.01f, -1, 0);
            controllerTipPosition = controllerPosition + downDirection * 0.095f;

            midPoint = (controllerTipPosition + controllerPosition) / 2;
            midPoint -= downDirection * 0.04f;

            distance = Vector3.Distance(controllerTipPosition, controllerPosition);
            length = (distance / 2) + 0.05f;
            edgePoint = midPoint - (downDirection * length);

            capsule.transform.position = midPoint;
            capsule.transform.rotation = Quaternion.LookRotation(downDirection) * Quaternion.Euler(90, 0, 0);
            capsule.transform.localScale = new Vector3(0.008f, length, 0.008f);

            Ray ray = new Ray(edgePoint, downDirection);
            if(boardObject.Raycast(ray, length*2f, out RaycastHit hit))
            {
                if (!isDrawing)
                {
                    StartDrawing(material);
                    startedDrawing = true;
                }
                UpdateLine(hit.point, hit.normal);
            }
            else
            {
                StopDrawing();
            }
        }
        else
        {
            if (startedDrawing)
            {
                frames++;
                if (frames > 100) { finishedDrawing = true; }
            }
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
    void UpdateLine(Vector3 point, Vector3 normal)
    {
        Vector3 drawPoint = point + normal * 0.02f;  
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

    // Stops drawing
    void StopDrawing()
    {
        isDrawing = false;
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
                    break;
                }
            }
        }
    }
}
