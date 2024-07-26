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

    Condition condition;

    void Start() {

        SetupNextCondition();
    
    }

    // Setup the next condition, will be run at the start of "BetweenConditions" scene
    public void SetupNextCondition() {

        int id = PlayerPrefs.GetInt("pid");
        int conditionState = PlayerPrefs.GetInt("conditionState");

        for (int i = 0; i < 9; i++) {

            GetOrdering(i);
        }
        
        condition = GetOrdering(id)[conditionState];
        
        PlayerPrefs.SetInt("DrawMethod", (int) condition.DrawMethod);
        PlayerPrefs.SetInt("Surface", (int) condition.Surface);
       
        instructions.SetActive(true);
    }


    // Start the next condition
    public void StartNextCondition() {

        switch (condition.Surface) {
            case (Surface.Physical):

                // Load the next condition scene: Physical Surface Draw
                SceneManager.LoadScene("PhysicalSurfaceDraw-C3-Colliders");
                break;
            
            case (Surface.Virtual):

                // Load the next condition scene: Virtual Surface Draw
                SceneManager.LoadScene("VirtualSurfaceDraw-C2-Colliders");
                break;
            
            case (Surface.None):

                // Load the next condition scene: No Surface Draw
                SceneManager.LoadScene("NoSurfaceDraw-C1");
                break;
        }

    }


    // Get the ordering of conditions based on participant ID
    Condition[] GetOrdering(int pid) {

        int order = pid % 9;
        
        Condition[] conditions = new Condition[9];

        switch (order) {
            
            case (0):

                conditions = new Condition[]{new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None)};

                break;

            case (1):

                conditions = new Condition[]{new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None)};

                break;

            case (2):

                conditions = new Condition[]{new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None)};

                break;

            case (3):

                conditions = new Condition[]{new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual)};

                break;

            case (4):

                conditions = new Condition[]{new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual)};

                break;

            case (5):

                conditions = new Condition[]{new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual)};

                break;

            case (6):

                conditions = new Condition[]{new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical)};

                break;

            case (7):

                conditions = new Condition[]{new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Index, Surface.Physical), new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical)};

                break;

            case (8):

                conditions = new Condition[]{new Condition(DrawMethod.Controller, Surface.Virtual), new Condition(DrawMethod.Controller, Surface.None), new Condition(DrawMethod.Controller, Surface.Physical), new Condition(DrawMethod.Pinch, Surface.Virtual), new Condition(DrawMethod.Pinch, Surface.None), new Condition(DrawMethod.Pinch, Surface.Physical), new Condition(DrawMethod.Index, Surface.Virtual), new Condition(DrawMethod.Index, Surface.None), new Condition(DrawMethod.Index, Surface.Physical)};

                break;
        }

        return conditions;
    }
}


// Condition class to hold "DrawMethod and Surface"
public class Condition {

    DrawMethod drawMethod;
    Surface surface;

    public DrawMethod DrawMethod { // This is a property.
        get {
            return drawMethod;
        }
    }

    public Surface Surface { // This is a property.
        get {
            return surface;
        }
    }

    public Condition(DrawMethod dM, Surface s) {
        drawMethod = dM;
        surface = s;
    }

}