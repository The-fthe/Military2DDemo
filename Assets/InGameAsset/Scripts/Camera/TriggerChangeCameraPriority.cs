using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeCameraPriority : MonoBehaviour
{
    [SerializeField] GameEvent SwitchToTransition;

    void OnTriggerEnter2D(Collider2D other)
    {
        SwitchToTransition?.Invoke();
    }
}
