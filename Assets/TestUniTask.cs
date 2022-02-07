using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts.UI;
using UnityEngine;

public class TestUniTask : MonoBehaviour
{
    CancellationToken _ct = new CancellationToken();
     void Start()
    {
        StartFunc();
        Debug.Log("start is finsih");
        _ct.WaitHandle.Dispose();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    async void StartFunc()
    {
        await CountDowning(_ct);
        Debug.Log("start function is finish Trigger");
    }
    async UniTask CountDowning(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3), false, cancellationToken: ct);
        Debug.Log("CountDown Is finish Trigger");

    }
}
