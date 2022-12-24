using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class DecoratedPercentText : MonoBehaviour
{
    private TextMeshProUGUI tmpTextbox;

    private void Awake()
    {
        tmpTextbox = GetComponent<TextMeshProUGUI>();

        UIActions.EventUpdateDecoratedPercent += SetPercent;
    }

    private void OnDestroy()
    {
        UIActions.EventUpdateDecoratedPercent -= SetPercent;
    }

    public void SetPercent(float percent)
    {
        tmpTextbox.text = $"{(int)(percent * 100)}%";
    }
}
