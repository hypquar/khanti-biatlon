using System.Collections.Generic;
using UnityEngine;

namespace Sleds
{
    public class SledInputController : MonoBehaviour
    {
        [SerializeField] private InputActiveZone _activeZone;
        [SerializeField] private InputDeadZone _deadZone;
        [SerializeField] private List<InputHandle> _handles;

        [SerializeField] private float _steeringInput;
        [SerializeField] private float _brakingInput;
        [SerializeField] private SledStatus _status;

        public float SteeringInput => _steeringInput;
        public float BrakingInput => _brakingInput;
        public SledStatus Status { get { return _status; } set { _status = value; } }

        public void DropHandles()
        {
            foreach (var handle in _handles)
            {
                handle.ChangeStatus(HandleStatus.BeyondActiveZone);
            }
        }
        private void FixedUpdate()
        {
            ManageHandleInput();
        }

        public void ChangeMovementStatus(SledStatus status)
        {
            _status = status;
        }

        private void ManageHandleInput()
        {
            float deadZoneRadius = _deadZone.Radius;
            float activeZoneRadius = _activeZone.Radius;

            float leftInput = 0f;
            float rightInput = 0f;

            foreach (var handle in _handles)
            {
                if (handle.Status == 
                    HandleStatus.DeadZone)
                {
                    if (handle.Side == HandleSide.Left)
                    {
                        leftInput = 0;
                    }
                    if (handle.Side == HandleSide.Right)
                    {
                        rightInput = 0;
                    }
                }


                else if (handle.Status == 
                    HandleStatus.ActiveZone)
                {
                    float distance = Vector3.Distance(handle.transform.position, _activeZone.transform.position);
                    float input = Mathf.Clamp01((distance - _deadZone.Radius) / (_activeZone.Radius - _deadZone.Radius));

                    if (handle.Side == HandleSide.Left)
                    {
                        leftInput = input;
                    }
                    if (handle.Side == HandleSide.Right)
                    {
                        rightInput = input;
                    }
                }


                else if (handle.Status == 
                    HandleStatus.BeyondActiveZone)
                {
                    if (handle.Side == HandleSide.Left)
                    {
                        leftInput = 1;
                    }
                    if (handle.Side == HandleSide.Right)
                    {
                        rightInput = 1;
                    }
                }
            }

            _steeringInput = Mathf.Clamp(-leftInput + rightInput, -1f, 1f);
            _brakingInput = Mathf.Min(leftInput, rightInput);
        }
    }
}
