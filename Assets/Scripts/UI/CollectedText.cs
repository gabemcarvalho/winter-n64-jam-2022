using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class CollectedText : MonoBehaviour
{
    public static Action<int> EventUpdateNumCollected;

    private TextMeshProUGUI tmpTextbox;

    public int numCollectibles;

    private void Awake()
    {
        tmpTextbox = GetComponent<TextMeshProUGUI>();

        EventUpdateNumCollected += UpdateNumCollected;
    }

    void Start()
    {
        numCollectibles = FindObjectsOfType<Collectible>().Length;
    }

    private void OnDestroy()
    {
        EventUpdateNumCollected -= UpdateNumCollected;
    }

    public void UpdateNumCollected(int collected)
    {
        tmpTextbox.text = $"{collected}/{numCollectibles}";
    }
}
