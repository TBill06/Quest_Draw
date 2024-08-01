using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using Oculus.Interaction.Input;


public enum Status { Idle, ShowStroke, BlankBeforeDraw, Drawing, BlankBeforeShowStroke }


public class TaskManager : MonoBehaviour
{
    public GameObject startBlock;

    public Hand rightHand;
    public Hand leftHand;
    
    private Hand hand;

    public int numberOfBlocks;
    public float timeToShowStroke;
    public float timeBreakBetweenDrawing;

    
    StrokeManager strokeManager;
    DrawMethod drawMethod;
    Surface surface;

    Status status;
    float timeRemaining;
    private Type activeScript;

    
    void Start() {

        int left = PlayerPrefs.GetInt("left");
        if (left == 1)
            hand = leftHand;
        else
            hand = rightHand;

        drawMethod = (DrawMethod) PlayerPrefs.GetInt("DrawMethod");
        surface = (Surface) PlayerPrefs.GetInt("Surface");

        strokeManager = GetComponent<StrokeManager>();

        ReadyForNextBlock();
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("Status: " + status);
        if (status != Status.Idle) {
            if (strokeManager.HasNext()) {
                UpdateStatus();
            } else {
                DeleteTubes();
                FinishBlock();
            }
        }
    }


    // Main workflow for going through a condition
    void UpdateStatus() {

        switch (status) {

            case (Status.BlankBeforeDraw):

                // Switch this based on what the condition is 
                // (i.e. controller, pinch, or index)
                // In the IsDrawing function
                LoadScripts(surface, drawMethod);
                if (IsDrawing()) {
                    status = Status.Drawing;
                }

                break;

            // Show stroke
            case (Status.ShowStroke):

                if (timeRemaining < 0) {
                    strokeManager.HideStroke();
                    status = Status.BlankBeforeDraw;
                }

                break;
            
            // When drawing
            case (Status.Drawing):

                if (!IsDrawing()) {
                    status = Status.BlankBeforeShowStroke;
                    strokeManager.NextStroke();
                    timeRemaining = timeBreakBetweenDrawing;
                }

                break;
            
            // After drawing
            case (Status.BlankBeforeShowStroke):

                OffloadScripts(surface, drawMethod);
                if (timeRemaining < 0) {
                    status = Status.ShowStroke;
                    DeleteTubes();
                    strokeManager.ShowStroke();
                    timeRemaining = timeToShowStroke;
                }
            
                break;

        }

        timeRemaining -= Time.deltaTime;
    }

    // Checks if the user is currently drawing
    bool IsDrawing() {

        bool isDrawing = false;

        if (activeScript != null)
        {
            var isDrawingProperty = activeScript.GetProperty("isDrawing");
            var component = GetComponent(activeScript);
            isDrawing = (bool)isDrawingProperty.GetValue(component);
        }

        return isDrawing;
    }

    // Actually starts the block
    public void StartBlock() {

        startBlock.SetActive(false);
        // LoadScripts(drawMethod);
        status = Status.BlankBeforeShowStroke;
        timeRemaining = timeBreakBetweenDrawing;

    }

    // Shows a prompt at the start of a block
    public void ReadyForNextBlock() {

        int block = PlayerPrefs.GetInt("block");

        TMPro.TextMeshProUGUI topText = startBlock.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        topText.text = "Draw Method: " + drawMethod +
                              "\nSurface: " + surface + 
                              "\nBlock: " + block;

        startBlock.SetActive(true);
        // OffloadScripts(drawMethod);
        status = Status.Idle;

        // Shuffles the stroke order
        strokeManager.ResetOrder();
    }

    // Finish a block of drawings
    void FinishBlock() {

        // Increment block number
        int block = PlayerPrefs.GetInt("block");
        block++;

        // If we have reached the end number of blocks
        if (block == numberOfBlocks) {
            int conditionState = PlayerPrefs.GetInt("conditionState");
            
            // Increment the condition state
            conditionState++;

            // Set block back to zero
            block = 0;

            // Set global condition state and block
            PlayerPrefs.SetInt("conditionState", conditionState);
            PlayerPrefs.SetInt("block", block);

            SceneManager.LoadScene("BetweenConditions");
            
        } else {

            // Otherwise, set global block
            PlayerPrefs.SetInt("block", block);

            // Shows the prompt at the start of a new block
            ReadyForNextBlock();
        }
    }

    void LoadScripts(Surface surface, DrawMethod drawMethod)
    {
        switch (surface)
        {
            case (Surface.None):
                switch (drawMethod)
                {
                    case (DrawMethod.Pinch):
                        GetComponent<PinchDrawingV2>().enabled = true;
                        activeScript = typeof(PinchDrawingV2);
                        break;

                    case (DrawMethod.Index):
                        GetComponent<PointDrawingV2>().enabled = true;
                        activeScript = typeof(PointDrawingV2);
                        break;

                    case (DrawMethod.Controller):
                        GetComponent<ControllerDrawing>().enabled = true;
                        activeScript = typeof(ControllerDrawing);
                        break;
                }
                break;
            
            case (Surface.Virtual):
            case (Surface.Physical):
                switch (drawMethod)
                {
                    case (DrawMethod.Pinch):
                        GetComponent<VSurfacePinchV2>().enabled = true;
                        activeScript = typeof(VSurfacePinchV2);
                        break;

                    case (DrawMethod.Index):
                        GetComponent<VSurfacePointV2>().enabled = true;
                        activeScript = typeof(VSurfacePointV2);
                        break;

                    case (DrawMethod.Controller):
                        GetComponent<VSurfaceControllerV2>().enabled = true;
                        activeScript = typeof(VSurfaceControllerV2);
                        break;
                }
                break;
        }
    }

    void OffloadScripts(Surface surface, DrawMethod drawMethod)
    {
        switch (surface)
        {
            case (Surface.None):
                switch (drawMethod)
                {
                    case (DrawMethod.Pinch):
                        GetComponent<PinchDrawingV2>().enabled = false;
                        break;

                    case (DrawMethod.Index):
                        GetComponent<PointDrawingV2>().enabled = false;
                        break;

                    case (DrawMethod.Controller):
                        GetComponent<ControllerDrawing>().enabled = false;
                        break;
                }
                break;
            
            case (Surface.Virtual):
            case (Surface.Physical):
                switch (drawMethod)
                {
                    case (DrawMethod.Pinch):
                        GetComponent<VSurfacePinchV2>().enabled = false;
                        break;

                    case (DrawMethod.Index):
                        GetComponent<VSurfacePointV2>().enabled = false;
                        break;

                    case (DrawMethod.Controller):
                        GetComponent<VSurfaceControllerV2>().enabled = false;
                        break;
                }
                break;
        }
    }

    void DeleteTubes()
    {
        Debug.Log("Deleting tubes");
        GameObject[] tubes = GameObject.FindGameObjectsWithTag("Tube");
        foreach (GameObject tube in tubes)
        {
            Destroy(tube);
        }
    }

}
