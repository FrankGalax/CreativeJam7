using UnityEngine;

public class LeDouxDouxPlayerController : MonoBehaviour
{
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
    public Vector3 m_Velocity { get; private set; }
    private bool m_ChangedDirection;
    private Vector3 m_NextCameraPosition;
    private bool m_isShooting;
    private Vector3 m_LastTarget;
    private bool m_LastTargetSet;

    void Awake()
    {
        m_Planet = Transform.FindObjectOfType<Planet>();
    }

    void Start()
    {
        m_Velocity = Vector3.zero;

        transform.position = m_Planet.transform.position + (transform.position - m_Planet.transform.position).normalized * (m_Planet.Radius + FlightHeight);
        m_Normal = (transform.position - m_Planet.transform.position).normalized;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, m_Normal).normalized;
        transform.rotation = Quaternion.LookRotation(forward, m_Normal);
        m_LastTargetSet = false;
        m_isShooting = false;
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

        Vector3 nextPosition = transform.position + m_Velocity * Time.deltaTime;
        nextPosition = m_Planet.transform.position + (nextPosition - m_Planet.transform.position).normalized * (m_Planet.Radius + FlightHeight);
        transform.position = nextPosition;

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

        Vector3 target = transform.position + transform.forward;
        Vector3 targetNormal = (target - m_Planet.transform.position).normalized;
        target = m_Planet.transform.position + targetNormal * (m_Planet.Radius + FlightHeight);

        Vector3 targetForward = Vector3.Cross(transform.right, targetNormal);

        m_NextCameraPosition = target + (targetNormal * Mathf.Sin(cameraShipAngleRad) - targetForward * Mathf.Cos(cameraShipAngleRad)) * CameraShipDistance; 

        camera.transform.position = Vector3.Lerp(camera.transform.position, m_NextCameraPosition, 6.0f * Time.deltaTime);
        Vector3 normal = (camera.transform.position - m_Planet.transform.position).normalized;
        camera.transform.rotation = Quaternion.LookRotation((target - camera.transform.position).normalized, Quaternion.AngleAxis(-velocityForwardAngle * RollAngleModifier * m_Velocity.magnitude * 0.5f, transform.forward) * normal);

        m_LastTarget = target;
        m_LastTargetSet = true;
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
}
