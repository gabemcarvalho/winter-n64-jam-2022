using UnityEngine;
using UnityEngine.UI;

public class InvertXToggleButton : MonoBehaviour
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

        CameraController.EventInvertX?.Invoke(toggle.isOn);
    }

    public void SetInvert(bool invert)
    {
        CameraController.EventInvertX?.Invoke(invert);
    }
}
