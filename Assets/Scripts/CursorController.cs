using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D mouseCursor;

    Vector2 hotSpot = new Vector2(0, 0);
    CursorMode cursorMode = CursorMode.Auto;

    private void Awake()
    {
        Cursor.SetCursor(mouseCursor, hotSpot, cursorMode);

        UIActions.EventPauseGame += UnlockCursor;
        UIActions.EventResumeGame += LockCursor;
    }

    private void OnDestroy()
    {
        UIActions.EventPauseGame -= UnlockCursor;
        UIActions.EventResumeGame -= LockCursor;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
