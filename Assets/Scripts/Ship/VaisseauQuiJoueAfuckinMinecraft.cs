using UnityEngine;
using System.Collections;

public class VaisseauQuiJoueAfuckinMinecraft : MonoBehaviour
{
    [SerializeField]
    float fuckShitUpBaseRange = 0.1f;
    [SerializeField]
    float fuckShitUpBonusRange = 0.05f;
    [SerializeField]
    float fuckShitUpBonusDuration = 5.0f;
    [SerializeField]
    uint fuckShitUpNbMissile = 4;

    float m_fuckShitUpBonusRemainingDuration = 0.0f;
    float m_fuckShitUpRange;

    PlanetFragment m_bestTarget;

    // Use this for initialization
    void Start ()
    {
        m_fuckShitUpRange = fuckShitUpBaseRange;
        m_bestTarget = null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_fuckShitUpBonusRemainingDuration > .0f)
        {
            m_fuckShitUpBonusRemainingDuration -= Time.deltaTime;
            if (m_fuckShitUpBonusRemainingDuration >= .0f)
            {
                m_fuckShitUpRange = fuckShitUpBaseRange;
            }
        }
	}

    public void AndMyAxe()
    {
        m_fuckShitUpBonusRemainingDuration = fuckShitUpBonusDuration;
        m_fuckShitUpRange = fuckShitUpBaseRange + fuckShitUpBonusRange;
    }

    public void ImRubberYoureGlue()
    {

    }

    public void PewPew()
    {
        PlanetFragment target = FindBestTarget();
        if (target != null)
        {
            for (uint missileIndex = 0; missileIndex < fuckShitUpNbMissile; missileIndex++)
            {
                Vector3 launchDirection = Quaternion.AngleAxis(Random.Range(-25, 25), transform.up) * ((missileIndex % 2 == 0 ? 1 : -1) * transform.right);
                GameObject missileGO = INetwork.Instance.Instantiate(ResourceManager.GetPrefab("Missile"), transform.position, Quaternion.identity);
                INetwork.Instance.RPC(missileGO, "Launch", PhotonTargets.MasterClient, target, launchDirection, transform.up, GetComponent<LeDouxDouxPlayerController>().m_Velocity);
            }
        }
    }

    PlanetFragment FindBestTarget()
    {
        PlanetFragment[] fragments = GameObject.FindObjectsOfType<PlanetFragment>();
        float bestDot = float.MinValue;
        PlanetFragment bestFrag = null;

        Planet planet = GameObject.FindObjectOfType<Planet>();

        if (planet == null)
            return bestFrag;

        foreach (PlanetFragment fragment in fragments)
        {
            float distanceFuckingRadial = Vector3.Angle(transform.position - planet.transform.position, fragment.transform.position - planet.transform.position) * Mathf.Deg2Rad * planet.Radius;

            if (distanceFuckingRadial <= m_fuckShitUpRange && fragment.GetComponentInChildren<Renderer>().isVisible)
            {
                Vector3 distUnwrapperSurUnPlane = Vector3.ProjectOnPlane(fragment.gameObject.transform.position - transform.position, transform.up).normalized;
                float dot = Vector3.Dot(transform.forward, distUnwrapperSurUnPlane) / distanceFuckingRadial;

                if (dot > bestDot && (Vector3.Angle(distUnwrapperSurUnPlane, transform.forward) <= 60.0f || distanceFuckingRadial < 50.0f))
                {
                    bestDot = dot;
                    bestFrag = fragment;
                }
            }
        }
        return bestFrag;
    }

    void OnCollisionEnter(Collision collision)
    {

    }
}
