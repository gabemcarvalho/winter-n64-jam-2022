using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    public enum CameraMode
    {
        Focus,
        Cutscene
    }

    [SerializeField] private CameraMode cameraMode = CameraMode.Focus;
    [SerializeField] private TextBlock[] dialogue;

    public TextBlock[] Dialogue => dialogue;
    public CameraMode Mode => cameraMode;
}
