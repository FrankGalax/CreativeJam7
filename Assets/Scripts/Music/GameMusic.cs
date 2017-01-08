using UnityEngine;

public class GameMusic : GameSingleton<GameMusic>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}