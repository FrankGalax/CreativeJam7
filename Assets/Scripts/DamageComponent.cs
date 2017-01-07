using UnityEngine;
using System.Collections;

public class DamageComponent : MonoBehaviour {

    public uint InitialHealth = 3;
    public float DamageCooldown = 0.0f;

    bool m_IsDead = false;
    uint m_CurrentHealth;
    bool m_OnCooldown = false;

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
        if (m_IsDead || m_OnCooldown)
            return;

        --m_CurrentHealth;
        SendMessage("OnDamageTaken");

        m_OnCooldown = true;
        Timer.Instance.Request(DamageCooldown, () => { m_OnCooldown = false; });

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
