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
    public static Action EventStartGame;

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private DialogueObject introDialog;

    private GameObject activePanel;
    private GameObject preSettingsPanel;

    void Awake()
    {
        EventPauseGame += ShowPauseMenu;
        EventResumeGame += ClosePauseMenu;
        EventStartGame += StartGame;
    }

    private void Start()
    {
        EventStartGame?.Invoke();
    }

    void OnDestroy()
    {
        EventPauseGame -= ShowPauseMenu;
        EventResumeGame -= ClosePauseMenu;
        EventStartGame -= StartGame;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void GotoSettings()
    {
        if (activePanel != null)
        {
            preSettingsPanel = activePanel;
            activePanel.SetActive(false);
        }
        
        activePanel = settingsPanel;
        activePanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        activePanel = preSettingsPanel;

        if (activePanel != null)
        {
            activePanel.SetActive(true);
        }
    }

    public void ShowPauseMenu()
    {
        activePanel.SetActive(false);
        pausePanel.SetActive(true);
        activePanel = pausePanel;
        Time.timeScale = 0.0f;
    }

    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
        activePanel = gamePanel;
        activePanel.SetActive(true);
        Time.timeScale = 1.0f;
    }

    public void InvokeResumeGame()
    {
        EventResumeGame?.Invoke();
    }

    public void StartGame()
    {
        PlayerController.EventSetCanMove?.Invoke(false);

        TransitionPanel.EventStartFadeInTransition?.Invoke(0.8f);

        activePanel = mainMenuPanel;
        mainMenuPanel.SetActive(true);

        gamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);

        CameraController.EventEnableTitleCamera?.Invoke();
    }

    public void OnPlayButtonPressed()
    {
        TransitionPanel.EventTransitionEnded += PlayIntroDialog;
        TransitionPanel.EventStartTransition?.Invoke(0.8f);
    }

    public void PlayIntroDialog()
    {
        TransitionPanel.EventTransitionEnded -= PlayIntroDialog;

        CameraController.EventDisableTitleCamera?.Invoke();
        mainMenuPanel.SetActive(false);
        DialogueUI.EventShowDialogue.Invoke(introDialog);
    }
}
