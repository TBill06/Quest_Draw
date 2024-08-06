using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using Oculus.Interaction.Input;
using Unity.ProceduralTube;
using System.Linq;

public enum Status { Idle, ShowStroke, BlankBeforeDraw, Drawing, BlankBeforeShowStroke }

public static class ScriptManager
{
    public static bool shouldRun { get; set; } = false;
}

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
    int pid, block;

    Status status;
    float timeRemaining;
    private Type activeScript;
    private bool deleteTubes = true;

    // Start sets the hand, draw method, surface, and stroke manager
    void Start() {

        int left = PlayerPrefs.GetInt("left");
        if (left == 1)
            hand = leftHand;
        else
            hand = rightHand;

        pid = PlayerPrefs.GetInt("pid");
        drawMethod = (DrawMethod) PlayerPrefs.GetInt("DrawMethod");
        surface = (Surface) PlayerPrefs.GetInt("Surface");
        block = PlayerPrefs.GetInt("block");

        strokeManager = GetComponent<StrokeManager>();
        SaveData.SetPID(pid);
        SaveData.SetDrawMethod(drawMethod.ToString());
        SaveData.SetSurface(surface.ToString());
        SaveData.SetBlock(block);
        ReadyForNextBlock();
    }


    // Update is called once per frame
    void Update()
    {
        if (status != Status.Idle) {
            if (strokeManager.HasNext()) {
                UpdateStatus();
            } else {
                if (deleteTubes) DeleteTubes();
                FinishBlock();
            }
        }
        timeRemaining -= Time.deltaTime;
    }


    // Main workflow for going through a condition
    void UpdateStatus() {

        switch (status) {

            // Show stroke
            case (Status.ShowStroke):
                Debug.Log("1 -- Showing Stroke");

                if (timeRemaining < 0) {
                    strokeManager.HideStroke();
                    status = Status.BlankBeforeDraw;
                }

                break;

            case (Status.BlankBeforeDraw):
                Debug.Log("2 -- Blank Before Draw");

                // Switch this based on what the condition is 
                // (i.e. controller, pinch, or index)
                // In the IsDrawing function
                SaveData.SetTimeTrialStart();
                ScriptManager.shouldRun = true;
                if (IsDrawing()) {
                    status = Status.Drawing;
                    SaveData.SetTimeDrawStart();
                }

                break;
            
            // When drawing
            case (Status.Drawing):
                Debug.Log("3 -- Drawing");

                if (!IsDrawing()) {
                    status = Status.BlankBeforeShowStroke;
                    SaveData.SetTimeDrawEnd();
                    deleteTubes = true;
                    strokeManager.NextStroke();
                    timeRemaining = timeBreakBetweenDrawing;
                }

                break;
            
            // After drawing
            case (Status.BlankBeforeShowStroke):
                Debug.Log("4 -- Blank Before Show Stroke");

                ScriptManager.shouldRun = false;
                if (timeRemaining < 0) {
                    status = Status.ShowStroke;
                    if (deleteTubes) DeleteTubes();
                    strokeManager.ShowStroke();
                    timeRemaining = timeToShowStroke;
                }
            
                break;

        }
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
        LoadScripts(surface, drawMethod);
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
        status = Status.Idle;

        // Shuffles the stroke order
        strokeManager.ResetOrder();
        timeRemaining = timeBreakBetweenDrawing;
    }

    // Finish a block of drawings
    void FinishBlock() {

        if(timeRemaining < 0)
        {
            // Increment block number
            int block = PlayerPrefs.GetInt("block");
            block++;

            // If we have reached the end number of blocks
            if (block == numberOfBlocks) {

                // Offload the scripts
                OffloadScripts(surface, drawMethod);

                // Increment the condition state
                int conditionState = PlayerPrefs.GetInt("conditionState");
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
                SaveData.SetBlock(block);

                // Shows the prompt at the start of a new block
                ReadyForNextBlock();
            }
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
                        GetComponent<ControllerDrawingV2>().enabled = true;
                        activeScript = typeof(ControllerDrawingV2);
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
                        GetComponent<ControllerDrawingV2>().enabled = false;
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
        string tubePoints = "";
        GameObject[] tubes = GameObject.FindGameObjectsWithTag("Tube");
        foreach (GameObject tube in tubes)
        {
            var points = tube.GetComponent<ProceduralTube>().Points;
            tubePoints += string.Join(",", points.Select(p => p.ToString())) + ",";
            Destroy(tube);
        }
        SaveData.AddStroke(tubePoints);
        deleteTubes = false;
    }

}
