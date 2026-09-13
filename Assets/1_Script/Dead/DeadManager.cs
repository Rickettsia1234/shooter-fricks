using UnityEngine;
using UnityEngine.UI;

public class DeadManager : MonoBehaviour
{
    [SerializeField] private Text textScore;
    [SerializeField] private AudioClip bgmDead;

    private void Start()
    {
        GameSystem.Instance.SetState(GameState.Dead);
        textScore.text = "Your Score : " + GameSystem.Instance.Score.ToString();
        AudioManager.Instance.SetBGM(bgmDead);
        AudioManager.Instance.PlayOneBGM();
    }

    public void GoTitle()
    {
        GameSystem.Instance.ChangeScene(SceneName.Title);
    }
}