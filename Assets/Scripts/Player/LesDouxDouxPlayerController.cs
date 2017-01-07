using UnityEngine;

public class LeDouxDouxPlayerController : MonoBehaviour
{
    public float CameraShipDistance = 5.0f;
    public float PlanetRadius = 5.0f;
    public float FlightHeight = 1.0f;
    public float Acceleration = 1.0f;
    public float Decceleration = 1.0f;
    public float MaxSpeed = 1.0f;
    public float TurnRate = 1.0f;

    private Transform m_Planet;
    private Vector3 m_Normal;
    private Vector3 m_Tangent;
    private Vector3 m_BiTangent;
    private Vector3 m_Velocity;
    private bool m_ChangedDirection;
    private Vector3 m_NextCameraPosition;

    void Awake()
    {
        m_Planet = GameObject.Find("Planet").transform;
    }

    void Start()
    {
        m_Velocity = Vector3.zero;
    }

    void Update()
    {
        m_Normal = (transform.position - m_Planet.position).normalized;
        m_Tangent = Vector3.ProjectOnPlane(Camera.main.transform.up, m_Normal).normalized;
        m_BiTangent = Vector3.Cross(m_Normal, m_Tangent);

        UpdateMovement();
        UpdateCamera();
    }

    private void UpdateMovement()
    {
        // Les maths la

        Camera camera = Camera.main;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        // décélération
        if (vertical == 0.0f && horizontal == 0.0f)
        {
            Vector3 velocityDirection = m_Velocity.normalized;
            m_Velocity -= Decceleration * velocityDirection * Time.deltaTime;
        }

        m_Velocity += Acceleration * (camera.transform.up * vertical + camera.transform.right * horizontal) * Time.deltaTime;

        float velocityLength = m_Velocity.magnitude;
        if (velocityLength > MaxSpeed)
        {
            m_Velocity = m_Velocity.normalized * MaxSpeed;
        }

        Vector3 nextPosition = transform.position + m_Velocity * Time.deltaTime;
        nextPosition = m_Planet.position + (nextPosition - m_Planet.position).normalized * (PlanetRadius + FlightHeight);
        transform.position = nextPosition;

        Vector3 forward = Vector3.Lerp(transform.forward, m_Velocity, TurnRate * Time.deltaTime);
        forward = Vector3.ProjectOnPlane(forward, (nextPosition - m_Planet.position).normalized).normalized;

        transform.rotation = Quaternion.LookRotation(forward, m_Normal);

    }

    private void UpdateCamera()
    {
        Camera camera = Camera.main;

        m_NextCameraPosition = m_Planet.position + m_Normal * (PlanetRadius + CameraUpDistance);
        camera.transform.position = Vector3.Lerp(camera.transform.position, m_NextCameraPosition, 6.0f * Time.deltaTime);
        camera.transform.rotation = Quaternion.LookRotation((transform.position - camera.transform.position).normalized, m_Tangent);
    }
}
