using System;
using System.Collections;
using InGameAsset.Scripts.Managers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class RoadBlocker : MonoBehaviour
{
    [SerializeField] GameObject[] _effects;
    [SerializeField] GameObject[] _blockers;
    [SerializeField] StageLevel _stageLevel;
    [SerializeField] AudioSource _explosionSound;
    bool _isTrigger;
    
    public UnityEvent OnRoadBlockerCalled;

    void Awake()
    {
        _explosionSound = GetComponent<AudioSource>();
        OnRoadBlockerCalled.AddListener((() => _explosionSound.Play()));
        foreach (var blocker in _blockers)
        {
            blocker.SetActive(false);
        }
        foreach (var effect in _effects)
        {
            effect.SetActive(false);
        }
    }

    void Start()
    {
        _stageLevel ??= FindObjectOfType<StageLevel>();
       
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !_isTrigger)
            StartCoroutine(TriggerBlocker());
    }

    IEnumerator TriggerBlocker()
    {
        foreach (var effect in _effects)
        {
            effect.gameObject.SetActive(true);
            effect.transform.rotation=Quaternion.Euler(0,0,UnityEngine.Random.Range(0,360f));
        }        
        yield return new WaitForSeconds(1f);
        switch (_stageLevel._currentGameStage)
        {
            case LevelManager.GameStage.Level1:
                _blockers[0].SetActive(true);
                break;
            case LevelManager.GameStage.Level2:
                _blockers[1].SetActive(true);
                break;
            case LevelManager.GameStage.Level4:
                _blockers[2].SetActive(true);
                break;
        }
        _isTrigger = true;
        OnRoadBlockerCalled?.Invoke();
    }
}