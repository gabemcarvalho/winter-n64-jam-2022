using UnityEngine;
using UnityEngine.UI;

public class InvertYToggleButton : MonoBehaviour
{
    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        UpdateToggleToCurrent();
    }

    private void OnEnable()
    {
        UpdateToggleToCurrent();
    }

    private void UpdateToggleToCurrent()
    {
        if (toggle == null) return;

        CameraController.EventInvertY?.Invoke(toggle.isOn);
    }

    public void SetInvert(bool invert)
    {
        CameraController.EventInvertY?.Invoke(invert);
    }
}
