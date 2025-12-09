using UnityEngine;

public class SnowTrailCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Игрок или объект для слежения

    [Header("Camera Settings")]
    [SerializeField] private float height = 50f; // Высота камеры над игроком
    [SerializeField] private Vector2 offset = Vector2.zero; // Смещение от центра игрока

    [Header("Orthographic Size")]
    [SerializeField] private float orthographicSize = 25f; // Размер области покрытия

    [Header("Boundary Settings (Optional)")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("SnowTrailCameraFollow: No Camera component found!");
            enabled = false;
            return;
        }

        // Автоматическая настройка камеры
        cam.orthographic = true;
        cam.orthographicSize = orthographicSize;

        // Если target не назначен, попробуйте найти игрока
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("SnowTrailCameraFollow: Автоматически найден игрок.");
            }
            else
            {
                Debug.LogWarning("SnowTrailCameraFollow: Target не назначен!");
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Вычисляем целевую позицию
        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + height,
            target.position.z + offset.y
        );

        // Применяем границы если включены
        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
        }

        // Жесткая привязка к позиции цели (без сглаживания)
        transform.position = targetPosition;

        // Убедимся что камера всегда смотрит вниз
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // Метод для динамического изменения размера области покрытия
    public void SetOrthographicSize(float newSize)
    {
        orthographicSize = newSize;
        if (cam != null)
        {
            cam.orthographicSize = orthographicSize;
        }
    }

    // Для отладки - показывает границы камеры
    private void OnDrawGizmosSelected()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic) return;

        Gizmos.color = Color.yellow;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        Vector3 center = transform.position;
        Vector3 size = new Vector3(camWidth, 0.1f, camHeight);

        Gizmos.DrawWireCube(center - new Vector3(0, height * 0.5f, 0), size);

        // Границы если включены
        if (useBoundaries)
        {
            Gizmos.color = Color.red;
            Vector3 boundsCenter = new Vector3(
                (minBounds.x + maxBounds.x) * 0.5f,
                transform.position.y - height * 0.5f,
                (minBounds.y + maxBounds.y) * 0.5f
            );
            Vector3 boundsSize = new Vector3(
                maxBounds.x - minBounds.x,
                0.1f,
                maxBounds.y - minBounds.y
            );
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}

