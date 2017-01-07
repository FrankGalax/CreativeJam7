using UnityEngine;
using System.Collections;

public class PlanetFragment : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage()
    {
        Destroy();
        GenerateResources();
    }

    private void Destroy()
    {
        Instantiate(ResourceManager.GetPrefab("Volcano"), transform.position, transform.rotation);
    }

    private void GenerateResources()
    {

    }
}
