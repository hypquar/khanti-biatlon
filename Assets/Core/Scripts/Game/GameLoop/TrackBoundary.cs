using UnityEngine;

public class TrackBoundary : MonoBehaviour
{
    [Header("Настройки")]
    public string TagName = "Player";
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private float respawnDelay = 0.5f;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(TagName))
        {
            if (autoRespawn)
            {
                Invoke(nameof(TriggerRespawn), respawnDelay);
                lastVehicle = collision.gameObject;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagName))
        {
            if (autoRespawn)
            {
                Invoke(nameof(TriggerRespawn), respawnDelay);
                lastVehicle = other.gameObject;
            }
        }
    }
    
    private GameObject lastVehicle;
    
    private void TriggerRespawn()
    {
        if (lastVehicle != null)
        {
            CheckpointManager manager = FindObjectOfType<CheckpointManager>();
            if (manager != null)
            {
                manager.RespawnPlayer(lastVehicle);
            }
        }
    }
}