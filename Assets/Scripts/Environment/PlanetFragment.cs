using UnityEngine;
using System.Collections;
using System;

public class PlanetFragment : MonoBehaviour {

    public int TotalPointValue = 0;
    public float GrosCashNormalDropRate = 0.0f;
    public float GrosCashMegaLootDropRate = 0.0f;
    public float RepawnTime = 0.0f;
    public float SpawnOffset = 1.0f;
    public float InitialCashVelocity = 1.0f;
    public float VolcanoRateAfterDestruction = 0.005f;

    GameObject m_Volcano;
    bool m_ShouldTryRespawn = false;

    public Color PrimaryColor
    {
        set
        {
            m_PrimaryColor = value;
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material.color = value;
        }
    }
    private Color m_PrimaryColor;

    // Use this for initialization
    void Start() {

    }
	
	// Update is called once per frame
	void Update () {
        if (INetwork.Instance.IsMaster())
        {
            if (!GetComponentInParent<Renderer>().enabled && UnityEngine.Random.Range(0.0f, 1.0f) < VolcanoRateAfterDestruction)
            {
                SpawnVolcano();
            }

            if (m_ShouldTryRespawn)
            {
                TryRespawn();
            }
        }
    }

    public void HandleDeath()
    {
        INetwork.Instance.RPC(gameObject, "HideFragment", PhotonTargets.All);

        if (INetwork.Instance.IsMaster())
        {
            SpawnVolcano();
            GenerateResources(true);
            Timer.Instance.Request(RepawnTime, TryRespawn);
        }
    }

    public void SpawnVolcano()
    {
        if (!m_Volcano)
        {
            Vector3 direction = GetComponent<Renderer>().bounds.center - transform.position;
            direction.Normalize();
            m_Volcano = INetwork.Instance.Instantiate(ResourceManager.GetPrefab("Volcano"), GetComponent<Renderer>().bounds.center, Quaternion.LookRotation(direction));
            INetwork.Instance.RPC(gameObject, "SetVolcanoParent", PhotonTargets.All, INetwork.Instance.GetViewId(m_Volcano));
        }
    }

    [PunRPC]
    private void SetVolcanoParent(int volcanoId)
    {
        GameObject volcano = INetwork.Instance.GetGameObjectWithView(volcanoId);
        volcano.transform.parent = GameObject.Find("Volcanos").transform;
    }

    [PunRPC]
    private void HideFragment()
    {
        GetComponentInParent<Renderer>().enabled = false;
    }

    public void TryRespawn()
    {
        if (!m_Volcano)
        {
            GetComponentInParent<DamageComponent>().Revive();
            INetwork.Instance.RPC(gameObject, "EnableRenderer", PhotonTargets.All);
            m_ShouldTryRespawn = false;
        }
        else
        {
            m_ShouldTryRespawn = true;
        }
    }

    [PunRPC]
    public void EnableRenderer()
    {
        GetComponentInParent<Renderer>().enabled = true;
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

            Vector3 direction = Quaternion.AngleAxis(UnityEngine.Random.Range(-45, 45), transform.right) * up;
            direction = Quaternion.AngleAxis(UnityEngine.Random.Range(-45, 45), transform.forward) * direction;
            direction.Normalize();
            Vector3 velocity = direction * InitialCashVelocity;

            if (cumulativePointValueSpawned + grosCashValue <= TotalPointValue && UnityEngine.Random.Range(0.0f, 1.0f) < grosCashDropRate)
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
