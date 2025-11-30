using UnityEngine;

public class SuspensionVehicle : MonoBehaviour
{
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _decelleration = 5f;
    [SerializeField] private float _maxSpeed = 100f;
    [SerializeField] private float _linearDrag = 2f;
    [SerializeField] private float _steeringSensitivity = 2f;
    [SerializeField] private AnimationCurve _steeringCurve;

    [SerializeField] private float _suspensionRestDistance;
    [SerializeField] private float _suspensionTravel;
    [SerializeField] private float _springStiffness;
    [SerializeField] private float _damperStiffness;
    [SerializeField] private float _sidewaysDragCoefficient = 1f;

    [SerializeField] private Transform[] _suspensionPoints;
    [SerializeField] private Transform _velocitiesDebugPoint;

    private Vector3 _currentLocalSpeed;
    private float _currentSpeedRatio = 0f;

    private Rigidbody _rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentLocalSpeed = transform.InverseTransformDirection(_rb.linearVelocity);

        _currentSpeedRatio = _currentLocalSpeed.z / _maxSpeed;

        Acceleration();
        Decceleration();
        Steering();
        Suspension();
        SidewaysDrag();
        VelocityDebug();
    }

    private void VelocityDebug()
    {
        Debug.DrawLine(_velocitiesDebugPoint.position, _velocitiesDebugPoint.position + new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z).normalized, Color.blue);
        Debug.DrawLine(_velocitiesDebugPoint.position, _velocitiesDebugPoint.position + transform.forward, Color.red);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = _currentLocalSpeed.x;
        float dragMagnitude = -currentSidewaysSpeed * _sidewaysDragCoefficient;

        Vector3 dragForce = transform.right * dragMagnitude;

        _rb.AddForceAtPosition(dragForce, _rb.worldCenterOfMass, ForceMode.Acceleration);
    }

    private void Suspension()
    {
        float maxLength = _suspensionRestDistance + _suspensionTravel;

        foreach (var point in _suspensionPoints)
        {
            RaycastHit hit;

            if (Physics.Raycast(point.position, -point.up, out hit, maxLength))
            {
                float currentDistance = hit.distance;
                float springCompression = (_suspensionRestDistance - currentDistance) / _suspensionTravel;
                float velocity = Vector3.Dot(_rb.GetPointVelocity(point.position), point.up);

                float stringForce = _springStiffness * springCompression;
                float damperForce = velocity * _damperStiffness;

                float netForce = stringForce - damperForce;

                _rb.AddForceAtPosition(point.up * netForce, point.position);
                Debug.DrawLine(point.position, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(point.position, point.position + (-point.up * maxLength), Color.green);
            }
        }
    }

    private void Steering() //TODO: add steering based on recieving axis info from SledController
    {
        
    }

    private void Decceleration()
    {
        _rb.AddForce(_currentSpeedRatio * _decelleration * -_rb.transform.forward, ForceMode.Acceleration);
    }

    private void Acceleration() //TODO: add acceleration based on input from SledController
    {
        
    }
}
