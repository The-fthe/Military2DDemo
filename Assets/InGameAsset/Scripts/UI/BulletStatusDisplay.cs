using InGameAsset.Scripts.Data;
using TMPro;
using UnityEngine;

public class BulletStatusDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _bulletDisplayPanel;
    [SerializeField] PlayerManager _playerManager;
    [SerializeField] GameObject _playerGO;
   [SerializeField] BulletData _bulletData;

   void Start()
   {
   
   }

   public void Initialize()
    {
        _playerManager ??= FindObjectOfType<PlayerManager>();
        if (_playerManager != null)
        {
            _playerGO = _playerManager.GetPlayerGO;
            AssignUIEventToFoundPlayerTurretEvent();
        }else
            Debug.LogError($"{this.name} cant find playerSpawner");

        if (_bulletData != null) UpdateBulletPanel(_bulletData);
    }
    public void UpdateBulletPanel(BulletData bulletData)
    {
        _bulletData = bulletData;
        _bulletDisplayPanel.SetText($"{_bulletData.BulletNum.RunTimeValue}/ {_bulletData.Capacity}");
    }
    public void UpdateBulletPanel()
    {
        _bulletDisplayPanel.SetText($"{_bulletData.BulletNum.RunTimeValue}/ {_bulletData.Capacity}");
    }
    
    void AssignUIEventToFoundPlayerTurretEvent()
    {
        var turret = _playerGO.GetComponentInChildren<TurretShootFromBarrelWithOwnUpdate>();
        if (turret != null)
        {
            turret.OnReloadingFinish.AddListener(UpdateBulletPanel);
            turret.OnShoot.AddListener(UpdateBulletPanel);
        }
    }
}