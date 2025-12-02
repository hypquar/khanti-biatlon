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
            Debug.Log($"=== OnTriggerEnter ===");
            Debug.Log($"Collider object: {other.gameObject.name}");
            Debug.Log($"Active: {other.gameObject.activeInHierarchy}");

            // Все компоненты на объекте
            var allComponents = other.GetComponents<Component>();
            Debug.Log($"Components on {other.name}:");
            foreach (var comp in allComponents)
            {
                Debug.Log($"  - {comp.GetType().Name}");
            }

            // Проверка InputHandle
            var handler = other.GetComponent<InputHandle>();
            Debug.Log($"GetComponent<InputHandle>: {handler}");

            // Проверка по типу
            var handleByType = other.GetComponent(typeof(InputHandle));
            Debug.Log($"GetComponent(typeof): {handleByType}");

            // Проверка через имя типа (если namespace проблема)
            var handleByString = other.GetComponent("InputHandle");
            Debug.Log($"GetComponent(string): {handleByString}");

            if (other.TryGetComponent(out InputHandle handle) && handle.Status == HandleStatus.BeyondActiveZone)
            {
                handle.ChangeStatus(HandleStatus.ActiveZone);

                Debug.Log($"Handle {other.name} entered active zone");
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out InputHandle handle))
            {
                handle.ChangeStatus(HandleStatus.BeyondActiveZone);

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
