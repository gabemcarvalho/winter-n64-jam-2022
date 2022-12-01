using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UIActions : MonoBehaviour
{
    public static Action EventPauseGame;
    public static Action EventResumeGame;

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
