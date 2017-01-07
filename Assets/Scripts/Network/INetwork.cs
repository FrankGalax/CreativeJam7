using ExitGames.Client.Photon;
using UnityEngine;
using System;
using System.Linq;

public class INetwork : PunGameSingleton<INetwork>
{
    private bool m_IsSinglePlayer;

    public bool IsSinglePlayer
    {
        get
        {
            return m_IsSinglePlayer;
        }
        set
        {
            m_IsSinglePlayer = value;
            PhotonNetwork.offlineMode = m_IsSinglePlayer;
        }
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("1.0");
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void SetRoomStarted()
    {
        PhotonNetwork.room.IsOpen = false;
    }

    public void SetId(int id)
    {
        PhotonNetwork.player.SetCustomProperties(new Hashtable() { { "Id", id } });
    }

    public int GetId()
    {
        return PhotonNetwork.player.CustomProperties["Id"] == null ? -1 : int.Parse(PhotonNetwork.player.CustomProperties["Id"].ToString());
    }

    public void SetShipId(int id)
    {
        PhotonNetwork.player.SetCustomProperties(new Hashtable() { { "ShipId", id } });
    }

    public int GetShipId()
    {
        return PhotonNetwork.player.CustomProperties["ShipId"] == null ? -1 : int.Parse(PhotonNetwork.player.CustomProperties["ShipId"].ToString());
    }

    public void SetCharacterId(int characterId)
    {
        PhotonNetwork.player.SetCustomProperties(new Hashtable() { { "CharacterId", characterId } });
    }

    public int GetCharacterId()
    {
        return PhotonNetwork.player.CustomProperties["CharacterId"] == null ? -1 : int.Parse(PhotonNetwork.player.CustomProperties["CharacterId"].ToString());
    }

    public string GetOtherPlayerTeam(PhotonPlayer otherPlayer)
    {
        return otherPlayer.CustomProperties["Team"] == null ?
            null :
            otherPlayer.CustomProperties["Team"].ToString();
    }

    public void SetPlayerName(string name)
    {
        PhotonNetwork.player.NickName = name;
    }

    public int GetViewId(GameObject gameObject)
    {
        PhotonView pv = gameObject.GetComponent<PhotonView>();
        if (pv == null)
            return 0;
        return pv.viewID;
    }

    public GameObject GetGameObjectWithView(int viewId)
    {
        PhotonView pv = PhotonView.Find(viewId);
        if (pv == null)
            return null;
        return pv.gameObject;
    }

    public bool IsMine(GameObject gameObject)
    {
        return gameObject.GetComponent<PhotonView>().isMine;
    }

    public string GetPlayerName()
    {
        return PhotonNetwork.player.NickName;
    }

    public bool IsMaster()
    {
        return PhotonNetwork.player.IsMasterClient;
    }

    public void RPC(GameObject gameObject, string methodName, PhotonTargets target, params object[] parameters)
    {
        PhotonView pv = gameObject.GetComponent<PhotonView>();
        if (pv == null)
        {
            throw new UnityException("GameObject has no PhotonView. Cannot make RPC Call");
        }
        pv.RPC(methodName, target, parameters);
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (IsSinglePlayer)
            return GameObject.Instantiate(prefab, position, rotation) as GameObject;
        return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0);
    }

    public void NetworkDestroy(GameObject gameObject)
    {
        if (IsSinglePlayer)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void LoadLevel(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public void SetAutomaticallySyncScene(bool automaticallySyncScene)
    {
        PhotonNetwork.automaticallySyncScene = automaticallySyncScene;
    }

    public bool IsConnected
    {
        get { return PhotonNetwork.connected; }
    }

    public int GetPlayerCount()
    {
        return PhotonNetwork.playerList.Length;
    }

    public int GetPlayerCountInTeam(string team)
    {
        return PhotonNetwork.playerList.Count(
            p => p.CustomProperties.ContainsKey("team") && p.CustomProperties["team"].ToString().Equals(team)
        );
    }

    void OnPhotonRandomJoinFailed()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), options, null);
    }
}