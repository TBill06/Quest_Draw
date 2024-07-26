using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Complexity { SIMPLE,  MEDIUM, COMPLEX }


public class Stroke : MonoBehaviour
{
    public string sName;
    public int num;
    public Complexity complexity;


    public string GetName() {
        return sName;
    }

   public Complexity GetComplexity() {
        return complexity;
   }

   public int GetNum()
    {
        return num;
    }
}
