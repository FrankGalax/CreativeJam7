using UnityEngine;
using System.Collections;

public enum EDouxDouxUpgrades
{
    EDouxDouxUpgrades_OFFENCE,
    EDouxDouxUpgrades_DEFENCE
}

struct SPowerUP
{
    public uint price;
    public float coolDown;

}
public class FatAssShip : MonoBehaviour
{
    [SerializeField]
    SPowerUP offensePowerUp;
    [SerializeField]
    SPowerUP defensePowerUp;

    [SerializeField]
    float pickShitUpDist = 50.0f;

    float m_offenseCD;
    float m_defenseCD;


    // Use this for initialization
    void Start()
    {
        m_offenseCD = 0.0f;
        m_defenseCD = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_offenseCD > 0.0f)
        {
            m_offenseCD -= Time.deltaTime;
        }
        else if (m_defenseCD > 0.0f)
        {
            m_defenseCD -= Time.deltaTime;
        }

        if (INetwork.Instance.IsMine(gameObject))
        {
            CashSale[] benDuCashSale = GameObject.FindObjectsOfType<CashSale>();
            foreach (CashSale cash in benDuCashSale)
            {
                if (Vector3.SqrMagnitude(cash.transform.position - transform.position) > pickShitUpDist)
                {
                    Game.Instance.Resources += cash.GetValue();
                    INetwork.Instance.NetworkDestroy(cash.gameObject);
                }
            }
        }
    }
    public void OnDamageTaken()
    {
        StunComponent sc = GetComponent<StunComponent>();
        if (sc != null)
        {
            sc.GetStunned();
        }
    }

    public void SpendDaMoneyz(EDouxDouxUpgrades canIHazPls)
    {
        if (CanIHazPls(canIHazPls))
        {
            VaisseauQuiJoueAfuckinMinecraft[] ships = GameObject.FindObjectsOfType<VaisseauQuiJoueAfuckinMinecraft>();
            switch (canIHazPls)
            {
                case EDouxDouxUpgrades.EDouxDouxUpgrades_OFFENCE:
                    foreach (VaisseauQuiJoueAfuckinMinecraft ship in ships)
                    {
                        ship.AndMyAxe();
                    }
                    m_offenseCD = offensePowerUp.coolDown;
                    break;
                case EDouxDouxUpgrades.EDouxDouxUpgrades_DEFENCE:
                    foreach (VaisseauQuiJoueAfuckinMinecraft ship in ships)
                    {
                        ship.ImRubberYoureGlue();
                    }
                    m_defenseCD = defensePowerUp.coolDown;
                    break;
            }
        }
    }

    public bool CanIHazPls(EDouxDouxUpgrades iReallyReallyNeedThis)
    {
        switch (iReallyReallyNeedThis)
        {
            case EDouxDouxUpgrades.EDouxDouxUpgrades_OFFENCE:
                return Game.Instance.Resources >= offensePowerUp.price && m_offenseCD >= 0.0f;
            case EDouxDouxUpgrades.EDouxDouxUpgrades_DEFENCE:
                return Game.Instance.Resources >= defensePowerUp.price && m_defenseCD >= 0.0f;
        }
        return false;
    }
}