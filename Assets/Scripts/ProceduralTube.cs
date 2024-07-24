using System.Collections.Generic;
using UnityEngine;

namespace Unity.ProceduralTube
{
    public class ProceduralTube : MonoBehaviour
    {
        [Min(0)]
        public float tubeRadius = 0.006f;
        [Min(1)]
        public int tubeSegments = 64;
        public Material material;

        private List<Vector3> points = new List<Vector3>();
        private List<Vector3> logPoints = new List<Vector3>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        private int tubeKidCount = 0;
        private GameObject lastTubeKid;
        private Vector3 lastPointAdded;

        private void Start()
        {
            CreateNewTubeKid();
        }

        public List<Vector3> Points => logPoints;

        public void AddPoint(Vector3 point)
        {
            logPoints.Add(point);
            points.Add(point);
            GenerateMesh();
        }

        private void GenerateMesh()
        {
            if (points.Count < 2) return;

            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            normals.Clear();

            float totalLength = 0f;
            float cumulativeDistance = 0f;

            for (int i = 1; i < points.Count; i++)
            {
                totalLength += Vector3.Distance(points[i - 1], points[i]);
            }

            // Frenet-Serret frame initialization
            Vector3 tangent = (points[1] - points[0]).normalized;
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
            if (normal == Vector3.zero)
            {
                normal = Vector3.Cross(tangent, Vector3.forward).normalized;
            }
            Vector3 binormal = Vector3.Cross(tangent, normal).normalized;

            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    Vector3 nextTangent = (points[i + 1] - points[i]).normalized;
                    Vector3 rotationAxis = Vector3.Cross(tangent, nextTangent);
                    float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(tangent, nextTangent), -1f, 1f)) * Mathf.Rad2Deg;
                    Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
                    normal = rotation * normal;
                    binormal = Vector3.Cross(nextTangent, normal).normalized;
                    tangent = nextTangent;
                }

                for (int j = 0; j < tubeSegments; j++)
                {
                    float angle = (float)j / tubeSegments * Mathf.PI * 2;
                    Vector3 offset = normal * Mathf.Cos(angle) * tubeRadius + binormal * Mathf.Sin(angle) * tubeRadius;
                    vertices.Add(points[i] + offset);
                    normals.Add(offset.normalized);
                    uvs.Add(new Vector2((float)j / tubeSegments, cumulativeDistance / totalLength));
                }

                if (i < points.Count - 1)
                {
                    cumulativeDistance += Vector3.Distance(points[i], points[i + 1]);
                }

                if (i > 0)
                {
                    int baseIndex = (i - 1) * tubeSegments;
                    for (int j = 0; j < tubeSegments; j++)
                    {
                        int nextSegment = (j + 1) % tubeSegments;

                        triangles.Add(baseIndex + j);
                        triangles.Add(baseIndex + j + tubeSegments);
                        triangles.Add(baseIndex + nextSegment);

                        triangles.Add(baseIndex + nextSegment);
                        triangles.Add(baseIndex + j + tubeSegments);
                        triangles.Add(baseIndex + nextSegment + tubeSegments);
                    }
                }

                if (vertices.Count >= 30000)
                {
                    FinalizeCurrentTubeKidMesh(vertices, triangles, uvs, normals);
                    CreateNewTubeKid();

                    // Clear lists for the next TubeKid
                    vertices.Clear();
                    triangles.Clear();
                    uvs.Clear();
                    normals.Clear();

                    // Adjust points for continuity
                    points.Clear();
                    points.Insert(0, lastPointAdded);
                    break;
                }
                lastPointAdded = points[i];
            }

            FinalizeCurrentTubeKidMesh(vertices, triangles, uvs, normals);
        }

        private void CreateNewTubeKid()
        {
            GameObject tubeObject = new GameObject("TubeKid" + tubeKidCount);
            tubeObject.transform.parent = transform;

            // Add MeshFilter and MeshRenderer
            MeshFilter meshFilter = tubeObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = tubeObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            // Create a new mesh for the TubeKid
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            tubeKidCount++;
            lastTubeKid = tubeObject;
        }

        private void FinalizeCurrentTubeKidMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals)
        {
            if (lastTubeKid == null) return;

            Mesh mesh = lastTubeKid.GetComponent<MeshFilter>().sharedMesh;
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }
}
