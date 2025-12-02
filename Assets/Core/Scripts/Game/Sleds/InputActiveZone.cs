using UnityEngine;
using UnityEngine.Events;

namespace Sleds
{
    public class InputActiveZone : MonoBehaviour
    {
        public UnityEvent<InputHandle> OnActiveZoneEnter;
        public UnityEvent<InputHandle> OnActiveZoneExit;

        [SerializeField] private float _radius;

        public float Radius
        {
            get { return _radius; }
        }

        private void Start()
        {
            SphereCollider _activeZone = gameObject.AddComponent<SphereCollider>();

            _activeZone.radius = _radius;
            _activeZone.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out InputHandle handle) && handle.Status != Sleds.HandleStatus.DeadZone)
            {
                handle.ChangeStatus(HandleStatus.ActiveZone);

                Debug.Log($"Handle {other.name} entered active zone");
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out InputHandle handle))
            {
                handle.ChangeStatus(Sleds.HandleStatus.BeyondActiveZone);

                Debug.Log($"Handle {other.name} exited active zone");
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
#endif
    }
}
