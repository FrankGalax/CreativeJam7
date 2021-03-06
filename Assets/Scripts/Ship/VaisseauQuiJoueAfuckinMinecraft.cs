﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VaisseauQuiJoueAfuckinMinecraft : MonoBehaviour
{
    [SerializeField]
    float fuckShitUpBaseRange = 100f;
    [SerializeField]
    float gottaGoFastBonusDuration = 5.0f;
    [SerializeField]
    float leaveMeTheFuckAloneBonusDuration = 5.0f;
    [SerializeField]
    float fuckShitUpMinRange = 20f;
    [SerializeField]
    GameObject daShields;

    [SerializeField]
    uint fuckShitUpNbMissile = 4;

    float m_gottaGoFastBonusRemainingDuration = 0.0f;
    float m_leaveMeTheFuckAloneBonusRemainingDuration = 0.0f;
    float m_fuckShitUpRange;

    PlanetFragment m_bestTarget;
    List<GameObject> m_CooldownFXs;
    GameObject m_TrusterFX;
    private bool m_isShooting;
    private float m_ShootingTimer;
    public float ShootCooldown = 2.0f;

    // Use this for initialization
    void Start ()
    {
        m_fuckShitUpRange = fuckShitUpBaseRange;
        m_bestTarget = null;
        daShields.GetComponent<Renderer>().enabled = false;
        m_CooldownFXs = new List<GameObject>();
        m_CooldownFXs.Add(transform.Find("CooldownFX1").gameObject);
        m_CooldownFXs.Add(transform.Find("CooldownFX2").gameObject);
        m_CooldownFXs.ForEach(p => p.SetActive(false));
        m_TrusterFX = transform.Find("TrusterFX").gameObject;
        m_TrusterFX.SetActive(false);

        if (INetwork.Instance.IsMine(gameObject))
        {
            int id = INetwork.Instance.GetId();
            INetwork.Instance.RPC(gameObject, "SetColor", PhotonTargets.AllBuffered, id);
        }
    }

    [PunRPC]
    private void SetColor(int id)
    {
        List<MeshRenderer> meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        switch (id)
        {
            case 1:
                meshRenderers.ForEach(p => p.material.color = Color.red);
                break;
            case 2:
                meshRenderers.ForEach(p => p.material.color = Color.green);
                break;
            case 3:
                meshRenderers.ForEach(p => p.material.color = Color.blue);
                break;
        }
    }

    public void OnDamageTaken()
    {
        if (m_leaveMeTheFuckAloneBonusRemainingDuration > .0f) return;

        StunComponent sc = GetComponent<StunComponent>();
        if (sc != null)
        {
            INetwork.Instance.RPC(gameObject, "GetStunned", PhotonTargets.All);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_gottaGoFastBonusRemainingDuration > .0f)
        {
            m_gottaGoFastBonusRemainingDuration -= Time.deltaTime;
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

        m_CooldownFXs.ForEach(p =>
        {
            p.SetActive(m_isShooting);
        });

        if (!IsGoingFast() && m_TrusterFX.GetActive())
        {
            m_TrusterFX.SetActive(false);
        }
    }

    [PunRPC]
    public void GottaGoFast()
    {
        m_gottaGoFastBonusRemainingDuration = gottaGoFastBonusDuration;
        m_TrusterFX.SetActive(true);

        transform.Find("PowerUpSound").GetComponent<AudioSource>().Play();
    }

    public bool IsGoingFast()
    {
        return m_gottaGoFastBonusRemainingDuration > .0f;
    }

    public bool IsRubberYourGlue()
    {
        return m_leaveMeTheFuckAloneBonusRemainingDuration > .0f;
    }


    [PunRPC]
    public void ImRubberYoureGlue()
    {
        m_leaveMeTheFuckAloneBonusRemainingDuration = leaveMeTheFuckAloneBonusDuration;
        daShields.GetComponent<Renderer>().enabled = true;

        transform.Find("PowerUpSound").GetComponent<AudioSource>().Play();
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

        GetComponent<AudioSource>().Play();
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
