using InGameAsset.Scripts;
using InGameAsset.Scripts.Managers;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : ManagerManagement
{
    [SerializeField] AudioClip _gamePrepareMusic;
    [SerializeField] AudioClip _mainMenuAudio;
    [SerializeField] AudioClip _winAudio;
    [SerializeField] AudioClip _lostAudio;
    [SerializeField] AudioClip[] _audioClips;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioMixer _audioMixer;

    void Start()
    {
        _audioSource = gameObject.GetComponentInChildren<AudioSource>();
        _audioSource.loop = true;
        _audioSource.clip = _mainMenuAudio;
        _audioSource.Play();
        LevelManager.OnGameStateChanged.AddListener(HandleGameStageChange);
    }

    void HandleGameStageChange(LevelManager.GameState currentState, LevelManager.GameState previousState)
    {
        if (previousState == LevelManager.GameState.PreGame && currentState == LevelManager.GameState.Running)
        {
            //PlayStageEnterMusic();
        }

        if (previousState == LevelManager.GameState.Running && currentState == LevelManager.GameState.Pause)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.volume = _audioSource.volume / 2;
            }
        }

        if (previousState == LevelManager.GameState.Pause && currentState == LevelManager.GameState.Running)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.volume = _audioSource.volume * 2;
            }
        }
        if (previousState == LevelManager.GameState.Pause && currentState == LevelManager.GameState.PreGame)
        {
        }
    }

    public void PlayMainMenuMusic()
    {
        _audioSource.Stop();
        _audioSource.clip = _mainMenuAudio;
        _audioSource.Play();
    }

    public void HandleStageLevelBGMusic()
    {
        switch (LevelManager.CurrentStageLevel._currentGameStage)
        {
            case LevelManager.GameStage.Level1:
                _audioSource.clip = _audioClips[0];
                break;
            case LevelManager.GameStage.Level2:
                _audioSource.clip = _audioClips[1];
                break;
            case LevelManager.GameStage.Level4:
                _audioSource.clip = _audioClips[2];
                break;
            default:
                _audioSource.clip = _audioClips[0];
                break;
        }

        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }
    
    public void PlayStageEnterMusic()
    {
        _audioSource.Stop();
        _audioSource.clip = _gamePrepareMusic;
        _audioSource.Play();
    }
    public void PlayWinMusic()
    {
        _audioSource.Stop();
        _audioSource.clip = _winAudio;
        _audioSource.Play();
    }

    public void PlayLostMusic()
    {
        _audioSource.Stop();
        _audioSource.clip = _lostAudio;
        _audioSource.Play();
    }

    public void AdjustSoundVolume(float sliderValue)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
}