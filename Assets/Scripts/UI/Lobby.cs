using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Lobby : MonoBehaviour
{
    public string GameSceneName;

    private List<Text> m_PlayerTexts;
    private Button m_ReadyButton;
    private int m_NbPlayerReady;

    void Start()
    {
        INetwork network = INetwork.Instance;
        if (!network.IsConnected)
        {
            Destroy(GameMusic.Instance.gameObject);
            SceneManager.LoadScene("MainMenu");
            return;
        }

        int nbPlayers = network.GetPlayerCount() - 1;
        network.SetId(nbPlayers);

        Transform canvas = FindObjectOfType<Canvas>().transform;
        m_PlayerTexts = new List<Text>();
        m_PlayerTexts.Add(canvas.Find("MothershipPanel").Find("Text").GetComponent<Text>());
        m_PlayerTexts.Add(canvas.Find("FirstShipPanel").Find("Text").GetComponent<Text>());
        m_PlayerTexts.Add(canvas.Find("SecondShipPanel").Find("Text").GetComponent<Text>());
        m_PlayerTexts.Add(canvas.Find("ThirdShipPanel").Find("Text").GetComponent<Text>());

        m_PlayerTexts[nbPlayers].text = network.GetPlayerName();
        network.RPC(gameObject, "NotifyNewPlayer", PhotonTargets.OthersBuffered, network.GetPlayerName(), nbPlayers);

        m_ReadyButton = canvas.Find("ReadyButton").GetComponent<Button>();
    }

    public void ReadyButton()
    {
        m_ReadyButton.interactable = false;
        m_ReadyButton.transform.Find("Text").GetComponent<Text>().text = "Waiting...";
        INetwork.Instance.RPC(gameObject, "NotifyReady", PhotonTargets.MasterClient);
    }

    [PunRPC]
    public void NotifyNewPlayer(string username, int id)
    {
        m_PlayerTexts[id].text = username;
    }

    [PunRPC]
    public void NotifyReady()
    {
        m_NbPlayerReady++;
        if (m_NbPlayerReady == INetwork.Instance.GetPlayerCount())
        {
            INetwork.Instance.RPC(gameObject, "StartMusic", PhotonTargets.All);
            StartCoroutine(StartGame());
        }
    }

    [PunRPC]
    private void StartMusic()
    {
        Destroy(MenuMusic.Instance.gameObject);
        GameMusic.Instance.GetComponent<AudioSource>().Play();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.5f);

        INetwork.Instance.SetRoomStarted();
        INetwork.Instance.LoadLevel(GameSceneName);
    }
}