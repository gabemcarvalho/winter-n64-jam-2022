using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RevealText : MonoBehaviour
{
    private string currentSentence;
    [SerializeField] private float scrollWaitTime;
    [SerializeField] private int maxLines;
    
    public Coroutine Run(string sentence, TextMeshProUGUI tmpGUI)
    {
        return StartCoroutine(Scroll(sentence, tmpGUI));
    }

    private IEnumerator Scroll(string sentence, TextMeshProUGUI tmpGUI)
    {
        tmpGUI.text = sentence;
        currentSentence = sentence;
        tmpGUI.maxVisibleCharacters = 0;

        yield return new WaitForEndOfFrame(); // need to wait for textInfo to update

        for (int i = 1; i <= tmpGUI.textInfo.characterCount; i++)
        {
            tmpGUI.maxVisibleCharacters = i;

            if (i == tmpGUI.textInfo.characterCount)
            {
                break;
            }

            float time = scrollWaitTime;
            char current = tmpGUI.textInfo.characterInfo[i - 1].character;
            if (current.ToString().Equals(",") || current.ToString().Equals("."))
            {
                time += 0.3f;
            }

            //Debug.Log(textMeshPro.textInfo.characterInfo[i - 1].index);
                
            if (tmpGUI.textInfo.characterInfo[i - 1].lineNumber == maxLines) // lineNumber starts at 0
            {
                int k = 0;
                while (tmpGUI.textInfo.characterInfo[k].lineNumber == 0)
                {
                    k++;
                }

                currentSentence = currentSentence.Substring(tmpGUI.textInfo.characterInfo[k-1].index + 1);
                tmpGUI.text = currentSentence;
                i -= k;
            }

            yield return new WaitForSeconds(time);
        }

    }

}
