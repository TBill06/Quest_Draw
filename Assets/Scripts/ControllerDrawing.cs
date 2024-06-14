using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class ControllerDrawing : MonoBehaviour
{
    // Reference to the Controller component
    public Controller controller; 

    // Reference to the LineRenderer component
    private LineRenderer lineRenderer;

    // Reference to the pokeBall prefab
    public GameObject leftPokeBall;
    public GameObject rightPokeBall;

    // Reference to the current line being drawn
    private GameObject currentLine;

    // Reference to the material used for the line
    public Material leftLineMaterial;
    public Material rightLineMaterial;

    // Private field to track drawing state
    private bool isDrawingLeft = false;
    private bool isDrawingRight = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Controller component
        controller = GetComponent<Controller>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (controller.Handedness == Handedness.Left && !isDrawingLeft)
            {
                StartDrawing(leftPokeBall, leftLineMaterial);
                isDrawingLeft = true;
            }
            if (isDrawingLeft)
            {
                UpdateDrawing(leftPokeBall);
            }
        }
        else
        {
            isDrawingLeft = false;
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            if (controller.Handedness == Handedness.Right && !isDrawingRight)
            {
                StartDrawing(rightPokeBall, rightLineMaterial);
                isDrawingRight = true;
            }
            if (isDrawingRight)
            {
                UpdateDrawing(rightPokeBall);
            }
        }
        else
        {
            isDrawingRight = false;
        }
        if (!isDrawingLeft && !isDrawingRight)
        {
            StopDrawing();
        }
    }

    void StartDrawing(GameObject pokeBall, Material lineMaterial)
    {
        // Create a new line object
        currentLine = new GameObject("Line");
        lineRenderer = currentLine.AddComponent<LineRenderer>();

        // Set the line properties
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = lineMaterial;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, pokeBall.transform.position);
    }

    void UpdateDrawing(GameObject pokeBall)
    {
        if (isDrawingLeft || isDrawingRight)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pokeBall.transform.position);
        }
    }

    void StopDrawing()
    {
        // Set the drawing state to false
        isDrawingLeft = false;
        isDrawingRight = false;
    }
}
