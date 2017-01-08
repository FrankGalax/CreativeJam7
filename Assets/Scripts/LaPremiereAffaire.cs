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
        playerSpawnPoints.Add(playerSpawnPointsContainer.Find("SecondPlayerSpawnPoint"));
        playerSpawnPoints.Add(playerSpawnPointsContainer.Find("ThirdPlayerSpawnPoint"));

        int id = network.GetId();
        bool isMothership = id == 0;
        GameObject prefab = isMothership ? ResourceManager.GetPrefab("Mothership") : ResourceManager.GetPrefab("Standardship");
        if (network.GetPlayerCount() == 1 && network.GetShipId() == 1)
        {
            id = 1;
            isMothership = false;
            prefab = ResourceManager.GetPrefab("Standardship");
        }
        GameObject ship = INetwork.Instance.Instantiate(prefab, playerSpawnPoints[id].transform.position, playerSpawnPoints[id].transform.rotation);
        
        if (isMothership)
        {
            ship.GetComponent<LeDouxPlayerController>().enabled = true;
        }
        else
        {
            ship.GetComponent<LeDouxDouxPlayerController>().enabled = true;
        }
        GameObject canvas = isMothership ? Game.Instance.MothershipCanvas : Game.Instance.StandardShipCanvas;
        canvas.SetActive(true);
	}
}
