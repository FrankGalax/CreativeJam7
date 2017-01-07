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

    float m_fuckShitUpBonusRemainingDuration = 0.0f;
    float m_fuckShitUpRange;
    // Use this for initialization
    void Start ()
    {
        m_fuckShitUpRange = fuckShitUpBaseRange;
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
            target.GetComponentInParent<DamageComponent>().TakeDamage();
        }
    }

    PlanetFragment FindBestTarget()
    {
        PlanetFragment[] fragments = GameObject.FindObjectsOfType<PlanetFragment>();
        float bestDot = float.MaxValue;
        PlanetFragment bestFrag = null;
        float maxDistSqrt = m_fuckShitUpRange * m_fuckShitUpRange;

        foreach (PlanetFragment fragment in fragments)
        {
            Vector3 planeToFrag = fragment.transform.position - transform.position;
            float distSqrt = Vector3.SqrMagnitude(planeToFrag);

            if (distSqrt <= maxDistSqrt)
            {
                float dot = Vector3.Dot(transform.forward, planeToFrag);
                if (dot < bestDot)
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
