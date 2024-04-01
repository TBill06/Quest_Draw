// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Oculus.Interaction.Input;

// public class HandDrawing : MonoBehaviour
// {
//     // Reference to the Hand component
//     public Hand hand;

//     // Reference to the LineRenderer component
//     private LineRenderer lineRenderer;

//     // Reference to the current line being drawn
//     private GameObject currentLine;

//     // Reference to the material used for the line
//     public Material leftLineMaterial;
//     public Material rightLineMaterial;

//     // Private fields to track drawing state
//     private bool isDrawing = false;
//     public bool isAutoDrawing = true;


//     // Start is called before the first frame update
//     void Start()
//     {
//         // Get the Hand component
//         hand = GetComponent<Hand>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             isAutoDrawing = !isAutoDrawing;
//         }

//         if (isAutoDrawing)
//         {
//             AutoDraw();
//         }
//         else
//         {
//             ManualDraw();
//         }
//     }

//     void AutoDraw()
//     {
//         // Check if the index finger is pinching
//         if (hand.GetIndexFingerIsPinching())
//         {
//             // If we're not currently drawing, start a new line
//             if (!isDrawing)
//             {
//                 StartDrawing();
//             }
//             // Add a point to the line at the index tip's position
//             UpdateLine();
//         }
//         else
//         {
//             StopDrawing();
//         } 
//     }

//     void ManualDraw()
//     {
//         // If we're not currently drawing, start a new line
//         if (!isDrawing)
//         {
//             StartDrawing();
//             UpdateLine();
//         }
//         // If we were drawing, stop drawing
//         else
//         {
//             StopDrawing();
//         }
//     }

//     void StartDrawing()
//     {
//         // Set the drawing state to true
//         isDrawing = true;

//         // Create a new GameObject for the line
//         GameObject lineObject = new GameObject("Line");
        
//         // Add a LineRenderer to the new GameObject
//         lineRenderer = lineObject.AddComponent<LineRenderer>();

//         // Set the material and other properties of the line
//         lineRenderer.material = hand.Handedness == Handedness.Left ? leftLineMaterial : rightLineMaterial;
//         lineRenderer.startWidth = 0.01f;
//         lineRenderer.endWidth = 0.01f;

//         // Add a point to the line at the index tip's position
//         if (hand.GetJointPose(HandJointId.HandIndexTip, out Pose pose))
//         {
//             lineRenderer.positionCount = 1;
//             lineRenderer.SetPosition(0, pose.position);
            
//         }
//     }

//     void UpdateLine()
//     {
//         // Add a new point to the line at the index tip's position
//         if (isDrawing && hand.GetJointPose(HandJointId.HandIndexTip, out Pose pose))
//         {
//             lineRenderer.positionCount++;
//             lineRenderer.SetPosition(lineRenderer.positionCount - 1, pose.position);
//         }
//     }

//     void StopDrawing()
//     {
//         isDrawing = false;
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Unity.TubeRenderer;

public class HandDrawing : MonoBehaviour
{
    // Reference to the Hand component
    public Hand hand;

    // Reference to the TubeRenderer component
    private TubeRenderer tubeRenderer;
    private GameObject currentLine;

    // Reference to the material used for the line
    public Material leftLineMaterial;
    public Material rightLineMaterial;

    // Private fields to track drawing state
    private bool isDrawing = false;
    public bool isAutoDrawing = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Hand component
        hand = GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAutoDrawing = !isAutoDrawing;
            Debug.Log("Space Pressed");
        }

        // Get the position of the index tip
        if (hand.GetJointPose(HandJointId.HandIndexTip, out Pose pose))
        {
            // If auto drawing is enabled, add a new point to the tube
            if (isAutoDrawing)
            {
                if (isDrawing == false)
                {
                    // Create a new GameObject and add a TubeRenderer to it
                    currentLine = new GameObject();
                    currentLine.AddComponent<MeshRenderer>();
                    currentLine.AddComponent<MeshFilter>();

                    tubeRenderer = currentLine.AddComponent<TubeRenderer>();
                    tubeRenderer.SetPositions(new Vector3[0]);
                    tubeRenderer.startWidth = 0.01f;
                    tubeRenderer.endWidth = 0.01f;
                    isDrawing = true;
                    tubeRenderer.tubeMaterial = hand.Handedness == Handedness.Left ? leftLineMaterial : rightLineMaterial;
                    Debug.Log("Material" + tubeRenderer.tubeMaterial);
                    // Add a debug log
                    Debug.Log("TubeRenderer created");
                }

                // Add the new position to the TubeRenderer's positions array
                List<Vector3> positionsList = new List<Vector3>(tubeRenderer.positions);
                positionsList.Add(pose.position);
                tubeRenderer.SetPositions(positionsList.ToArray());

                // Add a debug log
                Debug.Log("Position added: " + pose.position);
            }
            // If not auto drawing, clear the tube
            else
            {
                if (isDrawing)
                {
                    isDrawing = false;
                }
            }
        }
    }
}
