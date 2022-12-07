using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UIActions : MonoBehaviour
{
    public static Action EventPauseGame;
    public static Action EventResumeGame;
    public static Action<ThirdPersonCamera.Targetable> EventEnterTextboxCamera;
    public static Action<bool> EventExitTextboxCamera;
    public static Action EventUnlockCursor;
    public static Action EventLockCursor;
    public static Action<DecorationInfo> EventActiveDecorationChanged;

    void Awake()
    {
        
    }

    private void Start()
    {
        EventResumeGame?.Invoke();
    }

    void OnDestroy()
    {
        
    }
}
