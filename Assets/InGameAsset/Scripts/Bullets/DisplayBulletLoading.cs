using System;
using InGameAsset.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBulletLoading : MonoBehaviour
{
    [SerializeField] Slider _slider;
    bool _isPlayerSpawn;
   [SerializeField] float _reloadPercentage=1;

   public void Initialize()
    {
         var turret = FindObjectOfType<PlayerInputEvent>().GetComponentInChildren<TurretShootFromBarrelWithOwnUpdate>();
         if (turret != null)
         {
             turret.OnReloading.AddListener(DisplayPanelCoolDown);
         }
         else
         {
             Debugger.LogError($"{this.name} Turret cant be found ");
         }
    }

    void DisplayPanelCoolDown(float reloadPercentage)
    {
        _reloadPercentage = reloadPercentage;
        _slider.value = _reloadPercentage > 0.99f ? 0 : 1f-_reloadPercentage;
    }
}