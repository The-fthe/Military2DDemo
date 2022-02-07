using System.Diagnostics;
using System.Linq;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpDisplayPanel : MonoBehaviour
{
    Slider _hpSlider;
    [SerializeField] IntGameData _HpData;
    [SerializeField] IntGameData _level1EnemiesLife;
    [SerializeField] CanvasGroup[] _bossSpritesCG;

    [SerializeField] TextMeshProUGUI _level1LifeDisplayText;
    // [SerializeField] UnitStatus bossStatus;
    
    public void Start()
    {
        _hpSlider ??= GetComponentInChildren<Slider>();
        _hpSlider.value =(float) _HpData.RunTimeValue / (float)_HpData.Value;
        _level1LifeDisplayText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Bind()
    { 
        //_hpSlider.value = (float)bossStatus.Health / (float)bossStatus.MaxHealth.Value;
        _hpSlider.value =(float) _HpData.RunTimeValue / (float)_HpData.Value;
        _level1LifeDisplayText.SetText("x" +_level1EnemiesLife.RunTimeValue);
    }

    public void Bind(LevelManager.GameStage gameStage)
    {
        foreach (var bossSprite in _bossSpritesCG)
        {
            bossSprite.alpha = 0;
        }
        switch (gameStage)
        {
            case LevelManager.GameStage.Level1:
                _bossSpritesCG[0].alpha = 1;
                _level1LifeDisplayText.SetText("x" +_level1EnemiesLife.RunTimeValue);
                break;
            case LevelManager.GameStage.Level2:
                _bossSpritesCG[1].alpha = 1;
                break;
            case  LevelManager.GameStage.Level3:
                _bossSpritesCG[0].alpha = 1;
                break;
            case LevelManager.GameStage.Level4:
                _bossSpritesCG[2].alpha = 1;
                break;
        }
    }

    public void Bind(float percentage)
    {
        _hpSlider ??= GetComponentInChildren<Slider>();
        _hpSlider.value = percentage;
        _level1LifeDisplayText.SetText("x" +_level1EnemiesLife.RunTimeValue);
    }

    public void SetHpData(IntGameData intGameData)
    {
        _HpData = intGameData;
    }
}
