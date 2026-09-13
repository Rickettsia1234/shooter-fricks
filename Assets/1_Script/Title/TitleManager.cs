using System;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmTitle;

    private void Start()
    {
        GameSystem.Instance.SetState(GameState.Title);
        AudioManager.Instance.SetBGM(bgmTitle);
        AudioManager.Instance.PlayBGM();
    }

    public void StartGame()
    {
        GameSystem.Instance.ChangeScene(SceneName.Game);
    }
}