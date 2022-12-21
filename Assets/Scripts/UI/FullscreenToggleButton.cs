using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggleButton : MonoBehaviour
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

        toggle.isOn = Screen.fullScreen;
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreenMode = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}
