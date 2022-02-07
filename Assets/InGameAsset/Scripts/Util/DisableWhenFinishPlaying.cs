using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenFinishPlaying : MonoBehaviour
{
    AudioSource _audioSource;
    IEnumerator _disableAudioIenumerator;
    void Awake()
    {
        _audioSource ??= GetComponent<AudioSource>();
        _disableAudioIenumerator = DisableAudioCoroutine();
        StartCoroutine(_disableAudioIenumerator);
    }

    IEnumerator DisableAudioCoroutine()
    {
        while (_audioSource.isPlaying)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnEnable()
    {
        _disableAudioIenumerator = DisableAudioCoroutine();
        StartCoroutine(_disableAudioIenumerator);    
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
