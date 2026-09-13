using UnityEngine;
using UnityEngine.UI;

public class ClearManager : MonoBehaviour
{
    [SerializeField] private Text textScore;
    [SerializeField] private AudioClip bgmClear;

    private void Start()
    {
        GameSystem.Instance.SetState(GameState.Clear);
        textScore.text = "Your Score : " + GameSystem.Instance.Score.ToString();
        AudioManager.Instance.SetBGM(bgmClear);
        AudioManager.Instance.PlayOneBGM();
    }

    public void GoTitle()
    {
        GameSystem.Instance.ChangeScene(SceneName.Title);
    }
}