using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    [Header("Настройки исчезновения")]
    [Tooltip("Тег транспорта, с которым взаимодействует препятствие")]
    public string PlayerTag = "Player";
    
    [Tooltip("Эффект при исчезновении (опционально)")]
    public GameObject destroyEffect;
    
    [Tooltip("Звук при разрушении (опционально)")]
    public AudioClip destroySound;
    
    [Tooltip("Задержка перед исчезновением")]
    public float destroyDelay = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("asddasd");
        // Проверяем, столкнулись ли с транспортом
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            DestroyObstacle();
        }
    }

    // Альтернативный метод для триггеров (если используете Trigger вместо Collision)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ghgg");
        if (other.CompareTag(PlayerTag))
        {
            DestroyObstacle();
        }
    }

    private void DestroyObstacle()
    {
        // Воспроизводим эффект, если он назначен
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, transform.rotation);
        }

        // Воспроизводим звук, если он назначен
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }

        // Удаляем объект
        Destroy(gameObject, destroyDelay);
    }
}