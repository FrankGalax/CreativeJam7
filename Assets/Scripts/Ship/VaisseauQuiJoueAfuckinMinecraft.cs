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
            target.TakeDamage();
        }
    }

    PlanetFragment FindBestTarget()
    {
        PlanetFragment[] fragments = GameObject.FindObjectsOfType<PlanetFragment>();
        float bestDot = float.MaxValue;
        PlanetFragment bestFrag = null;

        Planet planet = GameObject.FindObjectOfType<Planet>();

        if (planet == null)
            return bestFrag;

        foreach (PlanetFragment fragment in fragments)
        {
            float distanceFuckingRadial = Vector3.Angle(transform.position - planet.transform.position, fragment.transform.position - planet.transform.position) * Mathf.Deg2Rad * planet.Radius;

            if (distanceFuckingRadial <= m_fuckShitUpRange && fragment.GetComponentInChildren<Renderer>().isVisible)
            {
                Camera cam = Camera.main;
                float dot = Vector3.Dot(cam.transform.forward, fragment.transform.position - cam.transform.forward);
                
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
