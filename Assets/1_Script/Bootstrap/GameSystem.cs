using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneDataPair
{
    public SceneName key;
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif
    public string sceneName;
}

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

    [field: SerializeField] public GameState State { get; private set; }
    [field: SerializeField] public float MasterVolume { get; private set; } = 1.0f;
    [field: SerializeField] public PlayerType PlayerType { get; private set; } = 0;
    [SerializeField] private List<SceneDataPair> SceneData;
    private readonly Dictionary<SceneName, string> sceneDictionary = new();
    [SerializeField] private int score;
    public int Score => score;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < SceneData.Count; i++)
        {
            var pair = SceneData[i];
            if (pair.sceneAsset != null)
            {
                pair.sceneName = pair.sceneAsset.name;
                SceneData[i] = pair;
            }
        }
    }
#endif

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var pair in SceneData)
        {
            sceneDictionary[pair.key] = pair.sceneName;
        }

        State = GameState.Title;
        ChangeScene(SceneName.Title);
    }

    public void ChangeScene(SceneName sceneName)
    {
        SceneManager.LoadScene(sceneDictionary[sceneName]);
    }

    public void SetState(GameState gameState)
    {
        State = gameState;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }
}