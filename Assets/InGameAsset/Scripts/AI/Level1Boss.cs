using System.Linq;
using InGameAsset.Scripts;
using InGameAsset.Scripts.AI;
using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

public class Level1Boss : BossUnit
{
    [SerializeField] StageLevel _stageLevel;
    [SerializeField] BossHpDisplayPanel _bossHpDisplayPanel;
    [SerializeField] IntGameData[] _tanksHps;
    [SerializeField] IntGameData _totalTanksNum;
    [SerializeField] float _totalBossHpPercentage=1f;
    [SerializeField] UnitStatus[] _tankUnits;
    [SerializeField] EnemyAI_ChaserTank[] _tankUnitAIs;
    [SerializeField] PlayerTankMover _TargetToKill;
    [SerializeField] bool _isInitialize;
    [SerializeField] bool _isLastFightInitialize;
    [SerializeField] int _lastFightTriggerMinimumTankKill =4;
    [SerializeField]  int indexTrigger;
    public UnityEvent OnDead;
    void Start()
    {
        _stageLevel.OnGameStart.AddListener(Initialize);
        var uiManager = FindObjectOfType<UIManager>();
        _bossHpDisplayPanel = uiManager.GetBossHpDisplayPanel;
    }

    void Initialize()
    {
        _TargetToKill = FindObjectOfType<PlayerTankMover>();
        _tankUnitAIs = FindObjectsOfType<EnemyAI_ChaserTank>();
        _tankUnits = FindObjectsOfType<UnitStatus>().Where(i => i.CompareTag("Level1BossTank")).ToArray();
        if (_tankUnits.Length > 0)
        {
            _tanksHps = _tankUnits.Select(i => i._currentHealthData).ToArray();
            _totalTanksNum = _tankUnits.FirstOrDefault()?.Life;
            _totalTanksNum.RunTimeValue = _totalTanksNum.Value;
            foreach (var tankUnit in _tankUnits)
            {
                tankUnit.gameObject.GetComponent<Damageable>().OnHit.AddListener(UpdateBossTanksHps);
                tankUnit.gameObject.GetComponent<Damageable>().OnDead.AddListener(()=>
                {
                    _totalTanksNum.RunTimeValue--;
                    UpdateBossTanksHps();
                });
            }
            indexTrigger = UnityEngine.Random.Range(_lastFightTriggerMinimumTankKill, _totalTanksNum.RunTimeValue);
            _isInitialize = true;
        }
        else
        {
            Debug.LogWarning("Level1 Boss cant find boss tanks");
        }
    }

    void Update()
    {
        if (!_isInitialize) return;
        if (_totalTanksNum.RunTimeValue <indexTrigger && !_isLastFightInitialize)
        {
            foreach (var tank in _tankUnitAIs)
            {
                if (tank.gameObject.activeSelf)
                {
                    tank.IsLastFight = true;
                    tank.FocusTarget = _TargetToKill.transform;
                }
            }
            _isLastFightInitialize = true;
        }
    }

    public override IntGameData GetBossHp()
    {
        SearchMapForTanks();
        int totalcurrentHp = 0;
        foreach (var tankHp in _tanksHps)
        {
            totalcurrentHp += tankHp.RunTimeValue;
        }
        _bossHp.Value = _totalTanksNum.RunTimeValue * _tanksHps[0].Value;
        _bossHp.RunTimeValue = totalcurrentHp;
        return _bossHp;
    }

    public void UpdateBossTanksHps()
    {
        _totalBossHpPercentage = CalculatedBossTankHps();
        _bossHpDisplayPanel.Bind(_totalBossHpPercentage);
        if (_totalBossHpPercentage <= 0)
        {
            OnDead?.Invoke();
        }
    }

    float CalculatedBossTankHps()
    {
        float totalcurrentHp = 0;
        foreach (var tankHp in _tanksHps)
        {
            totalcurrentHp += tankHp.RunTimeValue;
        }
        return totalcurrentHp /(_totalTanksNum.Value*_tanksHps[0].Value);
    }

    void SearchMapForTanks()
    {
        if (_tanksHps.Length <= 0)
        {
            var tankUnits = FindObjectsOfType<UnitStatus>().Where(i => i.CompareTag("Level1BossTank")).ToArray();
            _tanksHps = tankUnits.Select(i => i._currentHealthData).ToArray();
        }
    }
}
