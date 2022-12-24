using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class CollectedText : MonoBehaviour
{
    public static Action<int> EventUpdateNumCollected;
    public static Action EventUpdateNumCollectibles;

    private TextMeshProUGUI tmpTextbox;

    public int numCollectibles;

    private void Awake()
    {
        tmpTextbox = GetComponent<TextMeshProUGUI>();

        EventUpdateNumCollected += UpdateNumCollected;
        EventUpdateNumCollectibles += UpdateNumCollectibles;
    }

    private void OnDestroy()
    {
        EventUpdateNumCollected -= UpdateNumCollected;
        EventUpdateNumCollectibles -= UpdateNumCollectibles;
    }

    public void UpdateNumCollectibles()
    {
        numCollectibles = FindObjectsOfType<Collectible>().Length;
    }

    public void UpdateNumCollected(int collected)
    {
        tmpTextbox.text = $"{collected}/{numCollectibles}";
    }
}
