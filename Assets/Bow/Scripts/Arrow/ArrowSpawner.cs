using UnityEngine;

namespace Arrow
{
    public class ArrowSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _arrowUnitPrefab;
        [SerializeField] private GameObject _spawnPoint;

        private void Start()
        {
            _spawnPoint = GameObject.Find("Colchan");
        }

        public void SpawnArrow()
        {
            Instantiate(_arrowUnitPrefab, transform.position, transform.rotation, _spawnPoint.transform);
            Destroy(this);
        }
    }
}