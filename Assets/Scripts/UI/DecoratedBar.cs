using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecoratedBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        UIActions.EventUpdateDecoratedPercent += SetPercent;
    }

    private void OnDestroy()
    {
        UIActions.EventUpdateDecoratedPercent -= SetPercent;
    }

    public void SetPercent(float percent)
    {
        slider.value = percent;
    }
}
