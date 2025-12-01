using UnityEngine;
using UnityEngine.Events;

namespace Sleds
{
    public class InputDeadZone : MonoBehaviour
    {
        [SerializeField] private float _radius;

        public float Radius
        {
            get { return _radius; }
        }

        private void Start()
        {
            SphereCollider _deadZone = gameObject.AddComponent<SphereCollider>();

            _deadZone.radius = _radius;
            _deadZone.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out InputHandle handle))
            {
                handle.ChangeStatus(Sleds.HandleStatus.DeadZone);

                Debug.Log($"Handle {other.name} entered dead zone");
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out InputHandle handle))
            {
                handle.ChangeStatus(Sleds.HandleStatus.ActiveZone);

                Debug.Log($"Handle {other.name} exited dead zone");
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
#endif
        
    }
}
