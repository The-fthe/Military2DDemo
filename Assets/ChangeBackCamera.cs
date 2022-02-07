using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackCamera : MonoBehaviour
{
    [SerializeField] GameEvent changeBackCameraTriggerEvent;
    void Start()
    {
    }

    void OnEnable()
    {
        changeBackCameraTriggerEvent.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
