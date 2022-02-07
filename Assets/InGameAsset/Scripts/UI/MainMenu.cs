using InGameAsset.Scripts.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animation _mainMenuAnimator;
    [SerializeField] AnimationClip _fadeOutAnimation;
    [SerializeField] AnimationClip _fadeInAnimation;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] Button StartButton;
    [SerializeField] CanvasGroup _canvasGroup;
    [Space(10)]
    [SerializeField] Camera _dummyCamera;
    [Space(10)]
    public UnityEvent<bool> OnMainMenuFadeComplete;
    public void  SetMainMenuCameraToTrue ()=> _dummyCamera.gameObject.SetActive(true);
     void Start()
     {
         _canvasGroup = GetComponent<CanvasGroup>();
        _dummyCamera ??= Camera.main;
        _levelManager ??= FindObjectOfType<LevelManager>();
        if(_levelManager != null)
            _levelManager.OnGameStateChanged.AddListener(HandleGameStateChanged);
        if(StartButton!= null)
            StartButton.onClick.AddListener(_levelManager.StartGame);
        _levelManager.OnBackMenu.AddListener(()=>StartButton.Select());
    }

     void HandleGameStateChanged(LevelManager.GameState currentState, LevelManager.GameState previousState)
    {
        if(previousState== LevelManager.GameState.PreGame && currentState == LevelManager.GameState.Running)
            FadeOut();
        if (previousState != LevelManager.GameState.PreGame && currentState == LevelManager.GameState.PreGame)
            FadeIn();
    }

    public void OnFadeOutComplete()
    {
        OnMainMenuFadeComplete?.Invoke(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnFadeInComplete()
    {
        Debug.Log("Fade in is trigger");
        OnMainMenuFadeComplete?.Invoke(false);
        SetDummyCameraActivate(true);
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    

    public void FadeIn()
    {
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeInAnimation;
        _mainMenuAnimator.Play();
    }
    public void FadeOut()
    {
        SetDummyCameraActivate(false);
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeOutAnimation;
        _mainMenuAnimator.Play();
    } 
    
    public void SetDummyCameraActivate(bool active)
    {
        _dummyCamera.gameObject.SetActive(active);
    }
}