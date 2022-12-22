using System;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanel : MonoBehaviour
{
    private Image image;

    private float alpha = 0.0f;
    private float fadeTime = 0.4f;
    private bool fadingOut = false;

    public static Action EventTransitionEnded;

    public static Action<float> EventStartFadeInTransition;
    public static Action<float> EventStartTransition;

    void Awake()
    {
        image = GetComponent<Image>();

        EventStartFadeInTransition += StartFadeInTransition;
        EventStartTransition += StartTransition;
    }

    private void OnDestroy()
    {
        EventStartFadeInTransition -= StartFadeInTransition;
        EventStartTransition -= StartTransition;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingOut)
        {
            alpha += Time.unscaledDeltaTime / fadeTime;

            if (alpha >= 1.0f)
            {
                fadingOut = false;
                EventTransitionEnded?.Invoke();
            }

            UpdateAlpha();
        }
        else if (alpha > 0.0f)
        {
            alpha -= Time.unscaledDeltaTime / fadeTime;

            //if (alpha <= 0.0f)
            //{
            //    EventTransitionEnded?.Invoke();
            //}

            UpdateAlpha();
        }
    }

    private void UpdateAlpha()
    {
        Color col = image.color;
        col.a = alpha;
        image.color = col;
    }

    public void StartFadeInTransition(float newFadeTime)
    {
        fadeTime = newFadeTime;
        fadingOut = false;
        alpha = 1.0f;
        UpdateAlpha();
    }

    public void StartTransition(float newFadeTime)
    {
        fadeTime = newFadeTime;
        fadingOut = true;
        alpha = 0.0f;
        UpdateAlpha();
    }
}
