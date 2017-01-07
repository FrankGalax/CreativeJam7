using UnityEngine;
using System.Collections;

public enum EDouxDouxUpgrades
{
    EDouxDouxUpgrades_OFFENCE,
    EDouxDouxUpgrades_DEFENCE
}

public class FatAssShip : MonoBehaviour
{
    uint m_duGrosCashSale = 0;

    [SerializeField]
    uint offencePowerUpPrice = 1;

    [SerializeField]
    uint defencePowerUpPrice = 1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    void SpendDaMoneyz(EDouxDouxUpgrades canIHazPls)
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
                    break;
                case EDouxDouxUpgrades.EDouxDouxUpgrades_DEFENCE:
                    foreach (VaisseauQuiJoueAfuckinMinecraft ship in ships)
                    {
                        ship.ImRubberYoureGlue();
                    }
                    break;
            }
        }
    }

    bool CanIHazPls(EDouxDouxUpgrades iReallyReallyNeedThis)
    {
        switch (iReallyReallyNeedThis)
        {
            case EDouxDouxUpgrades.EDouxDouxUpgrades_OFFENCE:
                return m_duGrosCashSale >= offencePowerUpPrice;
            case EDouxDouxUpgrades.EDouxDouxUpgrades_DEFENCE:
                return m_duGrosCashSale >= defencePowerUpPrice;
        }
        return false;
    }
}