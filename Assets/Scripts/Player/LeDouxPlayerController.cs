using UnityEngine;

public class LeDouxPlayerController : MonoBehaviour
{
    public float CameraUpDistance = 5.0f;
    public float FlightHeight = 1.0f;
    public float Acceleration = 1.0f;
    public float Decceleration = 1.0f;
    public float MaxSpeed = 1.0f;
    public float TurnRate = 1.0f;

    private Planet m_Planet;
    private Vector3 m_Normal;
    private Vector3 m_Tangent;
    private Vector3 m_Velocity;
    private bool m_ChangedDirection;
    private Vector3 m_NextCameraPosition;
    private Quaternion m_NextCameraRotation;
    private bool m_offensiveBonusIsDown;
    private bool m_defensiveBonusIsDown;


    void Awake()
    {
        m_Planet = Transform.FindObjectOfType<Planet>();
    }

    void Start()
    {
        m_Velocity = Vector3.zero;
    }

    void Update()
    {
        m_Normal = (transform.position - m_Planet.transform.position).normalized;
        m_Tangent = Vector3.ProjectOnPlane(Camera.main.transform.up, m_Normal).normalized;

        UpdateMovement();
        UpdateCamera();
        UpdateActions();
    }

    private void UpdateMovement()
    {
        Camera camera = Camera.main;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        // deceleration
        if (vertical == 0.0f && horizontal == 0.0f)
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

        m_Velocity += Acceleration * (camera.transform.up * vertical + camera.transform.right * horizontal) * Time.deltaTime;

        // max speed check
        float velocityLength = m_Velocity.magnitude;
        if (velocityLength > MaxSpeed)
        {
            m_Velocity = m_Velocity.normalized * MaxSpeed;
        }

        Vector3 nextPosition = transform.position + m_Velocity * Time.deltaTime;
        nextPosition = m_Planet.transform.position + (nextPosition - m_Planet.transform.position).normalized * (m_Planet.Radius + FlightHeight);
        transform.position = nextPosition;

        float dot = Vector3.Dot(Vector3.Cross(transform.forward, m_Velocity.normalized), m_Normal);
        float angle = Mathf.Abs(dot) < 0.004f ? 0.0f : TurnRate * Time.deltaTime * Mathf.Sign(dot);

        if (Mathf.Abs(angle) < 0.3f)
            angle = 0.0f;

        Vector3 forward = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
        Vector3 nextNormal = (transform.position - m_Planet.transform.position).normalized;
        forward = Vector3.ProjectOnPlane(forward, nextNormal).normalized;

        transform.rotation = Quaternion.LookRotation(forward, m_Normal);

    }

    private void UpdateCamera()
    {
        Camera camera = Camera.main;

        m_NextCameraPosition = m_Planet.transform.position + (transform.position - m_Planet.transform.position).normalized * (m_Planet.Radius + CameraUpDistance);
        camera.transform.position = Vector3.Lerp(camera.transform.position, m_NextCameraPosition, 6.0f * Time.deltaTime);
        Vector3 cameraUp = camera.transform.up;
        m_NextCameraRotation = Quaternion.LookRotation((transform.position - camera.transform.position).normalized, cameraUp);
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, m_NextCameraRotation, 6.0f * Time.deltaTime);
    }

    private void UpdateActions()
    {
        bool offensiveBonus = Input.GetButtonDown("Fire1");
        bool defensiveBonus = Input.GetButtonDown("Fire2");

        if (offensiveBonus && !m_offensiveBonusIsDown)
        {
            FatAssShip motherShip = GetComponent<FatAssShip>();
            motherShip.SpendDaMoneyz(EDouxDouxUpgrades.EDouxDouxUpgrades_OFFENCE);
        }
        else if (defensiveBonus && !m_defensiveBonusIsDown)
        {
            FatAssShip motherShip = GetComponent<FatAssShip>();
            motherShip.SpendDaMoneyz(EDouxDouxUpgrades.EDouxDouxUpgrades_DEFENCE);
        }

        m_offensiveBonusIsDown = offensiveBonus;
        m_defensiveBonusIsDown = defensiveBonus;
    }
}
