using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveDecorationIcon : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        UIActions.EventActiveDecorationChanged += UpdateIcon;
    }

    private void OnDestroy()
    {
        UIActions.EventActiveDecorationChanged -= UpdateIcon;
    }

    void UpdateIcon(DecorationInfo info)
    {
        if (info != null)
        {
            image.sprite = info.uiIcon;
            image.color = Color.white;
        }
        else
        {
            image.color = new Color(0, 0, 0, 0);
        }
    }
}
