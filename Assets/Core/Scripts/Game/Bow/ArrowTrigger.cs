using UnityEngine;

namespace Bow
{
    public class ArrowTrigger : MonoBehaviour
    {
        [SerializeField] private Bow.ArrowSpawner _arrowSpawner;
        //private ArrowSpawner _arrowSpawner;

        //private void Start()
        //{
        //    _arrowSpawner = GetComponent<ArrowSpawner>();
        //}

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "ArrowUnit")
            {
                Destroy(other.gameObject);
                _arrowSpawner._haveArrow = true;
            }
        }
    }
}
