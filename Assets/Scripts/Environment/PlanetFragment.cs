using UnityEngine;
using System.Collections;

public class PlanetFragment : MonoBehaviour {

    public int TotalPointValue = 0;
    public float GrosCashNormalDropRate = 0.0f;
    public float GrosCashMegaLootDropRate = 0.0f;
    public float VolcanoSpawnRate = 0.0f;
    public float RepawnTime = 0.0f;
    public float SpawnOffset = 1.0f;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void HandleDeath()
    {
        GameObject layer1 = transform.Find("graphics/Layer1Fragment").gameObject;
        layer1.GetComponent<Renderer>().enabled = false;
        GenerateResources(true);

        Timer.Instance.Request(RepawnTime, () => Respawn());
    }

    public void Respawn()
    {
        GetComponentInParent<DamageComponent>().Revive();
    }
    
    public bool IsDestroyed()
    {
        return GetComponentInParent<DamageComponent>().GetIsDead();
    }

    public void OnDamageTaken()
    {
        if (!GetComponentInParent<DamageComponent>().GetIsDead())
        {
            if (Random.Range(0.0f, 1.0f) < VolcanoSpawnRate)
            {
                // TODO : don't instanciate if on top of an other one
                GameObject volcano = (GameObject)Instantiate(ResourceManager.GetPrefab("Volcano"), transform.position, transform.rotation);
                volcano.transform.parent = GameObject.Find("Volcanos").transform;
            }
            GenerateResources(false);
        }
    }

    private void GenerateResources(bool bigLoot)
    {
        uint grosCashValue = ResourceManager.GetPrefab("GrosCashSale").GetComponent<GrosCashSale>().GetValue();
        uint petiteMonnaieValue = ResourceManager.GetPrefab("PetiteMonnaie").GetComponent<PetitCashSale>().GetValue();
        float grosCashDropRate = bigLoot ? GrosCashMegaLootDropRate : GrosCashNormalDropRate;

        uint cumulativePointValueSpawned = 0;
        while (cumulativePointValueSpawned < TotalPointValue)
        {
            Vector3 direction = Quaternion.AngleAxis(Random.Range(-45, 45), Vector3.right) * transform.up;
            direction = Quaternion.AngleAxis(Random.Range(-45, 45), Vector3.forward) * direction;
            direction.Normalize();

            if (cumulativePointValueSpawned + grosCashValue <= TotalPointValue && Random.Range(0.0f, 1.0f) < grosCashDropRate)
            {
                GameObject grosCashSale = (GameObject)Instantiate(ResourceManager.GetPrefab("GrosCashSale"), transform.position, transform.rotation);
                grosCashSale.transform.parent = GameObject.Find("Cash").transform;
                grosCashSale.GetComponent<Rigidbody>().velocity = direction;
                cumulativePointValueSpawned += grosCashValue;
            }
            else
            {
                GameObject petiteMonnaie = (GameObject)Instantiate(ResourceManager.GetPrefab("PetiteMonnaie"), transform.position, transform.rotation);
                petiteMonnaie.transform.parent = GameObject.Find("Cash").transform;
                petiteMonnaie.GetComponent<Rigidbody>().velocity = direction;
                cumulativePointValueSpawned += petiteMonnaieValue;
            }
        }
    }
}
