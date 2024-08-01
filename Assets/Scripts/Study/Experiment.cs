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
       

        SetupInstructions(condition);
    }


    // Start the next condition
    public void StartNextCondition() {

        // Tushar: Uncomment this when ready
        switch (condition.Surface) {
            case (Surface.Physical):

                Debug.Log("Physical Surface");
                // Load the next condition scene: Physical Surface Draw
                SceneManager.LoadScene("PhysicalSurfaceDraw-C3-Colliders");
                break;
            
            case (Surface.Virtual):

                Debug.Log("Virtual Surface");
                // Load the next condition scene: Virtual Surface Draw
                SceneManager.LoadScene("VirtualSurfaceDraw-C2");
                break;
            
            case (Surface.None):

                Debug.Log("No Surface");
                // Load the next condition scene: No Surface Draw
                SceneManager.LoadScene("NoSurfaceDraw-C1");
                break;
        }

        // Delete this when finished
        // SceneManager.LoadScene("ExperimentTask");

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


    void SetupInstructions(Condition condition) {


        string textInstructions = "";


        switch (condition.DrawMethod) {
        
            case (DrawMethod.Pinch):

                textInstructions += "To begin drawing, touch your index finger and thumb together.\nWhen finished, release your thumb and index finger.";

                break;
            
            case (DrawMethod.Index):
                
                textInstructions += "Start with your index finger and thumb in an L shape.\nTo begin drawing, bring your thumb into your hand like a \"trigger\" while keeping your index finger extended.\nWhen finished, release your thumb to put your hand back into the L shape.";

                break;
            
            case (DrawMethod.Controller):

                textInstructions += "Hold the controller backwards.\nTo begin drawing, pull the \"trigger\" by pressing it with your thumb.\nWhen finished, release the trigger (button).";

                break;
        }

        textInstructions += "\n\n";

        switch (condition.Surface) {
        
            case (Surface.Physical):

                textInstructions += "You will be drawing on a physical surface.\nDraw while keeping contact with the physical surface.";

                break;
            
            case (Surface.Virtual):
                
                textInstructions += "You will be drawing on a virtual surface.\nDraw while keeping contact with the virtual surface.";

                break;
            
            case (Surface.None):
 
                textInstructions += "You will not be be drawing on a surface.\nDraw in free space in front of you.";

                break;
        }

        TMPro.TextMeshProUGUI topText = instructions.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        topText.text = textInstructions;
        instructions.SetActive(true);
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