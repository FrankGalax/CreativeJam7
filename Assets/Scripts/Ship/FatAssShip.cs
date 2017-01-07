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
    uint m_duGrosCashSale = 10;

    [SerializeField]
    SPowerUP offensePowerUp;
    [SerializeField]
    SPowerUP defensePowerUp;

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
    }

    void OnCollisionEnter(Collision collision)
    {
        CashSale cashSale = collision.collider.gameObject.GetComponent<CashSale>();
        if (cashSale != null)
        {
            m_duGrosCashSale += cashSale.GetValue();
            Destroy(collision.collider.gameObject);
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
                return m_duGrosCashSale >= offensePowerUp.price && m_offenseCD >= 0.0f;
            case EDouxDouxUpgrades.EDouxDouxUpgrades_DEFENCE:
                return m_duGrosCashSale >= defensePowerUp.price && m_defenseCD >= 0.0f;
        }
        return false;
    }
}