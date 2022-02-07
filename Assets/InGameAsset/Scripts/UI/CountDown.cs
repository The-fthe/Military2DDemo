using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGameAsset.Scripts.UI
{
    public class CountDown : MonoBehaviour
    {
        [SerializeField] int MaxCountDownNum = 3;
        [SerializeField] TextMeshProUGUI countDownText;
        [SerializeField] int CurrentCountDownNum;

        public UnityEvent OnCountDownFinish;

        public void Initialized()
        {
            countDownText ??= GetComponent<TextMeshProUGUI>();
            CurrentCountDownNum = MaxCountDownNum;
            //Debugger.Log("Count down is Initialize");
            StartCoroutine(WaitForCountDown());
        }

        IEnumerator WaitForCountDown()
        {
            countDownText.SetText("スタート");
            while (CurrentCountDownNum > 0)
            {
                countDownText.SetText(CurrentCountDownNum.ToString());
                //countDownText.text = CurrentCountDownNum.ToString();
                yield return new WaitForSeconds(1f);
                CurrentCountDownNum--;
                yield return null;
            }
            countDownText.SetText(String.Empty);
            OnCountDownFinish?.Invoke();
        }
    }
}