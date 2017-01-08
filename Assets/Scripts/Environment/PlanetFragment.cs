using UnityEngine;
using System.Collections;

public class PlanetFragment : MonoBehaviour {

    public int TotalPointValue = 0;
    public float GrosCashNormalDropRate = 0.0f;
    public float GrosCashMegaLootDropRate = 0.0f;
    public float RepawnTime = 0.0f;
    public float SpawnOffset = 1.0f;
    public float InitialCashVelocity = 1.0f;

    GameObject m_Volcano;

    // Use this for initialization
    void Start() {

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void HandleDeath()
    {
        INetwork.Instance.RPC(gameObject, "HideFragment", PhotonTargets.All);

        if (INetwork.Instance.IsMaster())
        {
            if (!m_Volcano)
            {
                Vector3 direction = GetComponent<Renderer>().bounds.center - transform.position;
                direction.Normalize();
                m_Volcano = INetwork.Instance.Instantiate(ResourceManager.GetPrefab("Volcano"), GetComponent<Renderer>().bounds.center, Quaternion.LookRotation(direction));
                INetwork.Instance.RPC(gameObject, "SetVolcanoParent", PhotonTargets.All, new object[] { INetwork.Instance.GetViewId(m_Volcano), direction });
            }

            GenerateResources(true);

            Timer.Instance.Request(RepawnTime, () => Respawn());
        }
    }

    [PunRPC]
    private void SetVolcanoParent(int volcanoId, Vector3 direction)
    {
        GameObject volcano = INetwork.Instance.GetGameObjectWithView(volcanoId);
        volcano.transform.parent = GameObject.Find("Volcanos").transform;
        volcano.GetComponent<Volcano>().Direction = direction;

    }

    [PunRPC]
    private void HideFragment()
    {
        GetComponentInParent<Renderer>().enabled = false;
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
        GenerateResources(false);
    }

    private void GenerateResources(bool bigLoot)
    {
        uint grosCashValue = ResourceManager.GetPrefab("GrosCashSale").GetComponent<GrosCashSale>().GetValue();
        uint petiteMonnaieValue = ResourceManager.GetPrefab("PetiteMonnaie").GetComponent<PetitCashSale>().GetValue();
        float grosCashDropRate = bigLoot ? GrosCashMegaLootDropRate : GrosCashNormalDropRate;

        uint cumulativePointValueSpawned = 0;
        while (cumulativePointValueSpawned < TotalPointValue)
        {
            Vector3 center = GetComponent<Renderer>().bounds.center;
            Vector3 up = center - transform.position;

            Vector3 direction = Quaternion.AngleAxis(Random.Range(-45, 45), transform.right) * up;
            direction = Quaternion.AngleAxis(Random.Range(-45, 45), transform.forward) * direction;
            direction.Normalize();
            Vector3 velocity = direction * InitialCashVelocity;

            if (cumulativePointValueSpawned + grosCashValue <= TotalPointValue && Random.Range(0.0f, 1.0f) < grosCashDropRate)
            {
                GameObject grosCashSale = INetwork.Instance.Instantiate(ResourceManager.GetPrefab("GrosCashSale"), center, transform.rotation);
                INetwork.Instance.RPC(gameObject, "SetCashParams", PhotonTargets.All, new object[] { INetwork.Instance.GetViewId(grosCashSale), velocity });
                cumulativePointValueSpawned += grosCashValue;
            }
            else
            {
                GameObject petiteMonnaie = INetwork.Instance.Instantiate(ResourceManager.GetPrefab("PetiteMonnaie"), center, transform.rotation);
                INetwork.Instance.RPC(gameObject, "SetCashParams", PhotonTargets.All, new object[] { INetwork.Instance.GetViewId(petiteMonnaie), velocity });
                cumulativePointValueSpawned += petiteMonnaieValue;
            }
        }
    }

    [PunRPC]
    private void SetCashParams(int cashId, Vector3 velocity)
    {
        GameObject cash = INetwork.Instance.GetGameObjectWithView(cashId);
        cash.transform.parent = GameObject.Find("Cash").transform;
        cash.GetComponent<Rigidbody>().velocity = velocity;
    }
}
