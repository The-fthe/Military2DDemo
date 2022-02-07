using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeWriterEffect : MonoBehaviour
{
    [SerializeField]  float StartTexting=1f;
    [SerializeField] float WaitNextText = 2f;
    [SerializeField] string[] fullText;
    [SerializeField] TextMeshProUGUI _textDisplay;
    [SerializeField] GameObject Panel;

     float delay = 0.1f;
     string currentText = "";

     void Awake()
     {
         Panel.SetActive(false);
     }

     void Start()
    {
        _textDisplay = GetComponent<TextMeshProUGUI>();
        StartCoroutine(ShowText());
    }
    
    IEnumerator ShowText()
    {
        yield return new WaitForSeconds(StartTexting);
        for (int i = 0; i < fullText.Length; i++ )
        {
            for (int j = 0; j <= fullText[i].Length; j++)
            {
                currentText = fullText[i].Substring(0, j);
                _textDisplay.SetText(currentText);;
                yield return new WaitForSeconds(delay);
            }
            yield return new WaitForSeconds(WaitNextText);
        }
    }
}
