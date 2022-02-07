using System;
using Cysharp.Threading.Tasks;
using InGameAsset.Scripts.Managers;
using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    [SerializeField] LevelManager.GameStage _gameStageToChange;
    [SerializeField] LevelManager.GameStage _gameStageToUnload;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] StageLevel _stageLevel;
    [SerializeField] bool _nextStageIsTrigger;
    [SerializeField] ArrowFaceDir _dirToFace;
    [SerializeField] SpriteRenderer _arrowSpriteRender;
    [SerializeField] Sprite[] _arrowSprites;
    public LevelManager GetLevelManager { set=> _levelManager= value; }
    public ArrowFaceDir SetArrowFaceDir
    {
        get => _dirToFace;
        set =>_dirToFace = value;
    }
    async void Start()
    {
        _stageLevel = FindObjectOfType<StageLevel>();
        _gameStageToChange = _stageLevel._nextStageTransfer;
        _gameStageToUnload = _stageLevel._currentGameStage;
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        //_levelManager ??= FindObjectOfType<LevelManager>();
        switch (_dirToFace)
        {
            case ArrowFaceDir.up:
                _arrowSpriteRender.sprite = _arrowSprites[0];
                break;
            case ArrowFaceDir.down:
                _arrowSpriteRender.sprite = _arrowSprites[1];
                break;
            case  ArrowFaceDir.left:
                _arrowSpriteRender.sprite = _arrowSprites[2];
                break;
            case ArrowFaceDir.right:
                _arrowSpriteRender.sprite = _arrowSprites[3];
                break;
        }
        if (_levelManager == null)
        {
            Debugger.LogError("Next Stage cant find level manager");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !_nextStageIsTrigger)
        {
            _nextStageIsTrigger = true;
            Debug.Log("On trigger enter is trigger");
            _levelManager.TransferToAnotherStage(_gameStageToChange.ToString(),_gameStageToUnload.ToString());
        }
    }

    public enum ArrowFaceDir
    {
        up,down,left,right
    }
}