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

        INetwork.Instance.Instantiate(ResourceManager.GetPrefab("Mothership"), playerSpawnPoints[0].transform.position, playerSpawnPoints[0].transform.rotation);
	}
}
