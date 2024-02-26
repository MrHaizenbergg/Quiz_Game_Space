using UnityEngine;
using UnityEngine.UI;
using YG;

public class ScoreHandler : MonoBehaviour
{
    public int maxScore;
    public int currentScore;
    public int recordScore;

    [SerializeField] private Text _maxScoreText;
    [SerializeField] private Text _currentScoreText;
    [SerializeField] private Text _endLevelScoreText;
    [SerializeField] private Text _recordScoreText;

    private void Start()
    {
        _maxScoreText.text = maxScore.ToString();
        recordScore = PlayerPrefs.GetInt("Record", 0);
        _recordScoreText.text = "��� ������: " + recordScore;
        _currentScoreText.text = "������ �����: " + 0;
    }

    public void IsTrueAnswer()
    {
        currentScore++;
        _currentScoreText.text = "������ �����: " + currentScore;
        Debug.Log("TrueAnswer");
    }

    public void ShowScoreEndLevel()
    {
        _maxScoreText.text = "�������� ����� ������ � �������: " + maxScore;
        _endLevelScoreText.text = "���� ���� ������: " + currentScore;
    }

    public void InstallAndShowRecord()
    {
        if (currentScore > recordScore)
        {
            recordScore = currentScore;
            PlayerPrefs.SetInt("Record", recordScore);
            YandexGame.savesData.recordScore = recordScore;
            YandexGame.NewLeaderboardScores("record", recordScore);
        }
        //else

        Debug.Log("CurrentScore: " + currentScore);
        Debug.Log("RecordScore: " + recordScore);
        _recordScoreText.text = "��� ������: " + recordScore;
    }

    public void IsFalseAnswer()
    {
        Debug.Log("FalseAnswer");
    }
}
