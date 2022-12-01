using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float xSensitivity = 1.5f;
    [SerializeField] float ySensitivity = 1.0f;

    private ThirdPersonCamera.FreeForm freeForm;
    private ThirdPersonCamera.CameraInputSampling_FreeForm cameraInput;

    // Start is called before the first frame update
    void Awake()
    {
        freeForm = GetComponent<ThirdPersonCamera.FreeForm>();
        cameraInput = GetComponent<ThirdPersonCamera.CameraInputSampling_FreeForm>();
        cameraInput.mouseSensitivity.x = xSensitivity;
        cameraInput.mouseSensitivity.y = ySensitivity;

        UIActions.EventPauseGame += DisableCamera;
        UIActions.EventResumeGame += EnableCamera;
    }

    void OnDestroy()
    {
        UIActions.EventPauseGame -= DisableCamera;
        UIActions.EventResumeGame -= EnableCamera;
    }

    void DisableCamera()
    {
        cameraInput.enabled = false;
        freeForm.stationaryModeHorizontal = ThirdPersonCamera.StationaryModeType.Fixed;
        freeForm.stationaryModeVertical = ThirdPersonCamera.StationaryModeType.Fixed;
        Time.timeScale = 0;
    }

    void EnableCamera()
    {
        cameraInput.enabled = true;
        freeForm.stationaryModeHorizontal = ThirdPersonCamera.StationaryModeType.Free;
        freeForm.stationaryModeVertical = ThirdPersonCamera.StationaryModeType.Free;
        Time.timeScale = 1.0f;
    }
}
