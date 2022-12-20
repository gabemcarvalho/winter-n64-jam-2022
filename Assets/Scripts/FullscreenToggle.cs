using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (Screen.fullScreen) Screen.fullScreenMode = FullScreenMode.Windowed;
            else Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }
}
