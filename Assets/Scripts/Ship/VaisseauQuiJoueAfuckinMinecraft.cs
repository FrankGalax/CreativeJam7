using UnityEngine;
using System.Collections;

public class VaisseauQuiJoueAfuckinMinecraft : MonoBehaviour
{
    [SerializeField]
    float fuckShitUpBaseRange = 100f;
    [SerializeField]
    float fuckShitUpBonusRange = 50f;
    [SerializeField]
    float fuckShitUpBonusDuration = 5.0f;
    [SerializeField]
    float leaveMeTheFuckAloneBonusDuration = 5.0f;
    [SerializeField]
    float fuckShitUpMinRange = 20f;
    [SerializeField]
    GameObject daShields;

    [SerializeField]
    uint fuckShitUpNbMissile = 4;

    float m_fuckShitUpBonusRemainingDuration = 0.0f;
    float m_leaveMeTheFuckAloneBonusRemainingDuration = 0.0f;
    float m_fuckShitUpRange;

    PlanetFragment m_bestTarget;
    GameObject m_CooldownFX;
    private bool m_isShooting;
    private float m_ShootingTimer;
    public float ShootCooldown = 2.0f;

    // Use this for initialization
    void Start ()
    {
        m_fuckShitUpRange = fuckShitUpBaseRange;
        m_bestTarget = null;
        m_CooldownFX = transform.Find("CooldownFX").gameObject;
        m_CooldownFX.SetActive(false);
        daShields.GetComponent<Renderer>().enabled = false;
	}

    public void OnDamageTaken()
    {
        if (m_leaveMeTheFuckAloneBonusRemainingDuration > .0f) return;
        
        StunComponent sc = GetComponent<StunComponent>();
        if (sc != null)
        {
            sc.GetStunned();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_fuckShitUpBonusRemainingDuration > .0f)
        {
            m_fuckShitUpBonusRemainingDuration -= Time.deltaTime;
            if (m_fuckShitUpBonusRemainingDuration <= .0f)
            {
                m_fuckShitUpRange = fuckShitUpBaseRange;
            }
        }

        if (m_leaveMeTheFuckAloneBonusRemainingDuration > .0f)
        {
            m_leaveMeTheFuckAloneBonusRemainingDuration -= Time.deltaTime;

            Color shieldColor = daShields.GetComponent<Renderer>().material.color;
            shieldColor.a = Mathf.Min(0.5f, m_leaveMeTheFuckAloneBonusRemainingDuration / leaveMeTheFuckAloneBonusDuration);
            daShields.GetComponent<Renderer>().material.color = shieldColor;
            if (m_leaveMeTheFuckAloneBonusRemainingDuration <= .0f)
            {
                daShields.GetComponent<Renderer>().enabled = false;
            }
        }

        if (m_isShooting)
        {
            m_ShootingTimer -= Time.deltaTime;
            if (m_ShootingTimer < 0)
            {
                m_ShootingTimer = 0;
                m_isShooting = false;
            }
        }

        m_CooldownFX.SetActive(m_isShooting);
        m_CooldownFX.transform.rotation = transform.rotation;
    }

    public void AndMyAxe()
    {
        m_fuckShitUpBonusRemainingDuration = fuckShitUpBonusDuration;
        m_fuckShitUpRange = fuckShitUpBaseRange + fuckShitUpBonusRange;
    }

    public void ImRubberYoureGlue()
    {
        m_leaveMeTheFuckAloneBonusRemainingDuration = leaveMeTheFuckAloneBonusDuration;
        daShields.GetComponent<Renderer>().enabled = true;
    }

    public void PewPew()
    {
        if (m_isShooting)
            return;

        PlanetFragment target = FindBestTarget();
        if (target != null)
        {
            INetwork.Instance.RPC(gameObject, "LaunchMissile", PhotonTargets.All, INetwork.Instance.GetViewId(target.gameObject));
        }

        ImRubberYoureGlue();
    }

    [PunRPC]
    private void LaunchMissile(int targetId)
    {
        GameObject targetObj = INetwork.Instance.GetGameObjectWithView(targetId);
        if (targetObj == null)
            return;

        EarthSeekingMissile[] missiles = new EarthSeekingMissile[fuckShitUpNbMissile];
        for (uint missileIndex = 0; missileIndex < fuckShitUpNbMissile; missileIndex++)
        {
            Vector3 launchDirection = Quaternion.AngleAxis(Random.Range(-25, 25), transform.up) * ((missileIndex % 2 == 0 ? 1 : -1) * transform.right);
            GameObject missileGO = (GameObject)Instantiate(ResourceManager.GetPrefab("Missile"), transform.position, Quaternion.identity);
            missiles[missileIndex] = missileGO.GetComponent<EarthSeekingMissile>();
            missileGO.GetComponent<EarthSeekingMissile>().Launch(targetObj.GetComponent<PlanetFragment>(), launchDirection, transform.up, GetComponent<LeDouxDouxPlayerController>().m_Velocity);
        }

        foreach (EarthSeekingMissile missile in missiles)
        {
            missile.otherMissiles = missiles;
        }

        m_isShooting = true;
        m_ShootingTimer = ShootCooldown;
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
            float distanceFuckingRadial = Vector3.Angle(transform.position - planet.transform.position, fragment.GetComponent<Renderer>().bounds.center - planet.transform.position) * Mathf.Deg2Rad * planet.Radius;

            if (distanceFuckingRadial <= m_fuckShitUpRange && !fragment.IsDestroyed() && fragment.GetComponentInChildren<Renderer>().isVisible)
            {
                Vector3 distUnwrapperSurUnPlane = Vector3.ProjectOnPlane(fragment.GetComponent<Renderer>().bounds.center - transform.position, transform.up).normalized;
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
