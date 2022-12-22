using UnityEngine;

[System.Serializable]
public class TextBlock
{
    [TextArea(4, 10)] public string text;
    public string characterName;
    public bool showTextbox = true;
    public bool moveCamera = false;
    public Vector3 cameraRotation;
    public Vector3 cameraStartPosition;
    public Vector3 cameraEndPosition;
    public float duration = 3.0f;
    public string specialEvent = string.Empty;
}
