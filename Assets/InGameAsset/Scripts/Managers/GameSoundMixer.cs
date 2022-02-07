using System.Collections.Generic;
using UnityEngine;

public class GameSoundMixer : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    public GameSoundMixer Instance { get; private set; }
    List<AudioClip> _audioClipsToPlay;
    int _currentClipIndex;
    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (_audioSource.isPlaying) return;
        if (_audioClipsToPlay.Count > 0)
        {
            _audioSource.clip = _audioClipsToPlay[--_currentClipIndex];
            _audioSource.Play();
        }
    }

    void PlaySynconic( AudioClip audioClip)
    {
        _audioClipsToPlay.Add(audioClip);
    }
    
    
}