using UnityEngine;
using System.Collections;

public class Volcano : MonoBehaviour {

    public float Lifetime = 10.0f;
    public float Length = 0.0f;
    public float EruptionSpeed = 1.0f;

    bool m_IsComingOut = false;
    float m_Displacement = 0.0f;

	// Use this for initialization
	void Start () {
        Timer.Instance.Request(Lifetime, () => { Destroy(this); });
        m_IsComingOut = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_IsComingOut)
        {
            Vector3 direction = transform.up;
            direction.Normalize();
            transform.position += direction * EruptionSpeed * Time.deltaTime;

            m_Displacement += EruptionSpeed * Time.deltaTime;
            if (m_Displacement >= Length)
            {
                m_IsComingOut = false;
            }
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        DamageComponent damageComponent = other.GetComponentInParent<DamageComponent>();
        if (damageComponent)
        {
            damageComponent.TakeDamage();
        }
    }
}
