using UnityEngine;
using System.Collections;

public class Volcano : MonoBehaviour {

    public float Lifetime = 10.0f;
    public float Length = 0.0f;
    public float Radius = 0.0f;
    public float EruptionDelay = 0.0f;

    bool m_erupted = false;
    
    // Use this for initialization
    void Start ()
    {
        transform.Rotate(Vector3.right, 90);

        ParticleSystem.EmissionModule emission = GetComponentInChildren<ParticleSystem>().emission;
        emission.enabled = false;

        if (INetwork.Instance.IsMaster())
        {
            Timer.Instance.Request(EruptionDelay, StartEruption);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_erupted)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, transform.up * Length + transform.position, out hit))
            {
                DamageComponent damageComponent = hit.collider.GetComponent<DamageComponent>();
                if (damageComponent)
                {
                    INetwork.Instance.RPC(damageComponent.gameObject, "TakeDamage", PhotonTargets.MasterClient);
                }
            }
        }
    }

    public void StartEruption()
    {
        Timer.Instance.Request(Lifetime * 0.8f, () => { INetwork.Instance.RPC(gameObject, "StopLight", PhotonTargets.All); });
        Timer.Instance.Request(Lifetime, () => { INetwork.Instance.NetworkDestroy(gameObject); });
        INetwork.Instance.RPC(gameObject, "Erupt", PhotonTargets.All);
    }

    [PunRPC]
    public void Erupt()
    {
        ParticleSystem.EmissionModule emission = GetComponentInChildren<ParticleSystem>().emission;
        emission.enabled = true;

        m_erupted = true;
    }

    [PunRPC]
    public void StopLight()
    {
        GetComponentInChildren<Light>().enabled = false;
    }
}
