using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private PlayerType playerType = PlayerType.Plain1;

    public void StartGame()
    {
        GameSystem.Instance.StartGame(playerType);
    }
}