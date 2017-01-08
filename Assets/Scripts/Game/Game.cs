using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : GameSingleton<Game>
{
    public List<PlanetZone> Zones;
    public int MaxDestroyedZones = 20;
    public uint Resources { get; set; }
    public int NbResourcesToWin = 100;
    public float MaxGameTime = 180.0f;
    public float GameTime { get; private set; }

    public GameObject WinCanvas;
    public GameObject GameOverCanvas;
    public GameObject MothershipCanvas;
    public GameObject StandardShipCanvas;

    public int AttackUpgradeCost = 10;
    public int DefenseUpgradeCost = 10;

    private bool m_GameEnded;
    private int m_GameTimeSeconds;

    void Start()
    {
        m_GameEnded = false;
        GameTime = MaxGameTime;
        m_GameTimeSeconds = (int)GameTime;
        ColorizePlanet();
    }

    void Update()
    {
        if (!INetwork.Instance.IsMaster())
            return;

        if (m_GameEnded)
            return;

        int destroyedZonesCount = 0;

        foreach (PlanetZone zone in Zones)
        {
            if (zone.Fragments.All(p => p.IsDestroyed()))
            {
                INetwork.Instance.RPC(gameObject, "GameOverZone", PhotonTargets.All, zone.Name);
                return;
            }

            destroyedZonesCount += zone.Fragments.Count(p => p.IsDestroyed());
        }

        if (destroyedZonesCount >= MaxDestroyedZones)
        {
            INetwork.Instance.RPC(gameObject, "GameOver", PhotonTargets.All);
            return;
        }
        
        GameTime -= Time.deltaTime;
        int gameTimeSeconds = (int)GameTime;
        if (gameTimeSeconds != m_GameTimeSeconds)
        {
            m_GameTimeSeconds = gameTimeSeconds;
            INetwork.Instance.RPC(gameObject, "UpdateGameTime", PhotonTargets.Others, GameTime);
        }

        if (GameTime < 0)
        {
            GameTime = 0;
            if (Resources >= NbResourcesToWin)
            {
                INetwork.Instance.RPC(gameObject, "Win", PhotonTargets.All);
            }
            else
            {
                INetwork.Instance.RPC(gameObject, "GameOver", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    private void GameOverZone(int destroyedZoneId)
    {
        m_GameEnded = true;
        MothershipCanvas.SetActive(false);
        StandardShipCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
        GameOverCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + Resources + " PLANET SHARD" + (Resources > 1 ? "S" : "");
    }

    [PunRPC]
    private void GameOver()
    {
        m_GameEnded = true;
        MothershipCanvas.SetActive(false);
        StandardShipCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
        GameOverCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + Resources + " PLANET SHARD" + (Resources > 1 ? "S" : "");
    }

    [PunRPC]
    private void Win()
    {
        m_GameEnded = true;
        MothershipCanvas.SetActive(false);
        StandardShipCanvas.SetActive(false);
        WinCanvas.SetActive(true);
        WinCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + Resources + " PLANET SHARD" + (Resources > 1 ? "S" : "");
    }

    [PunRPC]
    private void UpdateGameTime(float time)
    {
        GameTime = time;
    }

    public void MainMenu()
    {
        Destroy(GameMusic.Instance.gameObject);
        INetwork.Instance.Disconnect();
        SceneManager.LoadScene("mainMenu");
    }

    public void ColorizePlanet()
    {
        Zones.ForEach(zone =>
        {
            zone.Fragments.ForEach(fragment =>
            {
                fragment.PrimaryColor = zone.Color;
            });
        });
    }
}
