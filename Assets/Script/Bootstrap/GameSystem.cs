using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    private static GameSystem instance;
    public static GameSystem Instance
    {
        get
        {
            return instance;
        }
    }

    public GameState State { get; private set; }
    public float MasterVolume { get; private set; } = 1.0f;
    public PlayerType PlayerType { get; private set; } = 0;
    [SerializeField] private Dictionary<SceneName, SceneAsset> SceneData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        State = GameState.Title;
        SceneChange(SceneName.Title);
    }

    private void SceneChange(SceneName sceneName)
    {
        if (SceneData.TryGetValue(sceneName, out SceneAsset scene))
        SceneManager.LoadScene(scene.name);
    }

    public void StartGame(PlayerType playerType)
    {
        PlayerType = playerType;
        State = GameState.Game;
        SceneChange(SceneName.Game);
    }
}