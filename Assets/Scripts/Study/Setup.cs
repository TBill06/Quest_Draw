using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;


// Enum for which setup state we are in
enum SetupState { Block, Condition, Pid };

public class Setup : MonoBehaviour
{
    // Input fields for PID, condition, and block
    public TMP_InputField textField;

    // Text to display which hand to use
    public TMP_Text handToUse;

    // Dialog game objects for PID, condition, and block
    public GameObject dialogPID;
    public GameObject dialogCondition;
    public GameObject dialogBlock;

    // Boolean to check if the left hand is used
    public bool isLeft;

    // Initial setup state is PID
    SetupState state = SetupState.Pid;

    // Method to start the experiment
    public void StartExperiment() {

        // Get handedness
        PlayerPrefs.SetInt("left", isLeft ? 1 : 0);

        // Load the next scene
        SceneManager.LoadScene("BetweenConditions");
    }


    // Method to switch the handedneess
    public void SwitchHand() {

        isLeft = !isLeft;
        handToUse.text = isLeft ? "Hand: Left" : "Hand: Right";
    }

    // Method to handle entering the next setup state
    public void Enter() {
        
        switch (state) {

            case (SetupState.Pid):

                SetupPID();
                
                // Switch from PID dialog to Condition dialog
                dialogPID.SetActive(false);
                dialogCondition.SetActive(true);
                state = SetupState.Condition;
                
                break;

            case (SetupState.Condition):                
                
                SetupCondition();

                // Switch from Condition dialog to Block dialog
                dialogCondition.SetActive(false);
                dialogBlock.SetActive(true);
                state = SetupState.Block;
                
                break;

            case (SetupState.Block):

                SetupBlock();
                
                // Switch from Block dialog to starting the experiment
                dialogBlock.SetActive(false);
                StartExperiment();

                break;
        }
    }

    // Setup the PID
    private void SetupPID() {
        // Parse the PID from the input field
        // Do not want to default here, this will return an error + crash
        int id = int.Parse(textField.text);
        textField.text = "";
        // Store the PID and handedness in PlayerPrefs
        PlayerPrefs.SetInt("pid", id);
    }

    // Setup the Condition
    private void SetupCondition() {
        // Parse/store the condition number (default 0)
        int condition = ParseInput(textField.text);
        textField.text = "";
        PlayerPrefs.SetInt("conditionState", condition);
    }

    // Setup the Block
    private void SetupBlock() {
        // Parse/store the block number (default 0)
        int block = ParseInput(textField.text);
        textField.text = "";
        PlayerPrefs.SetInt("block", block);
    }

    // For trying to parse the input field
    // If there is nothing entered, value is 0 (start)
    public int ParseInput(string input) {
        int value;
        return int.TryParse(input, out value) ? value : 0;
    }
}