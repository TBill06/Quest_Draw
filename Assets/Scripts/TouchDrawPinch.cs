// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Oculus.Interaction.Input;
// using Oculus.Interaction;
// using Oculus.Interaction.Surfaces;
// using Meta.XR.MRUtilityKit;
// using Unity.ProceduralTube;
// using System.IO;
// using System.Text;

// public class TouchDrawPinch : MonoBehaviour
// {
//     public Hand hand;
//     public Material tubeMaterial;
//     public Material wallMaterial;
//     public AnchorPrefabSpawner anchorPrefabSpawner;
//     private bool isDrawing = false;
//     private bool wasPinching = false;
//     private GameObject quad;
//     private GameObject wall_art;
//     private Vector3 quadPosition;
//     private Vector3 quadSize;
//     private ProceduralTube currentTube;
//     private List<Vector3> points = new List<Vector3>();
//     private List<GameObject> tubes = new List<GameObject>();
//     private MeshRenderer meshRenderer;
//     private ColliderSurface colliderSurface;
//     private Vector3 minBoundary;
//     private Vector3 maxBoundary;
//     private float tubeRadius = 0.007f;

//     private IEnumerator WaitForAnchorPrefabSpawner()
//     {
//         while (anchorPrefabSpawner == null || anchorPrefabSpawner.AnchorPrefabSpawnerObjects.Count == 0) 
//         {
//             yield return null;
//         }
//         SetPrefabs();
//     }
//     void Start()
//     {
//         StartCoroutine(WaitForAnchorPrefabSpawner());
//     }

//     private void SetPrefabs()
//     {
//         var allSpawnerObjects = anchorPrefabSpawner.AnchorPrefabSpawnerObjects;
//         wall_art = GameObject.Find("WALL_ART");

//         MRUKAnchor parentAnchor = wall_art.GetComponent<MRUKAnchor>().ParentAnchor;
//         Debug.Log("Parent Anchor: "+parentAnchor.transform.position+parentAnchor.transform.rotation.eulerAngles+parentAnchor.transform.localScale);
//         Debug.Log("Wall Art Position: "+wall_art.transform.position+wall_art.transform.rotation.eulerAngles+wall_art.transform.localScale);
//         // float parentAnchorPositionZ = parentAnchor.transform.position.z;
//         // float wallArtPositionZ = wall_art.transform.position.z;
//         // Debug.Log("Parent Anchor: "+parentAnchor.transform.position);
//         // Debug.Log("Wall Art Position Z: "+wallArtPositionZ);
//         // Vector3 parentAnchorRotation = parentAnchor.transform.rotation.eulerAngles;
//         // Vector3 wallArtRotation = wall_art.transform.rotation.eulerAngles;
//         // Debug.Log("Parent Anchor Rotation: "+parentAnchorRotation);
//         // Debug.Log("Wall Art Rotation: "+wallArtRotation);
//         // wall_art.transform.position = new Vector3(wall_art.transform.position.x, wall_art.transform.position.y, wall_art.transform.position.z);
//         // wall_art.transform.rotation = Quaternion.Euler(parentAnchorRotation);
//         // Debug.Log("New Wall Art Position Z: "+wall_art.transform.position.z);
//         // Debug.Log("New Wall Art Rotation: "+wall_art.transform.rotation.eulerAngles);

//         // wall_art.layer = 3;
//         // BoxCollider2D boxCollider = wall_art.AddComponent<BoxCollider2D>();
//         // boxCollider.size = new Vector2(1.85f, 1.23f);

//         foreach (var spawnerObject in allSpawnerObjects)
//         {
//             // MRUKAnchor anchor = spawnerObject.Key;
//             GameObject prefabSpawner = spawnerObject.Value;
//             if (prefabSpawner.name == "WallBoard(PrefabSpawner Clone)")
//             {
//                 Debug.Log("Prefab Spawner Transform: "+prefabSpawner.transform.position+ prefabSpawner.transform.rotation.eulerAngles+ prefabSpawner.transform.localScale);
//                 quad = prefabSpawner.transform.GetChild(0).gameObject;
//                 Debug.Log("Quad Transform: "+quad.transform.position+ quad.transform.rotation.eulerAngles+quad.transform.localScale);
//                 MeshFilter meshFilter = quad.GetComponent<MeshFilter>();
//                 Debug.Log("Mesh Filter Bounds: "+meshFilter.mesh.bounds.size+" "+meshFilter.mesh.bounds.center);
//                 BoxCollider boxCollider = quad.AddComponent<BoxCollider>();
//                 boxCollider.size = new Vector3(prefabSpawner.transform.localScale.x, prefabSpawner.transform.localScale.y, 0.01f);
//                 boxCollider.center = meshFilter.mesh.bounds.center;
//                 Debug.Log("Box Collider Size: "+boxCollider.size+" "+boxCollider.center);
//                 // DrawBoxColliderBoundary(boxCollider);
//                 // MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
//                 // //boxCollider.size = meshRenderer.bounds.size;
//                 // Debug.Log("Mesh Renderer Bounds: "+meshRenderer.bounds.size+" "+meshRenderer.bounds.center);
//                 // Debug.Log("Box Collider Size: "+boxCollider.size+" "+boxCollider.center);
//                 // // Vector3 adjustedSize = new Vector3(
//                 // //     meshRenderer.bounds.size.x / prefabSpawner.transform.localScale.x,
//                 // //     meshRenderer.bounds.size.y / prefabSpawner.transform.localScale.y,
//                 // //     meshRenderer.bounds.size.z / prefabSpawner.transform.localScale.z
//                 // // );
//                 // Vector3 adjustedSize = new Vector3(1.83f, 1.23f, 0.01f);
//                 // boxCollider.size = new Vector3(adjustedSize.x, adjustedSize.y, 0.01f);
//                 // boxCollider.center = meshRenderer.bounds.center;
//                 // Debug.Log("Adjusted Box Collider Size: "+boxCollider.size+" "+boxCollider.center);

//                 // // Assuming this code is part of a method in the TouchDrawPinch.cs script
//                 // LineRenderer lineRenderer = quad.GetComponent<LineRenderer>();
//                 // if (lineRenderer == null)
//                 // {
//                 //     lineRenderer = quad.AddComponent<LineRenderer>();
//                 // }

//                 // // Configure the LineRenderer
//                 // lineRenderer.startWidth = 0.02f;
//                 // lineRenderer.endWidth = 0.02f;
//                 // lineRenderer.useWorldSpace = false; // Use local space to make it relative to the quad

//                 // // Set the material and color as needed
//                 // // lineRenderer.material = ...
//                 // lineRenderer.material = tubeMaterial;

//                 // // Calculate the corners of the BoxCollider
//                 // Vector3 center = boxCollider.center;
//                 // Vector3 size = boxCollider.size;
//                 // Vector3[] corners = new Vector3[8];
//                 // corners[0] = center + new Vector3(-size.x, -size.y, -size.z) *0.1f;
//                 // corners[1] = center + new Vector3(size.x, -size.y, -size.z) *0.1f;
//                 // corners[2] = center + new Vector3(size.x, -size.y, size.z) *0.1f;
//                 // corners[3] = center + new Vector3(-size.x, -size.y, size.z) *0.1f;
//                 // corners[4] = center + new Vector3(-size.x, size.y, -size.z) *0.1f;
//                 // corners[5] = center + new Vector3(size.x, size.y, -size.z) *0.1f;
//                 // corners[6] = center + new Vector3(size.x, size.y, size.z) *0.1f;
//                 // corners[7] = center + new Vector3(-size.x, size.y, size.z) *0.1f;

//                 // // Set the positions for the LineRenderer to draw the box
//                 // lineRenderer.positionCount = 24;
//                 // lineRenderer.SetPositions(new Vector3[]{
//                 //     corners[0], corners[1], corners[1], corners[2], corners[2], corners[3], corners[3], corners[0], // Bottom face
//                 //     corners[4], corners[5], corners[5], corners[6], corners[6], corners[7], corners[7], corners[4], // Top face
//                 //     corners[0], corners[4], corners[1], corners[5], corners[2], corners[6], corners[3], corners[7]  // Sides
//                 // });
//             }
//         }
//     }

//     // void DrawBoxColliderBoundary(BoxCollider boxCollider)
//     // {
//     //     // Calculate the four corner points of the BoxCollider
//     //     Vector3 colliderCenter = boxCollider.transform.TransformPoint(boxCollider.center);
//     //     float halfX = boxCollider.size.x * 0.5f * boxCollider.transform.localScale.x;
//     //     float halfY = boxCollider.size.y * 0.5f * boxCollider.transform.localScale.y;

//     //     Vector3[] corners = new Vector3[4];
//     //     corners[0] = colliderCenter + new Vector3(-halfX, -halfY, 0); // Bottom left
//     //     corners[1] = colliderCenter + new Vector3(halfX, -halfY, 0);  // Bottom right
//     //     corners[2] = colliderCenter + new Vector3(halfX, halfY, 0);   // Top right
//     //     corners[3] = colliderCenter + new Vector3(-halfX, halfY, 0);  // Top left

//     //     // Ensure there's a LineRenderer component and configure it
//     //     LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
//     //     if (lineRenderer == null)
//     //     {
//     //         lineRenderer = gameObject.AddComponent<LineRenderer>();
//     //     }

//     //     lineRenderer.positionCount = 5; // Four corners + first point again to close the loop
//     //     lineRenderer.loop = true; // Automatically connects the last point to the first

//     //     // Set the calculated points
//     //     for (int i = 0; i < 4; i++)
//     //     {
//     //         lineRenderer.SetPosition(i, corners[i]);
//     //     }
//     //     lineRenderer.SetPosition(4, corners[0]); // Close the loop

//     //     // Optional: Configure the appearance of the LineRenderer
//     //     lineRenderer.startWidth = 0.02f;
//     //     lineRenderer.endWidth = 0.02f;
//     //     lineRenderer.material = tubeMaterial;
//     //     // Set material and color as needed
//     // }

//     void Update()
//     {
//         bool isPinching = hand.GetIndexFingerIsPinching();
//         Pose pose1;
//         Pose pose2;
//         Pose indexBasePose;
//         Pose thumbBasePose;
//         hand.GetJointPose(HandJointId.HandIndexTip, out pose1);
//         hand.GetJointPose(HandJointId.HandIndex1, out indexBasePose);
//         hand.GetJointPose(HandJointId.HandThumbTip, out pose2);
//         hand.GetJointPose(HandJointId.HandThumb1, out thumbBasePose);

//         Vector3 indexDirection = (pose1.position - indexBasePose.position).normalized;
//         Vector3 thumbDirection = (pose2.position - thumbBasePose.position).normalized;
//         Vector3 drawpoint = (pose1.position + pose2.position) / 2;

//         Ray ray = new(drawpoint, indexDirection);
//         RaycastHit hit;
//         if (isPinching && Physics.Raycast(ray, out hit, 0.02f))
//         {
//             if (hit.collider != null)
//             {
//                 Debug.Log("Hit Point:"+hit.point);
//                 Debug.Log("Hit Collider:"+hit.collider.gameObject.name);
//                 if (!wasPinching || !isDrawing)
//                 {
//                     StartDrawing();
//                 }
//                 if (isDrawing)
//                 {
//                     UpdateLine(drawpoint, hit);
//                 }
//             }
//             else
//             {
//                 StopDrawing();
//             }
//         }
//         // if (colliderSurface !=null)
//         // {
//         //     if (isPinching && (Physics.Raycast(ray, out hit, 0.02f) || drawpoint.z < quadPosition.z))
//         //     {
//         //         Debug.Log("Hit Point:"+hit.point);
//         //         if (IsWithinBounds(drawpoint, minBoundary, maxBoundary))
//         //         {
//         //             Debug.Log("Within bounds");
//         //             if (!wasPinching || !isDrawing)
//         //             {
//         //                 StartDrawing();
//         //             }
//         //             if (isDrawing)
//         //             {
//         //                 UpdateLine(drawpoint, hit);
//         //             }
//         //         }
//         //     }
//         //     else
//         //     {
//         //         StopDrawing();
//         //     }
//         // }
//         wasPinching = isPinching;
//     }

//     void StartDrawing()
//     {   
//         Debug.Log("Start Drawing");
//         // Set the drawing state to true
//         isDrawing = true;
//         wasPinching = true;

//         GameObject tubeObject = new GameObject("Tube");
//         currentTube = tubeObject.AddComponent<ProceduralTube>();
//         meshRenderer = tubeObject.AddComponent<MeshRenderer>();
//         meshRenderer.material = tubeMaterial;

//         points = new List<Vector3>();
//         tubes.Add(tubeObject);
//     }
    
//     void UpdateLine(Vector2 drawpoint, RaycastHit hit)
//     {
//         Vector3 hitPoint;
//         // if (drawpoint.z < quadPosition.z)
//         // {
//         //     if (IsWithinBounds(drawpoint, minBoundary, maxBoundary))
//         //     {
//         //         Debug.Log("drawpoint.z < quadPosition.z: "+drawpoint.z+" "+quadPosition.z);
//         //         hitPoint = new Vector3(drawpoint.x, drawpoint.y, quadPosition.z+tubeRadius);
//         //         Debug.Log("Added point from the back: "+hitPoint);hit.point.z + (hit.point.z >= 0 ? -tubeRadius : +tubeRadius)
//         //     }
//         //     else
//         //     {
//         //         StopDrawing();
//         //         return;
//         //     }
//         // }
//         // else
//         // {
//         hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z + (hit.point.z >= 0 ? -0.1f : +0.1f));
//         Debug.Log("Added point from the front: "+hitPoint);
//         //}
//         currentTube.AddPoint(hitPoint);
//     }

//     void StopDrawing()
//     {
//         isDrawing = false;
//     }

//     bool IsWithinBounds(Vector3 point, Vector3 minBoundary, Vector3 maxBoundary)
//     {
//         return point.x >= minBoundary.x && point.x <= maxBoundary.x &&
//             point.y >= minBoundary.y && point.y <= maxBoundary.y;
//     }

//     // public void ContinueButtonClicked()
//     // {
//     //     using (StreamWriter sw = new StreamWriter("Assets/Logs/TouchDrawPinch.csv"))
//     //     {
//     //         StringBuilder sb = new StringBuilder();
//     //         foreach (GameObject tube in tubes)
//     //         {
//     //             ProceduralTube pt = tube.GetComponent<ProceduralTube>();
//     //             foreach (Vector3 point in pt.Points)
//     //             {
//     //                 string timestamp = System.DateTime.Now.ToString("o");
//     //                 sb.AppendLine($"{point.x},{point.y},{point.z},{timestamp}");
//     //             }
//     //         }
//     //         sw.Write(sb.ToString());

//     //     }

//     //     foreach (GameObject tube in tubes)
//     //     {
//     //         Destroy(tube);
//     //     }
//     //     tubes.Clear();
//     // }
// }