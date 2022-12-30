using UnityEngine;
using UnityEngine.UI;

public class CameraSensitivitySlider : MonoBehaviour
{
    public void Start()
    {
        float value = GetComponent<Slider>().value;
        CameraController.EventSetCameraSensitivity?.Invoke(value);
    }

    public void SetSensitivity(float sliderValue)
    {
        CameraController.EventSetCameraSensitivity?.Invoke(sliderValue);
    }
}
