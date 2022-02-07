using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class AutoTargetAimerTurret : MonoBehaviour
{
    [SerializeField] CircleCollider2D _enemyTankDetecterCol;
    [SerializeField] Transform _targetAimer;
    [SerializeField] Image _targetSpriteImage;
    [SerializeField] string _targetSpritePath;
    [SerializeField] List<Transform> enemies;
    [SerializeField] bool _manualAimTrigger = false;
    [SerializeField] Camera _mainCamera;
    public UnityEvent<Vector2, float> OnTurretMove;
    Vector2 _manualDirV2;
    CancellationTokenSource _cts = new CancellationTokenSource();
    int _currentAimTargetIndex;
    async void Start()
    {
        _enemyTankDetecterCol = GetComponent<CircleCollider2D>();
        if (!_enemyTankDetecterCol.isTrigger) Debugger.Log("Trigger is not set");
        if (_targetSpritePath == String.Empty) Debugger.LogError($"{this.name} load path is empty");
        var prefab = await LoadAsset(_targetSpritePath, _cts.Token);
        _targetAimer = Instantiate(prefab, transform.parent);
        if (_targetAimer != null)
        {
            _targetSpriteImage = _targetAimer.GetComponentInChildren<Image>();
            _targetSpriteImage.color = new Color(0, 0, 0, 0);
            //_targetAimer.gameObject.SetActive(false);
            MoveToTargetAsync(_cts.Token).Forget();
        }
    }

    public void TriggerAutoManualTrigger()
    {
        _manualAimTrigger = !_manualAimTrigger;
    }

    async UniTask MoveToTargetAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UniTask.DelayFrame(3, cancellationToken: ct);
            if (_mainCamera == null) continue;
            SetAimDir();
            if (enemies.Count <= 0 || _manualAimTrigger)
            {
                _targetSpriteImage.color = new Color(0, 0, 0, 0);
            }
            else
            {
                if (_targetSpriteImage.color == new Color(0, 0, 0, 0) && !_manualAimTrigger)
                    _targetSpriteImage.color = new Color(1, 1, 1, 1);
                if (_currentAimTargetIndex < enemies.Count)
                    _targetAimer.position = enemies[_currentAimTargetIndex] == null
                        ? Vector3.zero
                        : enemies[_currentAimTargetIndex].position;
            }
        }
    }

    public void OnLook(InputValue inputValue)
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            enemies.Clear();
        }
        _manualDirV2 = inputValue.Get<Vector2>() != Vector2.zero ? inputValue.Get<Vector2>() : _manualDirV2;
        //SetAimDir();
    }

    public void OnChangeTarget(InputValue inputValue)
    {
        var rightMouseInput = inputValue.isPressed;
        Debug.Log($"right mouse press is {rightMouseInput}");
    }

    public void OnMouseLook(InputValue inputValue)
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            //if(_mainCamera == null)
            enemies.Clear();
        }

        Vector3 pointerPosition = inputValue.Get<Vector2>();
        if (pointerPosition.x < -1 && pointerPosition.y < -1 || pointerPosition.x > 1 && pointerPosition.y > 1)
        {
            Vector3 mousePosition = pointerPosition;
            mousePosition.z = _mainCamera.nearClipPlane;
            Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            _manualDirV2 = (mouseWorldPosition - (Vector2) gameObject.transform.position)
                .normalized;
        }
        else
        {
            _manualDirV2 = pointerPosition;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if(other.gameObject.CompareTag("IgnoreEnemy"))
            {
                enemies.Clear();
                return;
            }
            if (!enemies.Contains(other.transform))
            {
                enemies.Add(other.transform);
            }
         
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        enemies.Remove(other.transform);
    }

    void SetAimDir()
    {
        if (_manualAimTrigger)
        {
            OnTurretMove?.Invoke(_manualDirV2, MathfPlus.DirToAng(_manualDirV2));
        }
        else
        {
            if (enemies.Count <= 0)
            {
                OnTurretMove?.Invoke(_manualDirV2, MathfPlus.DirToAng(_manualDirV2));
                return;
            }
            _currentAimTargetIndex = GetNearestEnemyFromPlayer(enemies);
            var dirAim = GetEnemiesDir(enemies[_currentAimTargetIndex] == null
                ? Vector3.zero
                : enemies[_currentAimTargetIndex].position);
            OnTurretMove?.Invoke(dirAim, MathfPlus.DirToAng(dirAim));
        }
    }

    int GetNearestEnemyFromPlayer(List<Transform> froms)
    {
        float currentAngle = 0;
        float nearestAngleDifferent = 360;
        int smallestIndex = 0;
        Vector2 playerPos = transform.position;
        for (int i = 0; i < froms.Count; i++)
        {
            if (enemies[i] == null) continue;
            Vector2 dirFromToPlayer =
                new Vector2(enemies[i].position.x - playerPos.x, enemies[i].position.y - playerPos.y).normalized;
            currentAngle = Vector2.Angle(dirFromToPlayer, _manualDirV2);

            if (currentAngle < nearestAngleDifferent)
            {
                nearestAngleDifferent = currentAngle;
                smallestIndex = i;
            }
        }

        return smallestIndex;
    }

    Vector2 GetEnemiesDir(Vector3 targetPos)
    {
        return (targetPos - transform.position).normalized;
    }

    void OnDestroy()
    {
        if (_cts == null) return;
        if (!_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    async UniTask<Transform> LoadAsset(string path, CancellationToken ct)
    {
        return await Resources.LoadAsync(path, typeof(RectTransform)).WithCancellation(ct) as Transform;
    }
}