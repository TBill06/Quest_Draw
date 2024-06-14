using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class Test : MonoBehaviour
{
    // Reference to the Hand component
    public Hand hand;

    // Reference to the LineRenderer component
    // private LineRenderer lineRenderer;

    // // Reference to the current line being drawn
    // private GameObject currentLine;

    // // Reference to the material used for the line
    // public Material leftLineMaterial;
    // public Material rightLineMaterial;

    // Private fields to track drawing state
    private bool isDrawing = false;
    public bool isAutoDrawing = true;

    public float tubeRadius = 0.05f;
    public int tubeSegments = 16;
    public Material tubeMaterial;
    public float minDistance = 0.1f;
    private Mesh currentTubeMesh;
    private List<Vector3> points = new List<Vector3>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        // Get the Hand component
        hand = GetComponent<Hand>();

        // Initialize the mesh filter and renderer
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tubeMaterial;

        currentTubeMesh = new Mesh();
        meshFilter.mesh = currentTubeMesh;  

    }

    void Update()
    {
        if (hand.GetIndexFingerIsPinching())
        {
            if (!isDrawing)
            {
                StartDrawing();
            }
            UpdateLine();
        }
        else
        {
            StopDrawing();
            GenerateMesh();
        }
    }

    void StartDrawing()
    {
        // Set the drawing state to true
        isDrawing = true;
        // // Create a new GameObject for the line
        // GameObject lineObject = new GameObject("Line");
        
        // // Add a LineRenderer to the new GameObject
        // lineRenderer = lineObject.AddComponent<LineRenderer>();

        // // Set the material and other properties of the line
        // lineRenderer.material = hand.Handedness == Handedness.Left ? leftLineMaterial : rightLineMaterial;
        // lineRenderer.startWidth = 0.01f;
        // lineRenderer.endWidth = 0.01f;

        // // Add a point to the line at the index tip's position
        // if (hand.GetJointPose(HandJointId.HandIndexTip, out Pose pose))
        // {
        //     lineRenderer.positionCount = 1;
        //     lineRenderer.SetPosition(0, pose.position);
            
        // }
    }

    void UpdateLine()
    {
        // Add a new point to the line at the index tip's position
        // if (isDrawing && hand.GetJointPose(HandJointId.HandIndexTip, out Pose pose))
        // {
        //     lineRenderer.positionCount++;
        //     lineRenderer.SetPosition(lineRenderer.positionCount - 1, pose.position);
        // }
        if (isDrawing && hand.GetJointPose(HandJointId.HandIndexTip, out Pose pose))
        {
            AddPoint(pose.position);
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }

    void AddPoint(Vector3 point)
    {
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], point) > minDistance)
        {
            points.Add(point);
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        if (points.Count < 2)
        {
            return;
        }

        currentTubeMesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            Vector3 forward = (i == points.Count - 1) ? points[i] - points[i-1] : points[i+1] - points[i];
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            if (right == Vector3.zero)
            {
                right = Vector3.Cross(Vector3.forward, forward).normalized;
            }
            Vector3 up = Vector3.Cross(forward, right).normalized;

            for (int j = 0; j < tubeSegments; j++)
            {
                float angle = (float)j / tubeSegments * Mathf.PI * 2;
                Vector3 offset = right * Mathf.Cos(angle) * tubeRadius + up * Mathf.Sin(angle) * tubeRadius;
                vertices.Add(point + offset);
                normals.Add(offset.normalized);
                uvs.Add(new Vector2((float)j / tubeSegments, (float)i / points.Count));
            }

            if (i > 0)
            {
                int baseIndex = vertices.Count - tubeSegments * 2;
                for (int j = 0; j < tubeSegments; j++)
                {
                    int nextSegment = (j + 1) % tubeSegments;

                    triangles.Add(baseIndex + j);
                    triangles.Add(baseIndex + nextSegment);
                    triangles.Add(baseIndex + tubeSegments + j);

                    triangles.Add(baseIndex + nextSegment);
                    triangles.Add(baseIndex + tubeSegments + nextSegment);
                    triangles.Add(baseIndex + tubeSegments + j);
                }
            }
        }

        currentTubeMesh.vertices = vertices.ToArray();
        currentTubeMesh.triangles = triangles.ToArray();
        currentTubeMesh.uv = uvs.ToArray();
        currentTubeMesh.normals = normals.ToArray();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Debug.Log("Space Pressed");
    //         isAutoDrawing = !isAutoDrawing;
    //     }

    //     if (isAutoDrawing)
    //     {
    //         AutoDraw();
    //     }
    //     else
    //     {
    //         Debug.Log("Manual Drawing");
    //         ManualDraw();
    //     }
    // }

    // void AutoDraw()
    // {
    //     // Check if the index finger is pinching
    //     if (hand.GetIndexFingerIsPinching())
    //     {
    //         // If we're not currently drawing, start a new line
    //         if (!isDrawing)
    //         {
    //             StartDrawing();
    //         }
    //         // Add a point to the line at the index tip's position
    //         UpdateLine();
    //     }
    //     else
    //     {
    //         StopDrawing();
    //     } 
    // }

    // void ManualDraw()
    // {
    //     // If we're not currently drawing, start a new line
    //     if (!isDrawing)
    //     {
    //         StartDrawing();
    //     }
    //     // If we were drawing, stop drawing
    //     else
    //     {
    //         StopDrawing();
    //     }
    // }
}