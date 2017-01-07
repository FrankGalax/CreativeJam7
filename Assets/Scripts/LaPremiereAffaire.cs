using UnityEngine;
using System.Collections;
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
	}
}
