using System.Linq;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Player;
using UnityEngine;

public class HealthStatusDisplay : MonoBehaviour
{
    [SerializeField] PlayerHealth[] _playerHps;
    [SerializeField] IntGameData _health;
    [SerializeField] IntGameData _lifeData;

    void Start()
    {
        _playerHps =_playerHps.Length <=0? GetComponentsInChildren<PlayerHealth>(): _playerHps;
    }

    public void Initialize()
    {
        _playerHps =_playerHps.Length <=0? GetComponentsInChildren<PlayerHealth>(): _playerHps;
        FindPlayerAndUpdateUI();
    }

    void FindPlayerAndUpdateUI()
    {
        var playerDamageable= FindObjectsOfType<Damageable>().FirstOrDefault((i)=>i.IsPlayer);
        if(playerDamageable == null) Debug.LogError("Player damagable cant be find");
        else
        {
            playerDamageable.OnHealthAndLifeChange.AddListener(UpdateLifeIcon);
        }
        var unitStatus = FindObjectsOfType<UnitStatus>().FirstOrDefault((i) => i.gameObject.layer == LayerMask.NameToLayer("Player"));
        if(unitStatus == null) Debug.LogError("player unit status cant be found");
        else
        {
            unitStatus.OnUnitCreated.AddListener(UpdateLifeIcon);
        }
    }

    public void UpdateLifeIcon()
    {
        var zeroBaselife = _lifeData.RunTimeValue-1;
        if(_health != null)
            _playerHps[_lifeData.RunTimeValue - 1].CurrenthealthPercentage = _health.RunTimeValue / _health.Value;
        
        if (zeroBaselife -1  >= 0)
        {
            for (int i = 0; i < zeroBaselife; i++)
            {
                _playerHps[i].CurrenthealthPercentage = 1f;
            }
        }
    }

    public void UpdateLifeIcon(IntGameData healthData,IntGameData lifeData)
    {
        _health = healthData;
        _lifeData = lifeData;
        var zeroBaseIndex = lifeData.RunTimeValue - 1;
        _playerHps =_playerHps.Length <=0? GetComponentsInChildren<PlayerHealth>(): _playerHps;
        _playerHps[zeroBaseIndex].CurrenthealthPercentage =(float) healthData.RunTimeValue/(float)healthData.Value;
        for (int i = 0; i <= zeroBaseIndex-1; i++)
        {
            _playerHps[i].CurrenthealthPercentage = 1f;
        }
    }
}