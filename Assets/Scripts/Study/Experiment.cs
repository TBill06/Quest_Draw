using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;


public enum Surface { Physical, Virtual, None }
public enum DrawMethod { Pinch, Index, Controller }


public class Experiment : MonoBehaviour
{

    public GameObject instructions;

    // I just put the Setup scene here to start
    public Scene nextScene;

    void Start() {
        SetupNextCondition();
    }

    public void SetupNextCondition() {

        int id = PlayerPrefs.GetInt("pid");
        int conditionState = PlayerPrefs.GetInt("conditionState");

        // Tuple<DrawMethod, Surface> condition = GetOrdering(id)[conditionState];
        Tuple<DrawMethod, Surface> condition = new Tuple<DrawMethod, Surface>(DrawMethod.Pinch, Surface.None);

        switch (condition.Item1) {
            case (DrawMethod.Pinch):
            
                // Set to Pinch
                PlayerPrefs.SetInt("DrawMethod", (int) DrawMethod.Pinch);
                break;
            
            case (DrawMethod.Index):
            
                // Set to Index
                PlayerPrefs.SetInt("DrawMethod", (int) DrawMethod.Index);
                break;
            
            case (DrawMethod.Controller):

                // Set to Controller
                PlayerPrefs.SetInt("DrawMethod", (int) DrawMethod.Controller);
                break;
        }


        switch (condition.Item2) {
            case (Surface.Physical):

                // Set "nextScene" to the Physical Surface Scene
                break;
            
            case (Surface.Virtual):

                // Set "nextScene" to the Virtual Surface Scene
                break;
            
            case (Surface.None):

                // Set "nextScene" to the No Surface Scene
                break;
        }

        ShowInstructions();
    }


    void ShowInstructions() {
        instructions.SetActive(true);
    }


    // This is inefficient, I'll rewrite it
    public List<Tuple<DrawMethod, Surface>> GetOrdering(int pid)
    {

        int order = pid % 9;
        
        DrawMethod[] drawMethodOrder = new DrawMethod[3];
        Surface[] surfaceOrder = new Surface[3];

        switch (order)
        {
            case (0):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Pinch, DrawMethod.Index, DrawMethod.Controller};
                surfaceOrder =  new Surface[]{Surface.Physical, Surface.Virtual, Surface.None};
                break;

            case (1):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Index, DrawMethod.Controller, DrawMethod.Pinch};
                surfaceOrder = new Surface[]{Surface.Physical, Surface.Virtual, Surface.None};
                break;

            case (2):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Controller, DrawMethod.Pinch, DrawMethod.Index};
                surfaceOrder = new Surface[]{Surface.Physical, Surface.Virtual, Surface.None};
                break;
            
            //------------------------
           
            case (3):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Pinch, DrawMethod.Index, DrawMethod.Controller};
                surfaceOrder = new Surface[]{Surface.None, Surface.Physical, Surface.Virtual};
                break;

            case (4):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Index, DrawMethod.Controller, DrawMethod.Pinch};
                surfaceOrder = new Surface[]{Surface.None, Surface.Physical, Surface.Virtual};
                break;
                
            case (5):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Controller, DrawMethod.Pinch, DrawMethod.Index};
                surfaceOrder = new Surface[]{Surface.None, Surface.Physical, Surface.Virtual};
                break;
            
            //------------------------
           
            case (6):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Pinch, DrawMethod.Index, DrawMethod.Controller};
                surfaceOrder = new Surface[]{Surface.Virtual, Surface.None, Surface.Physical};
                break;

            case (7):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Index, DrawMethod.Controller, DrawMethod.Pinch};
                surfaceOrder = new Surface[]{Surface.Virtual, Surface.None, Surface.Physical};
                break;
                
            case (8):
                drawMethodOrder = new DrawMethod[]{DrawMethod.Controller, DrawMethod.Pinch, DrawMethod.Index};
                surfaceOrder = new Surface[]{Surface.Virtual, Surface.None, Surface.Physical};
                break;

        }

        List<Tuple<DrawMethod, Surface>> conditions = new List<Tuple<DrawMethod, Surface>>();

        foreach (DrawMethod dM in drawMethodOrder) {
            foreach (Surface s in surfaceOrder) {
                conditions.Add(new Tuple<DrawMethod, Surface>(dM, s));
            }
        }

        return conditions;
    }
}
