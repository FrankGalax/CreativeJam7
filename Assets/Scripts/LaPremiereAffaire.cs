using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LaPremiereAffaire : MonoBehaviour
{
	void Start ()
    {
        INetwork network = INetwork.Instance;
        if (!network.IsConnected)
        {
            SceneManager.LoadScene("mainMenu");
            return;
        }

        List<Transform> playerSpawnPoints = new List<Transform>();
        Transform playerSpawnPointsContainer = GameObject.Find("PlayerSpawnPoints").transform;
        playerSpawnPoints.Add(playerSpawnPointsContainer.Find("MothershipSpawnPoint"));
        playerSpawnPoints.Add(playerSpawnPointsContainer.Find("FirstPlayerSpawnPoint"));

        int id = network.GetId();
        GameObject prefab = id == 0 ? ResourceManager.GetPrefab("Mothership") : ResourceManager.GetPrefab("Standardship");
        if (network.GetPlayerCount() == 1 && network.GetShipId() == 1)
        {
            id = 1;
            prefab = ResourceManager.GetPrefab("Standardship");
        }
        INetwork.Instance.Instantiate(prefab, playerSpawnPoints[id].transform.position, playerSpawnPoints[id].transform.rotation);
	}
}
