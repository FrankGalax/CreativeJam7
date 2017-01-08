using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : GameSingleton<Game>
{
    public List<PlanetZone> Zones;
    public int MaxDestroyedZones = 20;
    public int Resources { get; set; }
    public int NbResourcesToWin = 100;
    public float MaxGameTime = 180.0f;
    public float GameTime { get; private set; }

    public GameObject WinCanvas;
    public GameObject GameOverCanvas;
    public GameObject GameUICanvas { get; set; }
    public int AttackUpgradeCost = 10;
    public int DefenseUpgradeCost = 10;

    private bool m_GameEnded;

    void Start()
    {
        m_GameEnded = false;
        GameTime = MaxGameTime;
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
        
        GameTime -= Time.deltaTime;
        if (GameTime < 0)
        {
            GameTime = 0;
            if (Resources >= NbResourcesToWin)
            {
                Win();
            }
            else
            {
                GameOver();
            }
        }
    }

    private void GameOver(PlanetZone destroyedZone)
    {
        m_GameEnded = true;
        Destroy(GameUICanvas);
        GameOverCanvas.SetActive(true);
        GameOverCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + Resources + " PLANET SHARD" + (Resources > 1 ? "S" : "");
    }

    private void GameOver()
    {
        m_GameEnded = true;
        Destroy(GameUICanvas);
        GameOverCanvas.SetActive(true);
        GameOverCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + Resources + " PLANET SHARD" + (Resources > 1 ? "S" : "");
    }

    private void Win()
    {
        m_GameEnded = true;
        Destroy(GameUICanvas);
        WinCanvas.SetActive(true);
        WinCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + Resources + " PLANET SHARD" + (Resources > 1 ? "S" : "");
    }

    public void MainMenu()
    {
        INetwork.Instance.Disconnect();
        SceneManager.LoadScene("mainMenu");
    }
}
