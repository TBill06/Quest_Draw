using UnityEngine;
using Unity.TubeRenderer;
using Oculus;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;

public class TubeGenerator : MonoBehaviour
{
    public Material lineMaterial;
    public OVRHand hand;

    private TubeRenderer currentTube;
    private List<Vector3> currentPoints;
    private Vector3 lastPosition;
    private float positionThreshold = 0.01f;

    void Start()
    {
        hand = GetComponent<OVRHand>();
        currentPoints = new List<Vector3>();
    }

    void Update()
    {
        if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (currentTube == null)
            {
                StartDrawing();
            }
            UpdateTrail();
        }
        else
        {
            StopDrawing();
        }
    }

    void StartDrawing()
    {
        GameObject tube = new GameObject("Tube");
        MeshRenderer meshRenderer = tube.AddComponent<MeshRenderer>();
        currentTube = tube.AddComponent<TubeRenderer>();
        meshRenderer.material = lineMaterial;

        currentTube.subdivisions = 3;
        currentTube.segments = 8;
        currentTube.startWidth = 0.006f;
        currentTube.endWidth = 0.006f;
        currentTube.showNodesInEditor = false;
        currentTube.uvScale = Vector2.one;
        currentTube.inside = false;

        currentPoints.Clear();
    }

    void UpdateTrail()
    {
        OVRSkeleton skeleton = hand.GetComponent<OVRSkeleton>();
        if (skeleton != null && skeleton.IsInitialized )
        {
            OVRBone indexFingerTip = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];
            Vector3 indexFingerTipPosition = indexFingerTip.Transform.position;

            if (Vector3.Distance(lastPosition, indexFingerTipPosition) > positionThreshold)
            {
                currentPoints.Add(indexFingerTipPosition);
                currentTube.SetPositions(currentPoints.ToArray());
                lastPosition = indexFingerTipPosition;
            }
        }
    }

    void StopDrawing()
    {
        currentTube = null;
    }
}