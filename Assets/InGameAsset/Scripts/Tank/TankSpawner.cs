using System;
using System.Collections;
using InGameAsset.Scripts;
using InGameAsset.Scripts.Player;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] UnitStatus _unitStatus;
    Vector3 _spawnPos;
    public void SpawnUnitAtSpawnPoint(GameObject tankToSpawn,Vector3 spawnPos)
    {
        _spawnPos = spawnPos;
        _unitStatus = tankToSpawn.GetComponentInChildren<UnitStatus>();
        StartCoroutine(RestUnitToSpawnPointAndMinusUnitLife(tankToSpawn));
    }

    IEnumerator RestUnitToSpawnPointAndMinusUnitLife(GameObject tankToSpawn)
    {
        yield return new WaitForSeconds(_unitStatus.SpawnTime);
        Debug.Log("SpawnUnitAndMinusUnitLife");
        SetChildPosToSpawnPos();
        SetUnitCurrentHpToMaxHp();
        if (_unitStatus.Life.RunTimeValue >= 0) _unitStatus.Life.RunTimeValue--;
        tankToSpawn.SetActive(true);
    }

     void SetChildPosToSpawnPos()
    {
        FindObjectOfType<PlayerManager>().ResetSpawnPosition(_spawnPos);
    }

     void SetUnitCurrentHpToMaxHp()
     {
         _unitStatus.Health = _unitStatus.MaxHealth.Value;
     }
}