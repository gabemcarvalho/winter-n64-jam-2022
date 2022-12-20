using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public bool capFrameRate = true;
    public int targetFrameRate = 30;

    void Start()
    {
        if (capFrameRate)
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
