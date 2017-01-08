using UnityEngine;

public class MenuMusic : GameSingleton<MenuMusic>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}