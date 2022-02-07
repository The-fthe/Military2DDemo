using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class DisplayLog:MonoBehaviour
    {
        public Func<string> Log;
        
        string _message;
        IEnumerator _displayMessage;
        TextMeshProUGUI _textMeshProUGUI;
        void Start()
        {
            _textMeshProUGUI ??= GetComponent<TextMeshProUGUI>();
            _displayMessage = displayMessageIEnumerator();
            StartCoroutine(_displayMessage);
        }

        IEnumerator displayMessageIEnumerator()
        {
            yield return new WaitForSeconds(1f);
            _message = Log?.Invoke();
            _textMeshProUGUI.SetText(_message);
            _displayMessage = displayMessageIEnumerator();
            StartCoroutine(_displayMessage);
        }
    }
}