using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sleds
{
    public class SuspensionVehicle : MonoBehaviour
    {
        [SerializeField] private SledInputController _controller;
        [SerializeField] private VehicleEntrySystem _entrySystem;

        [SerializeField] private float _acceleration = 20f;
        [SerializeField] private float _decelleration = 5f;
        [SerializeField] private float _maxSpeed = 100f;
        [SerializeField] private float _linearDrag = 2f;
        [SerializeField] private float _steeringSencitivity = 2f;
        [SerializeField] private float _brakingSencitivity = 20f;
        [SerializeField] private AnimationCurve _steeringCurve;

        [SerializeField] private float _suspensionRestDistance;
        [SerializeField] private float _suspensionTravel;
        [SerializeField] private float _springStiffness;
        [SerializeField] private float _damperStiffness;
        [SerializeField] private float _sidewaysDragCoefficient = 1f;

        [SerializeField] private Transform[] _suspensionPoints;
        [SerializeField] private Transform _velocitiesDebugPoint;

        [SerializeField] private InputActionProperty _movementAction;

        [SerializeField] private float _speed;

        private Vector3 _currentLocalSpeed;
        private float _currentSpeedRatio = 0f;

        private Rigidbody _rb;

        private void OnEnable()
        {
            if (_movementAction.action != null)
            {
                _movementAction.action.Enable();
                _movementAction.action.performed += OnMovementPressed;
            }
        }

        private void OnDisable()
        {
            if (_movementAction.action != null)
            {
                _movementAction.action.performed -= OnMovementPressed;
                _movementAction.action.Disable();
            }
        }

        private void OnMovementPressed(InputAction.CallbackContext context)
        {
            if (_entrySystem.IsInVehicle)
            {
                if (_controller.Status == SledStatus.Halt)
                {
                    _controller.Status = SledStatus.Moving;
                    return;
                }

                if (_controller.Status == SledStatus.Moving)
                {
                    _controller.Status = SledStatus.Halt;
                    return;
                }
            }
        }

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

            if (_controller.Status == SledStatus.Moving)
            {
                Acceleration();
            }
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

        private void Steering() 
        {
            _rb.AddTorque(_controller.SteeringInput * _steeringCurve.Evaluate(Mathf.Abs(_currentSpeedRatio)) * _steeringSencitivity * Time.fixedDeltaTime * transform.up, ForceMode.VelocityChange);
        }

        private void Decceleration()
        {
            _rb.AddForce(_currentSpeedRatio * _decelleration * -_rb.transform.forward, ForceMode.Acceleration);
        }

        private void Acceleration() 
        {
            if (_currentLocalSpeed.z < _maxSpeed)
            {
                _rb.AddForce(transform.forward * (_acceleration - (_controller.BrakingInput * _brakingSencitivity)), ForceMode.Acceleration);
            }
        }
    }
}
