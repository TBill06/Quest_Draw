using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using Oculus;

public class HandDrawingWithTrail : MonoBehaviour
{
    public OVRHand hand;
    public Material trailMaterial;
    public float trailWidth = 0.01f;
    private bool isDrawing = false;
    private TrailRenderer currentTrail;

    void Start()
    {
        hand = GetComponent<OVRHand>();
    }

    void Update()
    {
        if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (!isDrawing)
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
        isDrawing = true;
        currentTrail = CreateTrail();
    }

    TrailRenderer CreateTrail()
    {
        GameObject trailObject = new GameObject("Trail");
        trailObject.transform.SetParent(transform);
        TrailRenderer trailRenderer = trailObject.AddComponent<TrailRenderer>();
        trailRenderer.material = trailMaterial;
        trailRenderer.widthMultiplier = trailWidth;
        trailRenderer.time = Mathf.Infinity;  // Keeps the trail forever
        trailRenderer.startWidth = trailWidth;
        trailRenderer.endWidth = trailWidth;
        trailRenderer.minVertexDistance = 0.01f;  // Minimum distance for a new vertex
        trailRenderer.numCapVertices = 4;  // Round caps
        return trailRenderer;
    }

    void UpdateTrail()
    {
        if (!isDrawing) return;

        OVRSkeleton skeleton = hand.GetComponent<OVRSkeleton>();
        if (skeleton != null && skeleton.IsInitialized )
        {
            OVRBone indexFingerTip = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];
            Vector3 indexFingerTipPosition = indexFingerTip.Transform.position;
            OVRBone thumbTip = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_ThumbTip];
            Vector3 thumbTipPosition = thumbTip.Transform.position;
            Vector3 trailPosition = (indexFingerTipPosition + thumbTipPosition) / 2;
            Debug.Log("Trail position: " + trailPosition);
            Debug.Log("Index finger tip position: " + indexFingerTipPosition);
            currentTrail.transform.position = trailPosition;
        }
    }

    void StopDrawing()
    {
        if (!isDrawing) return;

        isDrawing = false;

        // Create a new GameObject and parent the current trail to it
        GameObject trailHolder = new GameObject("TrailHolder");
        currentTrail.transform.SetParent(trailHolder.transform);

        currentTrail = null;
    }
}