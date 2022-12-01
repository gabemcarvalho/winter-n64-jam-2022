using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] [TextArea(4, 10)] private string[] dialogue;

    public string[] Dialogue => dialogue;
    public string CharacterName => characterName;
}
