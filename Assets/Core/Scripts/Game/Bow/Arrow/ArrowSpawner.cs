using UnityEngine;

namespace Arrow
{
    public class ArrowSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _arrowUnitPrefab;
        [SerializeField] private GameObject _spawnPoint;

        public void SpawnArrow()
        {
            if (_spawnPoint != null)
            {
                Instantiate(_arrowUnitPrefab, transform.position, transform.rotation, _spawnPoint.transform);
                Destroy(this);
            }
            else
            {
                Debug.Log("_spawnPoint IS NULL");
            }
        }
    }
}