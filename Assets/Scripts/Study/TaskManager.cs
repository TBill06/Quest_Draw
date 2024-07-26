using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using Oculus.Interaction.Input;


public enum Status { Idle, ShowStroke, BlankBeforeDraw, Drawing, BlankAfterDraw }


public class TaskManager : MonoBehaviour
{
    public GameObject startButton;
    public GameObject instructions;


    public Hand hand;

    public int numberOfBlocks;
    public float timeToShowStroke;
    public float timeBreakBetweenDrawing;
    
    StrokeManager strokeManager;
    Experiment experiment;
    

    Status status;
    float timeRemaining;

    
    void Start() {
        strokeManager = GetComponent<StrokeManager>();
        experiment = GetComponent<Experiment>();
        status = Status.Idle;
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


    void UpdateStatus() {

        switch (status) {

            case (Status.BlankBeforeDraw):

                if (hand.GetIndexFingerIsPinching()) {
                    status = Status.Drawing;
                }

                break;

            case (Status.ShowStroke):

                if (timeRemaining < 0) {
                    strokeManager.HideStroke();
                    status = Status.BlankBeforeDraw;
                }

                break;
            
            case (Status.Drawing):

                if (!hand.GetIndexFingerIsPinching()) {
                    status = Status.BlankAfterDraw;
                    timeRemaining = timeBreakBetweenDrawing;
                }

                break;
            
            case (Status.BlankAfterDraw):

                if (timeRemaining < 0) {
                    status = Status.ShowStroke;
                    strokeManager.NextStroke();
                    strokeManager.ShowStroke();
                    timeRemaining = timeToShowStroke;
                }
            
                break;

        }

        timeRemaining -= Time.deltaTime;
    }

    public void StartBlock()
    {
        startButton.SetActive(false);
        GetComponent<HandDrawingTest>().enabled = true;
        status = Status.BlankBeforeDraw;

    }

    public void ShowStartButton() {
        strokeManager.HideStroke();
        instructions.SetActive(false);
        startButton.SetActive(true);
    }

    void FinishBlock() {
        strokeManager.ResetOrder();
        int block = PlayerPrefs.GetInt("block");
        block++;

        if (block == numberOfBlocks) {
            int conditionState = PlayerPrefs.GetInt("conditionState");
            
            conditionState++;
            block = 0;

            PlayerPrefs.SetInt("conditionState", conditionState);
            PlayerPrefs.SetInt("block", block);

            experiment.SetupNextCondition();
            
        } else {
            PlayerPrefs.SetInt("block", block);
            ShowStartButton();
        }
    }

}
