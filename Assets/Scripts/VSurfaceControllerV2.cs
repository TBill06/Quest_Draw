using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using Meta.XR.MRUtilityKit;

// This script draws on a virtual board when user holds the controller like a pen and presses the hand trigger.
// It uses the ProceduralTube component to draw the tubes. Ideal to use for our draw in virtual surface condition.
// Don't draw with both controllers at the same time.
// It uses the virtual board's collider and do raycasting to draw on the board.
// Parameters: tubeMaterial.
public class VSurfaceControllerV2 : MonoBehaviour
{
    public Material leftTubeMaterial;
    public Material rightTubeMaterial;
    public GameObject board;
    public GameObject capsule;
    private ProceduralTube currentTube;
    private bool _isDrawing = false;
    private bool isFirstPoint = true;
    private Vector3 rotationCheck, midPoint, downDirection, controllerTipPosition, edgePoint;
    private float distance, length;
    private Vector3 lastDrawPoint;
    private BoxCollider boxCollider;
    private MRUKAnchor boardObject;

    public bool isDrawing
    {
        get { return _isDrawing; }
        set { _isDrawing = value; }
    }

    void Start()
    {
        if(board != null)
        {
            boxCollider = board.GetComponent<BoxCollider>();
        }
    }

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
            if(boxCollider.Raycast(ray, out RaycastHit hit, length*2))
            {
                if (!isDrawing)
                {
                    StartDrawing(material);
                }
                UpdateLine(hit.point, hit.normal);
            }
            else
            {
                StopDrawing();
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
        Vector3 drawPoint = point + normal * 0.01f;  
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
                    Transform wallChild = boardObject.transform.GetChild(0);
                    Transform wallGrandChild = wallChild.GetChild(0);
                    boxCollider = wallGrandChild.GetComponent<BoxCollider>();
                }
            }
        }
    }
}
