using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public static Action EventEnableCutsceneCamera;
    public static Action EventDisableCutsceneCamera;
    public static Action EventEnableTitleCamera;
    public static Action EventDisableTitleCamera;

    [SerializeField] Vector3 normalCameraOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] float targetChangeLerp = 0.1f;
    [SerializeField] float xSensitivity = 1.5f;
    [SerializeField] float ySensitivity = 1.0f;

    public ThirdPersonCamera.CameraController cameraController;
    public ThirdPersonCamera.FreeForm freeForm;
    public ThirdPersonCamera.CameraInputSampling_FreeForm cameraInput;
    public ThirdPersonCamera.LockOnTarget lockOnTarget;

    private CutsceneCamera cutsceneCamera;
    private TitleScreenCamera titleScreenCamera;
    MiniGameCamera miniGameCamera;
    public Vector3 cameraOffsetTarget;

    // Start is called before the first frame update
    void Awake()
    {
        cameraController = GetComponent<ThirdPersonCamera.CameraController>();
        freeForm = GetComponent<ThirdPersonCamera.FreeForm>();
        cameraInput = GetComponent<ThirdPersonCamera.CameraInputSampling_FreeForm>();
        lockOnTarget = GetComponent<ThirdPersonCamera.LockOnTarget>();
        cutsceneCamera = GetComponent<CutsceneCamera>();
        titleScreenCamera = GetComponent<TitleScreenCamera>();
        miniGameCamera = GetComponent<MiniGameCamera>();

        cameraInput.mouseSensitivity.x = xSensitivity;
        cameraInput.mouseSensitivity.y = ySensitivity;

        UIActions.EventPauseGame += DisableCamera;
        UIActions.EventResumeGame += EnableCamera;
        UIActions.EventUnlockCursor += DisableCamera;
        UIActions.EventLockCursor += EnableCamera;
        UIActions.EventEnterTextboxCamera += EnterTextboxCamera;
        UIActions.EventExitTextboxCamera += ExitTextboxCamera;
        UIActions.EventStartGame += EnableMainMenuCamera;

        cameraOffsetTarget = normalCameraOffset;

        EventEnableCutsceneCamera += EnableCutsceneCamera;
        EventDisableCutsceneCamera += DisableCutsceneCamera;
        EventEnableTitleCamera += EnableTitleCamera;
        EventDisableTitleCamera += DisableTitleCamera;
    }

    void OnDestroy()
    {
        UIActions.EventPauseGame -= DisableCamera;
        UIActions.EventResumeGame -= EnableCamera;
        UIActions.EventUnlockCursor -= DisableCamera;
        UIActions.EventLockCursor -= EnableCamera;
        UIActions.EventEnterTextboxCamera -= EnterTextboxCamera;
        UIActions.EventExitTextboxCamera -= ExitTextboxCamera;
        UIActions.EventStartGame -= EnableMainMenuCamera;

        EventEnableCutsceneCamera -= EnableCutsceneCamera;
        EventDisableCutsceneCamera -= DisableCutsceneCamera;
        EventEnableTitleCamera -= EnableTitleCamera;
        EventDisableTitleCamera -= DisableTitleCamera;
    }

    private void Update()
    {
        cameraController.offsetVector = Vector3.MoveTowards(cameraController.offsetVector, cameraOffsetTarget, Time.deltaTime * targetChangeLerp);
    }

    void DisableCamera()
    {
        cameraInput.enabled = false;
        freeForm.stationaryModeHorizontal = ThirdPersonCamera.StationaryModeType.Fixed;
        freeForm.stationaryModeVertical = ThirdPersonCamera.StationaryModeType.Fixed;
    }

    void EnableCamera()
    {
        cameraInput.enabled = true;
        freeForm.stationaryModeHorizontal = ThirdPersonCamera.StationaryModeType.Free;
        freeForm.stationaryModeVertical = ThirdPersonCamera.StationaryModeType.Free;
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
    public void EnableMiniGame()
    {
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
}
