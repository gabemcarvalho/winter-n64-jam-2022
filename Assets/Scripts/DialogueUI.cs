using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI tmpTextbox;
    [SerializeField] private TextMeshProUGUI tmpNamebox;

    public static Action<DialogueObject> EventShowDialogue;
    public static event Action EventResumePlayerControl;

    private Animator animator;
    private RevealText revealText;

    private void Start()
    {
        animator = dialogueBox.GetComponent<Animator>();
        revealText = GetComponent<RevealText>();
        CloseDialogueBox();
        EventShowDialogue += ShowDialogue;
    }

    private void OnDestroy()
    {
        EventShowDialogue -= ShowDialogue;
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        StartCoroutine(AdvanceDialogue(dialogueObject));
    }

    // this now doubles as a cutscene system
    private IEnumerator AdvanceDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        tmpNamebox.text = string.Empty;
        tmpTextbox.text = string.Empty;

        if (dialogueObject.Mode == DialogueObject.CameraMode.Cutscene)
        {
            CameraController.EventEnableCutsceneCamera?.Invoke();
        }

        bool textboxVisible = false;
        foreach (TextBlock textBlock in dialogueObject.Dialogue)
        {
            if (!string.IsNullOrEmpty(textBlock.specialEvent))
            {
                if (textBlock.specialEvent == "StartGame")
                {
                    AudioManager.GetInstance().ResumeOverworldMusic(0.0f);
                }

                continue;
            }

            if (dialogueObject.Mode == DialogueObject.CameraMode.Cutscene && textBlock.moveCamera)
            {
                CutsceneCamera.EventChangeCutsceneCamera?.Invoke(textBlock.cameraRotation, textBlock.cameraStartPosition, textBlock.cameraEndPosition, textBlock.duration);
            }

            if (textBlock.showTextbox)
            {
                if (!textboxVisible)
                {
                    animator.Play("animEntry"); // assume instant animation for this game
                    yield return 0; // wait for next frame
                    animator.Play("animIdle");
                    textboxVisible = true;
                }

                tmpNamebox.text = textBlock.characterName;
                yield return revealText.Run(textBlock.text, tmpTextbox);
                yield return new WaitUntil(() => Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0));

                continue;
            }
            else
            {
                if (textboxVisible)
                {
                    animator.Play("animExit");
                    yield return 0; // wait for next frame
                    textboxVisible = false;
                    tmpNamebox.text = string.Empty;
                    tmpTextbox.text = string.Empty;
                }

                yield return new WaitForSeconds(textBlock.duration);
                continue;
            }
        }
        
        tmpTextbox.text = string.Empty;

        animator.Play("animExit");
        yield return 0; // wait for next frame
        
        tmpNamebox.text = string.Empty;

        if (dialogueObject.Mode == DialogueObject.CameraMode.Cutscene)
        {
            CameraController.EventDisableCutsceneCamera?.Invoke();
            UIActions.EventResumeGame?.Invoke();
        }

        CloseDialogueBox();
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        tmpNamebox.text = string.Empty;
        tmpTextbox.text = string.Empty;
        EventResumePlayerControl?.Invoke();
    }
}
