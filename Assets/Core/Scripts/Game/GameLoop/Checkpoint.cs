using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Настройки")]
    public string PlayerTag = "Player";
    [SerializeField] private int checkpointIndex = 0; // Порядковый номер чекпоинта
    [SerializeField] private Color gizmoColor = Color.green;
    
    public int Index => checkpointIndex;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            CheckpointManager manager = FindObjectOfType<CheckpointManager>();
            if (manager != null)
            {
                manager.CheckpointReached(this, other.gameObject);
            }
        }
    }
    
    // Визуализация в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
        
        // Отображение номера чекпоинта
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position, $"Checkpoint {checkpointIndex}");
        #endif
    }
}