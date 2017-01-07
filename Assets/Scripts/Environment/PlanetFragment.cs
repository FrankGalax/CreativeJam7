using UnityEngine;
using System.Collections;

public class PlanetFragment : MonoBehaviour {

    public int TotalPointValue = 0;
    public float GrosCashDropRate = 0.0f;

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
        GameObject layer1 =  transform.Find("Layer1Graphics/Layer1Fragment").gameObject;
        if (layer1.GetComponent<Renderer>().enabled)
        {
            layer1.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            GameObject layer2 = transform.Find("Layer2Graphics/Layer2Fragment").gameObject;
            if (layer2.GetComponent<Renderer>().enabled)
            {
                layer2.GetComponent<Renderer>().enabled = false;
                Instantiate(ResourceManager.GetPrefab("Volcano"), layer2.transform.position, layer2.transform.rotation);
            }
        }
    }

    private void GenerateResources()
    {
        uint grosCashValue = ResourceManager.GetPrefab("GrosCashSale").GetComponent<GrosCashSale>().GetValue();
        uint petiteMonnaieValue = ResourceManager.GetPrefab("PetiteMonnaie").GetComponent<PetitCashSale>().GetValue();

        uint cumulativePointValueSpawned = 0;
        while (cumulativePointValueSpawned < TotalPointValue)
        {
            Vector3 direction = Quaternion.AngleAxis(Random.Range(-45, 45), Vector3.right) * transform.up;
            direction = Quaternion.AngleAxis(Random.Range(-45, 45), Vector3.forward) * direction;
            direction.Normalize();

            if (cumulativePointValueSpawned + grosCashValue <= TotalPointValue && Random.Range(0.0f, 1.0f) < GrosCashDropRate)
            {
                GameObject grosCashSale = (GameObject)Instantiate(ResourceManager.GetPrefab("GrosCashSale"), transform.position, transform.rotation);
                grosCashSale.GetComponent<Rigidbody>().velocity = direction;
                cumulativePointValueSpawned += grosCashValue;
            }
            else
            {
                GameObject petiteMonnaie = (GameObject)Instantiate(ResourceManager.GetPrefab("PetiteMonnaie"), transform.position, transform.rotation);
                petiteMonnaie.GetComponent<Rigidbody>().velocity = direction;
                cumulativePointValueSpawned += petiteMonnaieValue;
            }
        }
    }
}
