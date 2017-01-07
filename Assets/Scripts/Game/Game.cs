using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Game : GameSingleton<Game>
{
    public List<PlanetZone> Zones;
    public int MaxDestroyedZones = 20;
    public int Resources { get; set; }
    public int NbResourcesToWin = 100;

    private bool m_GameEnded;

    void Start()
    {
        m_GameEnded = false;
    }

    void Update()
    {
        if (m_GameEnded)
            return;

        int destroyedZonesCount = 0;

        foreach (PlanetZone zone in Zones)
        {
            if (zone.Fragments.All(p => p.IsDestroyed()))
            {
                GameOver(zone);
                return;
            }

            destroyedZonesCount += zone.Fragments.Count(p => p.IsDestroyed());
        }

        if (destroyedZonesCount >= MaxDestroyedZones)
        {
            GameOver();
            return;
        }

        if (Resources >= NbResourcesToWin)
        {
            Win();
            return;
        }
    }

    private void GameOver(PlanetZone destroyedZone)
    {
        m_GameEnded = true;
        Debug.Log("Game over");
    }

    private void GameOver()
    {
        m_GameEnded = true;
        Debug.Log("Game over");
    }

    private void Win()
    {
        m_GameEnded = true;
    }
}
