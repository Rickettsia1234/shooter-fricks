using UnityEngine;
using UnityEngine.UI;

public class DeadManager : MonoBehaviour
{
    [SerializeField] private Text textScore;

    private void Start()
    {
        GameSystem.Instance.SetState(GameState.Dead);
        textScore.text = "Your Score : " + GameSystem.Instance.Score.ToString();
    }

    public void GoTitle()
    {
        GameSystem.Instance.ChangeScene(SceneName.Title);
    }
}