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
        if (status != Status.Idle) {
            if (strokeManager.HasNext()) {
                UpdateStatus();
            } else {
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

                if (timeRemaining < 0) {
                    status = Status.ShowStroke;
                    strokeManager.ShowStroke();
                    timeRemaining = timeToShowStroke;
                }
            
                break;

        }

        timeRemaining -= Time.deltaTime;
    }

    // TODO: Use Tushar this for initializing the drawing vs. stopping
    // Will probably need public getters and setters in your scripts
    bool IsDrawing() {

        bool isDrawing = false;

        // something like this or a switch
        // if (drawMethod == DrawMethod.Pinch) { 
        //     if (surface == Surface.None) {
        //         // couldn't access this because it's private
        //         // draw = GetComponent<PinchDrawingV2>().isDrawing;
        //     }
        // }

        // Delete this when done
        isDrawing = hand.GetIndexFingerIsPinching();

        return isDrawing;
    }

    // Actually starts the block
    public void StartBlock() {

        startBlock.SetActive(false);
        GetComponent<PinchDrawingV2>().enabled = true;
        status = Status.BlankBeforeShowStroke;

    }

    // Shows a prompt at the start of a block
    public void ReadyForNextBlock() {

        int block = PlayerPrefs.GetInt("block");

        TMPro.TextMeshProUGUI topText = startBlock.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        topText.text = "Draw Method: " + drawMethod +
                              "\nSurface: " + surface + 
                              "\nBlock: " + block;

        startBlock.SetActive(true);
        GetComponent<PinchDrawingV2>().enabled = false;
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

}
