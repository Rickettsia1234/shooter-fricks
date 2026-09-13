using UnityEngine;
using UnityEngine.UI;

public class DrawManager : MonoBehaviour
{
    [SerializeField] private Text textScore;
    [SerializeField] private Text textTime;

    public void DrawScore(int score)
    {
        textScore.text = $"DESTROY SHOOTER {35 - score:N0}!";
    }

    public void DrawTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        textTime.text = $"TIME : {minutes:D2}:{remainingSeconds:D2}";
    }
}