using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;


public class Setup : MonoBehaviour
{
    public TMP_InputField pidTextField;
    public TMP_Text handToUse;


    public bool isLeft;


    public void StartExperiment() {
    
        int id = int.Parse(pidTextField.text);

        PlayerPrefs.SetInt("pid", id);
        PlayerPrefs.SetInt("left", isLeft ? 1 : 0);

        PlayerPrefs.SetInt("block", 0);
        PlayerPrefs.SetInt("conditionState", 0);

        SceneManager.LoadScene("BetweenConditions");
    }


    public void SwitchHand() {
        isLeft = !isLeft;
        if (isLeft)
            handToUse.text = "Hand: Left";
        else
            handToUse.text = "Hand: Right";
    }
}