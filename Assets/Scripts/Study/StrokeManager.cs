using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.ProceduralTube;



public class StrokeManager : MonoBehaviour
{


    public List<Stroke> strokes;
    
    private int[] order = {0,1,2,3,4,5,6,7,8,9,10,11};
    private int position = 0;


    public void NextStroke() {
        position++;
    }

    public bool HasNext() {
        return position < order.Length;
    }

    public void ShowStroke() {
        strokes[order[position]].gameObject.SetActive(true);
    }

    public void HideStroke() {
        strokes[order[position]].gameObject.SetActive(false);
    }

    public void ResetOrder() {

        for (int i = order.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = order[i];
            order[i] = order[j];
            order[j] = temp;
        }
        position = 0;
    }



}
