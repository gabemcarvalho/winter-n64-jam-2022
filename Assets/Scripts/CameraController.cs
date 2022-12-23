using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 normalCameraOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] float targetChangeLerp = 0.1f;
    [SerializeField] float xSensitivity = 1.5f;
    [SerializeField] float ySensitivity = 1.0f;

    public ThirdPersonCamera.CameraController cameraController;
    public ThirdPersonCamera.FreeForm freeForm;
    public ThirdPersonCamera.CameraInputSampling_FreeForm cameraInput;
    public ThirdPersonCamera.LockOnTarget lockOnTarget;

<<<<<<< Updated upstream
=======
    private CutsceneCamera cutsceneCamera;
    private TitleScreenCamera titleScreenCamera;
    public MiniGameCamera miniGameCamera;

    

>>>>>>> Stashed changes
    public Vector3 cameraOffsetTarget;

    // Start is called before the first frame update
    void Awake()
    {
        cameraController = GetComponent<ThirdPersonCamera.CameraController>();
        freeForm = GetComponent<ThirdPersonCamera.FreeForm>();
        cameraInput = GetComponent<ThirdPersonCamera.CameraInputSampling_FreeForm>();
        lockOnTarget = GetComponent<ThirdPersonCamera.LockOnTarget>();

        miniGameCamera = GetComponent<MiniGameCamera>();

        cameraInput.mouseSensitivity.x = xSensitivity;
        cameraInput.mouseSensitivity.y = ySensitivity;

        UIActions.EventPauseGame += DisableCamera;
        UIActions.EventResumeGame += EnableCamera;
        UIActions.EventUnlockCursor += DisableCamera;
        UIActions.EventLockCursor += EnableCamera;
        UIActions.EventEnterTextboxCamera += EnterTextboxCamera;
        UIActions.EventExitTextboxCamera += ExitTextboxCamera;

        cameraOffsetTarget = normalCameraOffset;
    }

    void OnDestroy()
    {
        UIActions.EventPauseGame -= DisableCamera;
        UIActions.EventResumeGame -= EnableCamera;
        UIActions.EventUnlockCursor -= DisableCamera;
        UIActions.EventLockCursor -= EnableCamera;
        UIActions.EventEnterTextboxCamera -= EnterTextboxCamera;
        UIActions.EventExitTextboxCamera -= ExitTextboxCamera;
    }

    private void Update()
    {
        cameraController.offsetVector = Vector3.MoveTowards(cameraController.offsetVector, cameraOffsetTarget, Time.deltaTime * targetChangeLerp);
        //cameraController.offsetVector = cameraOffsetTarget;
    }

    void DisableCamera()
    {
        cameraInput.enabled = false;
        freeForm.stationaryModeHorizontal = ThirdPersonCamera.StationaryModeType.Fixed;
        freeForm.stationaryModeVertical = ThirdPersonCamera.StationaryModeType.Fixed;
        //Time.timeScale = 0;
    }

    void EnableCamera()
    {
        cameraInput.enabled = true;
        freeForm.stationaryModeHorizontal = ThirdPersonCamera.StationaryModeType.Free;
        freeForm.stationaryModeVertical = ThirdPersonCamera.StationaryModeType.Free;
        //Time.timeScale = 1.0f;
    }

    void EnterTextboxCamera(ThirdPersonCamera.Targetable target)
    {
        freeForm.cameraEnabled = false;
        lockOnTarget.followTarget = target;

        Vector3 playerPos = cameraController.target.transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 lineOfSight = new Vector3(targetPos.x - playerPos.x, 0, targetPos.z - playerPos.z).normalized;

        cameraOffsetTarget = new Vector3(1.0f, 0.7f, 0.3f * lineOfSight.magnitude);
    }

    void ExitTextboxCamera(bool enableFreeFormCamera)
    {
        freeForm.cameraEnabled = enableFreeFormCamera;
        lockOnTarget.followTarget = null;
        cameraController.pivotRotation = Quaternion.identity;
        cameraOffsetTarget = normalCameraOffset;
    }
<<<<<<< Updated upstream
=======

    void EnableMainMenuCamera()
    {
        DisableCamera();
    }

    // This probably won't interfere with other camera events for this game
    void EnableCutsceneCamera()
    {
        cameraController.enabled = false;
        cutsceneCamera.enabled = true;
    }

    void DisableCutsceneCamera()
    {
        cameraController.enabled = true;
        cutsceneCamera.enabled = false;
    }

    void EnableTitleCamera()
    {
        cameraController.enabled = false;
        titleScreenCamera.enabled = true;
    }

    void DisableTitleCamera()
    {
        cameraController.enabled = true;
        titleScreenCamera.enabled = false;
    }

    public void EnableMiniGame() {
        cameraController.enabled = false;
        DisableCamera();
        miniGameCamera.enabled = true;

    }
    public void DisableMiniGame()
    {
        cameraController.enabled = true;
        EnableCamera();
        miniGameCamera.enabled = false;

    }
>>>>>>> Stashed changes
}
