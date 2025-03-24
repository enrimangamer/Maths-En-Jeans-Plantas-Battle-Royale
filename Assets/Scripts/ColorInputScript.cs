using UnityEngine;

public class ColorInputScript : MonoBehaviour
{
    // Drag & drop slider
    public UnityEngine.UI.Slider slider ;

// Drag & drop handle
    public UnityEngine.UI.Image handle ;

// Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        handle.color = Color.HSVToRGB(slider.value, 1, 1);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
}
