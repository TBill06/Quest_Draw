using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction.Input;
public class raydraw : MonoBehaviour
{
    private Texture2D texture;
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        texture = new Texture2D(Screen.width, Screen.height);
        rawImage.texture = texture;
    }

    void Update()
    {
        
    }

    void DrawOnTexture()
    {

    }
}