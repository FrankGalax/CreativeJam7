using UnityEngine;
using System.Collections;

public class DamageComponent : MonoBehaviour {

    public uint InitialHealth = 3;

    bool m_IsDead = false;
    uint m_CurrentHealth;

    public uint GetCurrentHealth() { return m_CurrentHealth; }
    public bool GetIsDead() { return m_IsDead; }

    // Use this for initialization
    void Start () {
        m_CurrentHealth = InitialHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage()
    {
        if (m_IsDead)
            return;

        --m_CurrentHealth;
        SendMessage("OnDamageTaken");

        if (m_CurrentHealth == 0)
        {
            Die();
        }

    }

    public void Die()
    {
        m_IsDead = true;
        SendMessage("HandleDeath");
    }

    public void Revive()
    {
        m_IsDead = false;
        m_CurrentHealth = InitialHealth;
    }
}
