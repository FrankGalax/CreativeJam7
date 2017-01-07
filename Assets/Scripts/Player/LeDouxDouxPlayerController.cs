using UnityEngine;

public class LeDouxDouxPlayerController : MonoBehaviour
{
    public GameObject DebugSpherePrefab;

    public float CameraShipDistance = 5.0f;
    public float CameraShipAngle = 30.0f;
    public float LookAhead = 10.0f;
    public float LookAheadAngleModifier = 0.5f;
    public float RollAngleModifier = 0.25f;
    public float FlightHeight = 2.0f;
    public float Acceleration = 1.0f;
    public float Decceleration = 1.0f;
    public float MaxSpeed = 1.0f;
    public float TurnRate = 1.0f;

    private Planet m_Planet;
    private Vector3 m_Normal;
    private Vector3 m_Velocity;
    private bool m_ChangedDirection;
    private Vector3 m_NextCameraPosition;
    private bool m_isShooting;

    void Awake()
    {
        m_Planet = Transform.FindObjectOfType<Planet>();
    }

    void Start()
    {
        m_Velocity = Vector3.zero;

        transform.position = RoundUp(transform.position, Vector3.zero);
        m_Normal = (transform.position - m_Planet.transform.position).normalized;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, m_Normal).normalized;
        transform.rotation = Quaternion.LookRotation(forward, m_Normal);
    }

    void Update()
    {
        m_Normal = (transform.position - m_Planet.transform.position).normalized;

        UpdateMovement();
        UpdateCamera();
        UpdateShoot();
    }

    private void UpdateMovement()
    {
        // Les maths la

        Camera camera = Camera.main;

        float vertical = Input.GetAxis("Propel");
        float horizontal = Input.GetAxis("Horizontal");

        // décélération
        if (vertical <= 0.0f)
        {
            Vector3 velocityDirection = m_Velocity.normalized;
            float length = m_Velocity.magnitude;
            length -= Decceleration * Time.deltaTime;
            if (length < 0.0f)
            {
                length = 0.0f;
            }
            m_Velocity = velocityDirection * length;
        }

        transform.Rotate(Vector3.up, horizontal * TurnRate * Time.deltaTime);

        m_Velocity += Acceleration * (transform.forward * vertical) * Time.deltaTime;

        float velocityLength = m_Velocity.magnitude;
        if (velocityLength > MaxSpeed)
        {
            m_Velocity = m_Velocity.normalized * MaxSpeed;
        }
        
        transform.position = RoundUp(transform.position, m_Velocity);

        Vector3 nextNormal = (transform.position - m_Planet.transform.position).normalized;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, nextNormal).normalized;
        transform.rotation = Quaternion.LookRotation(forward, nextNormal);

    }

    private void UpdateCamera()
    {
        Camera camera = Camera.main;

        float cameraShipAngleRad = Mathf.Deg2Rad * CameraShipAngle;

        Vector3 velocityOnPlane = Vector3.ProjectOnPlane(m_Velocity, transform.up);

        Vector3 velocityNormalizedOnPlane = velocityOnPlane.magnitude > 0.1 ? Vector3.ProjectOnPlane(m_Velocity, m_Normal).normalized : transform.forward;

        float velocityForwardAngle = m_Velocity.magnitude < 0.001f ? 0.0f : Vector3.Angle(transform.forward, velocityNormalizedOnPlane);
        Vector3 rightCancelRoll = Vector3.ProjectOnPlane(transform.right, m_Normal).normalized;
        if (Vector3.Angle(rightCancelRoll, velocityNormalizedOnPlane) < 90.0f)
            velocityForwardAngle *= -1.0f;

        transform.rotation = Quaternion.LookRotation(transform.forward, Quaternion.AngleAxis(-velocityForwardAngle * RollAngleModifier * m_Velocity.magnitude * 1.25f, transform.forward) * m_Normal);

        Vector3 predictiveVelocity = Quaternion.AngleAxis(velocityForwardAngle * LookAheadAngleModifier, transform.up) * transform.forward * m_Velocity.magnitude;
        Vector3 target = PredictiveRoundUp(transform.position, predictiveVelocity);

        Vector3 targetNormal = (target - m_Planet.transform.position).normalized;
        Vector3 targetForward = predictiveVelocity.magnitude < 0.001f ?
            Vector3.Cross(rightCancelRoll, targetNormal) :
            Vector3.ProjectOnPlane(predictiveVelocity, transform.up).normalized;

        m_NextCameraPosition = target + (targetNormal * Mathf.Sin(cameraShipAngleRad) - targetForward * Mathf.Cos(cameraShipAngleRad)) * CameraShipDistance; 

        camera.transform.position = Vector3.Lerp(camera.transform.position, m_NextCameraPosition, 6.0f * Time.deltaTime);
        Vector3 normal = (camera.transform.position - m_Planet.transform.position).normalized;
        camera.transform.rotation = Quaternion.LookRotation((target - camera.transform.position).normalized, /*Quaternion.AngleAxis(-velocityForwardAngle * RollAngleModifier * m_Velocity.magnitude * 0.5f, transform.forward) **/ normal);
    }

    private void UpdateShoot()
    {
        bool shoot = Input.GetButtonDown("Fire1");

        if (shoot && !m_isShooting)
        {
            GetComponent<VaisseauQuiJoueAfuckinMinecraft>().PewPew();
        }

        m_isShooting = shoot;
    }

    private Vector3 RoundUp(Vector3 previous, Vector3 velocity)
    {
        float speed = velocity.magnitude;
        Vector3 direction = velocity.normalized;
        float radius = m_Planet.Radius + FlightHeight;
        float angularSpeed = speed / radius;
        float deltaAngle = angularSpeed * Time.fixedDeltaTime;
        Vector3 displacement = radius * Mathf.Tan(deltaAngle) * direction;
        Vector3 next = previous + displacement;
        return m_Planet.transform.position + (next - m_Planet.transform.position).normalized * radius;
    }

    private Vector3 PredictiveRoundUp(Vector3 previous, Vector3 velocity)
    {
        float speed = velocity.magnitude;
        Vector3 direction = velocity.normalized;
        float radius = m_Planet.Radius + FlightHeight;
        float angularSpeed = speed / radius;
        float deltaAngle = angularSpeed * LookAhead * Time.fixedDeltaTime;
        if (deltaAngle > 0.3f)
            deltaAngle = 0.3f;
        Vector3 displacement = radius * Mathf.Tan(deltaAngle) * direction;
        Vector3 next = previous + displacement;
        return m_Planet.transform.position + (next - m_Planet.transform.position).normalized * radius;
    }
}
