using UnityEngine;
using System.Collections;

public class StunComponent : MonoBehaviour {

    [SerializeField]
    float stunDuration;

    float m_timer;
    bool m_isStunned;

	// Use this for initialization
	void Start () {
        m_isStunned = false;
        m_timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_isStunned)
        {
            LeDouxPlayerController ldpc = gameObject.GetComponent<LeDouxPlayerController>();
            LeDouxDouxPlayerController lddpc = gameObject.GetComponent<LeDouxDouxPlayerController>();

            if (INetwork.Instance.IsMine(gameObject))
            {
                if (ldpc != null && ldpc.enabled)
                {
                    ldpc.enabled = false;
                }

                if (lddpc != null && lddpc.enabled)
                {
                    lddpc.enabled = false;
                }
            }
        
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();

            renderer.enabled = !renderer.enabled;

            m_timer -= Time.deltaTime;
            if (m_timer <= 0.0f)
            {
                m_isStunned = false;

                if (INetwork.Instance.IsMine(gameObject))
                {
                    if (ldpc != null && !ldpc.enabled)
                    {
                        ldpc.enabled = true;
                    }

                    if (lddpc != null && !lddpc.enabled)
                    {
                        lddpc.enabled = true;
                    }
                }
                renderer.enabled = true;
            }
        }
	}
    [PunRPC]
    public void GetStunned()
    {
        m_timer = stunDuration;
        m_isStunned = true;
    }
}
