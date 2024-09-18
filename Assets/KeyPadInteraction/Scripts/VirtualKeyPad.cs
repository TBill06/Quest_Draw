using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class VirtualKeyPad : MonoBehaviour
{
	
	public static VirtualKeyPad instance;
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public TMP_InputField targetText;
	
	
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void KeyPress(string k){
		words += k;
		targetText.text = words;	
	}
	
	public void Backspace(){
		if (words != "")
		{
            words = words.Remove(words.Length - 1, 1);
            targetText.text = words;
        }
		
	}

	public void Enter() {
		words = "";
	}

	public void ShowVirtualKeyPad(){
		vkCanvas.SetActive(true);
	}
	
	public void HideVirtualKeyPad(){
		vkCanvas.SetActive(false);
	}
}
