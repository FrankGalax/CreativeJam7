using UnityEngine;
using System.Collections;

public class CashSale : MonoBehaviour {

    [SerializeField]
    uint value = 1;

    public float MaxDistanceToPlanet = 20.0f;
    protected GameObject planet;

    public uint GetValue() { return value; }

    // Use this for initialization
    void Start () {
	    if (INetwork.Instance.IsMaster())
        {
            planet = ResourceManager.GetPrefab("Planet");
        }

        DoStart();
	}

    protected virtual void DoStart() { }

	// Update is called once per frame
	void Update () {
	    if (INetwork.Instance.IsMaster())
        {
            float distance = (transform.position - planet.transform.position).magnitude;
            if (distance > MaxDistanceToPlanet)
            {
                INetwork.Instance.NetworkDestroy(gameObject);
            }
        }

        DoUpdate();
    }

    protected virtual void DoUpdate() { }
}
