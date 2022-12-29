using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class UIActions : MonoBehaviour
{
    public static Action EventPauseGame;
    public static Action EventResumeGame;
    public static Action<ThirdPersonCamera.Targetable> EventEnterTextboxCamera;
    public static Action<bool> EventExitTextboxCamera;
    public static Action EventUnlockCursor;
    public static Action EventLockCursor;
    public static Action EventHideCursor;
    public static Action<DecorationInfo> EventActiveDecorationChanged;
    public static Action EventStartGame;
    public static Action EventShowMinigameUI;
    public static Action EventHideMinigameUI;
    public static Action<float> EventUpdateDecoratedPercent;
    public static Action<bool> EventSetSuccessEnabled;
    public static Action<bool> EventSetTryAgainEnabled;
    public static Action<string> EventSetRequestText;
    public static Action<bool> EventSetDecoratePromptEnabled;
    public static Action EventPlayCredits;

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject minigamePanel;

    [SerializeField] private GameObject successText;
    [SerializeField] private GameObject tryAgainText;
    [SerializeField] private TextMeshProUGUI tmpRequestText;
    [SerializeField] private GameObject decoratePrompt;

    [SerializeField] private DialogueObject introDialog;
    [SerializeField] private DialogueObject creditsDialog;

    private GameObject activePanel;
    private GameObject preSettingsPanel;
    private GameObject prePausePanel;

    public static bool PlayedCredits = false;

    void Awake()
    {
        EventPauseGame += ShowPauseMenu;
        EventResumeGame += ClosePauseMenu;
        EventStartGame += StartGame;
        EventShowMinigameUI += ShowMinigameUI;
        EventHideMinigameUI += HideMinigameUI;
        EventSetSuccessEnabled += SetSuccessEnabled;
        EventSetTryAgainEnabled += SetTryAgainEnabled;
        EventSetRequestText += SetRequestText;
        EventSetDecoratePromptEnabled += SetDecoratePromptEnabled;
        EventPlayCredits += PlayCreditsDialog;
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
        EventShowMinigameUI -= ShowMinigameUI;
        EventHideMinigameUI -= HideMinigameUI;
        EventSetSuccessEnabled -= SetSuccessEnabled;
        EventSetTryAgainEnabled -= SetTryAgainEnabled;
        EventSetRequestText -= SetRequestText;
        EventSetDecoratePromptEnabled -= SetDecoratePromptEnabled;
        EventPlayCredits -= PlayCreditsDialog;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void GotoSettings()
    {
        AudioManager.GetInstance().PlaySound("Button");

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
        AudioManager.GetInstance().PlaySound("Button");

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
        prePausePanel = activePanel;
        activePanel = pausePanel;
        Time.timeScale = 0.0f;
        MiniGameCamera.EventStopRotating?.Invoke(); // hack
    }

    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
        activePanel = prePausePanel ? prePausePanel : gamePanel;
        activePanel.SetActive(true);
        Time.timeScale = 1.0f;
    }
    
    public void ShowMinigameUI()
    {
        activePanel.SetActive(false);
        minigamePanel.SetActive(true);
        activePanel = minigamePanel;
    }

    public void HideMinigameUI()
    {
        minigamePanel.SetActive(false);
        activePanel = gamePanel;
        activePanel.SetActive(true);
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
        minigamePanel.SetActive(false);

        CameraController.EventEnableTitleCamera?.Invoke();
    }

    public void OnPlayButtonPressed()
    {
        AudioManager.GetInstance().PlaySound("Button");
        EventHideCursor?.Invoke();
        TransitionPanel.EventTransitionEnded += PlayIntroDialog;
        TransitionPanel.EventStartTransition?.Invoke(0.8f);
        AudioManager.GetInstance().StopMusic(3.0f);
        EventUpdateDecoratedPercent?.Invoke(0.0f);
        CollectedText.EventUpdateNumCollectibles?.Invoke(); // a hack since these didn't work in Start()
        CollectedText.EventUpdateNumCollected?.Invoke(0);
    }

    public void PlayIntroDialog()
    {
        TransitionPanel.EventTransitionEnded -= PlayIntroDialog;

        CameraController.EventDisableTitleCamera?.Invoke();
        mainMenuPanel.SetActive(false);
        DialogueUI.EventShowDialogue.Invoke(introDialog);
    }

    public void PlayCreditsDialog()
    {
        if (PlayedCredits) return;

        PlayerController.EventSetCanMove?.Invoke(false);
        DialogueUI.EventShowDialogue.Invoke(creditsDialog);
        PlayedCredits = true;
    }

    public void OnMinigameResetPressed()
    {
        MiniGameScript.EventResetDecoratable?.Invoke();
        AudioManager.GetInstance().PlaySound("Button");
    }

    public void OnMinigameFinishPressed()
    {
        MiniGameScript.EventFinishMinigame?.Invoke();
        AudioManager.GetInstance().PlaySound("Button");
    }

    public void SetSuccessEnabled(bool enabled)
    {
        successText.SetActive(enabled);
    }
    
    public void SetTryAgainEnabled(bool enabled)
    {
        tryAgainText.SetActive(enabled);
    }

    public void SetRequestText(string text)
    {
        tmpRequestText.text = text;
    }

    public void SetDecoratePromptEnabled(bool enabled)
    {
        decoratePrompt.SetActive(enabled);
    }

    public void RotateTreeLeft()
    {
        MiniGameCamera.EventStartRotating?.Invoke(false);
    }

    public void RotateTreeRight()
    {
        MiniGameCamera.EventStartRotating?.Invoke(true);
    }

    public void StopRotatingTree()
    {
        MiniGameCamera.EventStopRotating?.Invoke();
    }
}
