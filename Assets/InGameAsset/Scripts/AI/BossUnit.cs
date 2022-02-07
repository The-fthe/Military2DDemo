using InGameAsset.Scripts.Player;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BossUnit:MonoBehaviour
{
    [SerializeField]protected IntGameData _bossHp;
    public abstract IntGameData GetBossHp();
}