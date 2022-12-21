using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public Texture2D mouseCursor;

    [SerializeField] Image reticleImage;

    Vector2 hotSpot = new Vector2(0, 0);
    CursorMode cursorMode = CursorMode.Auto;

    private void Awake()
    {
        Cursor.SetCursor(mouseCursor, hotSpot, cursorMode);
        Cursor.visible = false;
        reticleImage.gameObject.SetActive(false);

        UIActions.EventPauseGame += UnlockCursor;
        UIActions.EventResumeGame += LockCursor;
        UIActions.EventUnlockCursor += ShowReticle;
        UIActions.EventLockCursor += HideReticle;
        UIActions.EventStartGame += UnlockCursor;
    }

    private void OnDestroy()
    {
        UIActions.EventPauseGame -= UnlockCursor;
        UIActions.EventResumeGame -= LockCursor;
        UIActions.EventUnlockCursor -= ShowReticle;
        UIActions.EventLockCursor -= HideReticle;
        UIActions.EventStartGame -= UnlockCursor;
    }

    public void Update()
    {
        Vector2 screenCoords = PlayerController.GetGameCameraMousePosition();
        reticleImage.rectTransform.anchoredPosition = screenCoords;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        reticleImage.gameObject.SetActive(false);
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        reticleImage.gameObject.SetActive(false);
    }

    public void ShowReticle()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        reticleImage.gameObject.SetActive(true);
    }

    public void HideReticle()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        reticleImage.gameObject.SetActive(false);
    }
}
