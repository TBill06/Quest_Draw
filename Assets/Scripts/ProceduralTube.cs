using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace Unity.ProceduralTube
{
    [RequireComponent(typeof(MeshFilter))]
    public class ProceduralTube : MonoBehaviour
    {
        [Min(0)]
        public float tubeRadius = 0.009f;
        [Min(1)]
        public int tubeSegments = 64;
        [Min(0)]
        public float minDistance = 0.009f;

        private Mesh currentTubeMesh;
        private List<Vector3> points = new List<Vector3>();
        private MeshFilter meshFilter;
        private int lastUpdate = 0;
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Vector3> normals = new List<Vector3>();

        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (currentTubeMesh == null) currentTubeMesh = new Mesh();
            meshFilter.mesh = currentTubeMesh;
            lastUpdate = PropHashCode();
        }

        public void AddPoint(Vector3 point)
        {
            // if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], point) > minDistance)
            // {
            //     points.Add(point);
            //     Debug.Log("AddPoint: " + point);
            //     Debug.Log("points.Count Tubeside: " + points.Count);
            //     GenerateMesh();
            // }
            points.Add(point);
            GenerateMesh();
            Debug.Log("AddPoint: " + point);
        }

        private int PropHashCode()
        {
            return points.Aggregate(0, (total, it) => total ^ it.GetHashCode()) ^ tubeRadius.GetHashCode() ^ tubeSegments.GetHashCode() ^ minDistance.GetHashCode();
        }

        private void GenerateMesh()
        {
            if (points.Count < 2)
            {
                return;
            }

            float totalLength = 0f;
            float cumulativeDistance = 0f;
            currentTubeMesh.Clear();
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            normals.Clear();

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
            }

            currentTubeMesh.vertices = vertices.ToArray();
            currentTubeMesh.triangles = triangles.ToArray();
            currentTubeMesh.uv = uvs.ToArray();
            currentTubeMesh.normals = normals.ToArray();
            meshFilter.mesh = currentTubeMesh;
        }

        private void LateUpdate()
        {
            if (lastUpdate != PropHashCode())
            {
                GenerateMesh();
            }
        }
    }
}
