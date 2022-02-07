using TMPro;
using UnityEngine;

public class ScoreCounter:MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreTMP;
    [SerializeField] TextMeshProUGUI _GameOverMenuTMP;
        [SerializeField] int _eachScoreIncreament = 10;
    int _scoreNum;

    public int ScoreNum
    {
        get => _scoreNum; 
        set => _scoreNum = value;
    }

    public void ShowScoreText()
    {
        UpdateScoreBoard();
        _GameOverMenuTMP.SetText("スコア: "+ _scoreNum);
    }
    public void EveryScoreIncrease()
    {
        _scoreNum += _eachScoreIncreament;
        UpdateScoreBoard();
    }

    public void ResetScore()
    {
        _scoreNum = 0;
        UpdateScoreBoard();
    }
    void UpdateScoreBoard()
    {
        _scoreTMP.SetText($"スコア: {_scoreNum}");
    }
}