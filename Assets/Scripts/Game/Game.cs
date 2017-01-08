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
    public float EndGameExplosionSpeed = 0.5f;

    public GameObject WinCanvas;
    public GameObject GameOverCanvas;
    public GameObject MothershipCanvas;
    public GameObject StandardShipCanvas;
    public GameObject EndGameExplosionSphere;
    public GameObject Planet;

    public int AttackUpgradeCost = 10;
    public int DefenseUpgradeCost = 10;

    private bool m_GameEnded;
    private bool m_GameLostExplosion;
    private int m_GameTimeSeconds;

    void Start()
    {
        m_GameEnded = false;
        m_GameLostExplosion = false;
        GameTime = MaxGameTime;
        m_GameTimeSeconds = (int)GameTime;
        ColorizePlanet();
    }

    void Update()
    {
        if (m_GameLostExplosion)
        {
            if (EndGameExplosionSphere.transform.localScale.x > 45.0f)
            {
                return;
            }

            EndGameExplosionSphere.transform.localScale += new Vector3(EndGameExplosionSpeed, EndGameExplosionSpeed, EndGameExplosionSpeed);
            if (Planet && EndGameExplosionSphere.transform.localScale.x > Planet.GetComponent<Planet>().Radius)
            {
                foreach (Renderer renderer in Planet.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }
            }
        }

        if (!INetwork.Instance.IsMaster())
            return;

        if (m_GameEnded)
            return;

        int destroyedZonesCount = 0;

        foreach (PlanetZone zone in Zones)
        {
            if (zone.Fragments.All(p => p.IsDestroyed()))
            {
                INetwork.Instance.RPC(gameObject, "GameOver", PhotonTargets.All, 0, zone.Name, (int)Resources);
                return;
            }

            destroyedZonesCount += zone.Fragments.Count(p => p.IsDestroyed());
        }

        if (destroyedZonesCount >= MaxDestroyedZones)
        {
            INetwork.Instance.RPC(gameObject, "GameOver", PhotonTargets.All, 1, string.Empty, (int)Resources);
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
                INetwork.Instance.RPC(gameObject, "Win", PhotonTargets.All, (int)Resources);
            }
            else
            {
                INetwork.Instance.RPC(gameObject, "GameOver", PhotonTargets.All, 2, string.Empty, (int)Resources);
            }
        }
    }

    [PunRPC]
    private void GameOver(int reason, string zone, int resources)
    {
        m_GameEnded = true;
        MothershipCanvas.SetActive(false);
        StandardShipCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
        GameOverCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + resources + " PLANET SHARD" + (resources > 1 ? "S" : "");
        Text reasonText = GameOverCanvas.transform.Find("Reason").GetComponent<Text>();
        switch (reason)
        {
            case 0:
                reasonText.text = "YOU MINED ALL THE RESOURCES IN THE " + zone + " ZONE TOO QUICKLY.";
                break;
            case 1:
                reasonText.text = "YOU MINED TOO MANY RESOURCES. THE PLANET'S CORE EXPLODED.";
                m_GameLostExplosion = true;
                break;
            case 2:
            default:
                reasonText.text = "YOU DID NOT MINE ENOUGH RESOURCES.";
                break;
        }

        transform.Find("GameOverSound").GetComponent<AudioSource>().Play();
    }

    [PunRPC]
    private void Win(uint resources)
    {
        m_GameEnded = true;
        MothershipCanvas.SetActive(false);
        StandardShipCanvas.SetActive(false);
        WinCanvas.SetActive(true);
        WinCanvas.transform.Find("ResourceText").GetComponent<Text>().text = "YOU GOT " + resources + " PLANET SHARD" + (resources > 1 ? "S" : "");
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
