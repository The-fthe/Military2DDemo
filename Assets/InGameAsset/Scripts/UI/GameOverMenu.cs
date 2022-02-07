using System;
using InGameAsset.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Button RestartButton;
    public Button MainMenuButton;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] TextMeshProUGUI _textMeshProUGUI;

    void Start()
    {
        RestartButton.onClick.AddListener(HandleRestartClicked);
        MainMenuButton.onClick.AddListener(HandleBackToMainMenu);
    }

    void OnEnable()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        if (_levelManager != null)
        {
            if (_levelManager.IsLost)
                _textMeshProUGUI.SetText("任務失敗");
            else if (_levelManager.IsWin)
                _textMeshProUGUI.SetText("任務完了");
        }
    }

    void HandleRestartClicked()
    {
        _levelManager.RestatGame();
    }

    void HandleBackToMainMenu()
    {
        _levelManager.BackToMainMenu();
    }
}