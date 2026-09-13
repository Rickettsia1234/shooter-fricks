using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        GameSystem.Instance.SetState(GameState.Title);
    }

    public void StartGame()
    {
        GameSystem.Instance.ChangeScene(SceneName.Game);
    }
}