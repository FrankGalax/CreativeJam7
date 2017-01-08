using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon;

public class MainMenu : PunBehaviour
{
    private string m_Username;
    private Button m_PlayButton;

    void Awake()
    {
        INetwork.Instance.SetAutomaticallySyncScene(true);
    }

    void Start()
    {
        m_PlayButton = Transform.FindObjectOfType<Canvas>().transform.Find("PlayButton").GetComponent<Button>();
        m_PlayButton.interactable = false;
    }

    public void SetUsername(string username)
    {
        m_Username = username;
        m_PlayButton.interactable = username.Length > 0;
    }

    public void Play()
    {
        INetwork.Instance.Connect();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Photon : Joined lobby");
        INetwork.Instance.SetPlayerName(m_Username);
        if (Transform.FindObjectOfType<Toggle>())
            INetwork.Instance.SetShipId(Transform.FindObjectOfType<Toggle>().isOn ? 1 : 0);
        INetwork.Instance.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Photon : Joined Room");
        INetwork.Instance.LoadLevel("Lobby");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
