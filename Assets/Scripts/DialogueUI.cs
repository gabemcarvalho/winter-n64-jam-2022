using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI tmpTextbox;
    [SerializeField] private TextMeshProUGUI tmpNamebox;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private Animator animator;

    public static Action<DialogueObject> EventShowDialogue;
    public static event Action EventResumePlayerControl;

    private RevealText revealText;

    private void Start()
    {
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

    private IEnumerator AdvanceDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        tmpNamebox.text = dialogueObject.CharacterName;
        tmpTextbox.text = string.Empty;

        animator.Play("animEntry");
        yield return 0; // wait for next frame
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play("animIdle");

        foreach (string dialogue in dialogueObject.Dialogue)
        {
            yield return revealText.Run(dialogue, tmpTextbox);
            yield return new WaitUntil(() => Input.GetButtonDown("Jump"));
        }
        
        tmpTextbox.text = string.Empty;

        animator.Play("animExit");
        yield return 0; // wait for next frame
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        tmpNamebox.text = string.Empty;

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
