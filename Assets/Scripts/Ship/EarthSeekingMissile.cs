using UnityEngine;
using System.Collections;

public class EarthSeekingMissile : MonoBehaviour {

    [SerializeField]
    float jerk = 1.0f;
    [SerializeField]
    float jerkDuration = 2.0f;
    [SerializeField]
    float initialSpeed = 50.0f;
    [SerializeField]
    float initialAccel = 10.0f;

    [SerializeField]
    float superLockInDist = 20.0f;

    float m_acceleration;
    Vector3 m_velocity;
    float m_jerkTimer;
    PlanetFragment m_target;

    bool m_isHardLocked;

	// Use this for initialization
	void Start () {
        m_isHardLocked = false;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 planetPos = GameObject.FindObjectOfType<Planet>().transform.position;
        float missileRadius = (transform.position - planetPos).magnitude;

        Vector3 targetPos = (m_target.gameObject.transform.position - planetPos).normalized * missileRadius;
        Vector3 distance = targetPos - transform.position;

        Vector3 distUnwrapperSurUnPlane = Vector3.ProjectOnPlane(distance, transform.up);

        if (m_jerkTimer > 0.0f)
        {
            m_jerkTimer -= Time.deltaTime;

            float ratio = m_jerkTimer / jerkDuration;

            m_velocity += m_acceleration * distUnwrapperSurUnPlane.normalized + (m_jerkTimer > jerkDuration/2.0f ? 1.0f : -1.0f) * jerk * transform.forward * Time.deltaTime;
        }
        else
        {
            m_velocity += m_acceleration * distUnwrapperSurUnPlane.normalized * Time.deltaTime;
        }
        if (m_target.IsDestroyed())
        {
            Destroy();
            return;
        }

        float sqrDist = distUnwrapperSurUnPlane.sqrMagnitude;
 

        if (m_isHardLocked)
        {
            m_velocity = m_velocity.magnitude * (m_target.transform.position - transform.position).normalized;
        }
        else if (!m_isHardLocked && sqrDist < superLockInDist * superLockInDist)
        {
            m_isHardLocked = true;
        }

        if ( sqrDist < 0.1f)
        {
            m_target.TakeDamage();
            Destroy();
        }

        Vector3 displacement = m_velocity * Time.deltaTime;

        float deltaAngle = displacement.magnitude / missileRadius;
        Vector3 sphereDisplacement = missileRadius * Mathf.Tan(deltaAngle) * m_velocity.normalized;

        Vector3 newPos = planetPos + (transform.position + sphereDisplacement - planetPos).normalized * missileRadius;

        transform.position = newPos;
    }

    Vector3 RadialDisplacement(float radius, float angle, Vector3 velocity, Vector3 planetPos)
    {
        float speed = m_velocity.magnitude;
        Vector3 direction = m_velocity.normalized;
        float angularSpeed = speed / radius;
        float deltaAngle = angularSpeed * Time.fixedDeltaTime;
        Vector3 displacement = radius * Mathf.Tan(deltaAngle) * direction;
        Vector3 next = transform.position + displacement;
        return planetPos + (next - planetPos).normalized * radius;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public void Launch(PlanetFragment target, Vector3 direction, Vector3 upVect, Vector3 shipSpeed)
    {
        transform.rotation = Quaternion.LookRotation(direction, upVect);
        m_target = target;
        m_velocity = initialSpeed * direction + shipSpeed;
        m_acceleration = initialAccel;
        m_jerkTimer = jerkDuration;
    }

    public void HardLock()
    {
        m_isHardLocked = true;
    }
}
