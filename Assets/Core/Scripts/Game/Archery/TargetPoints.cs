using System;
using UnityEngine;

public class TargetPoints : MonoBehaviour
{
    public static event Action<GameObject, int> OnTargetHit; // Теперь передаём int!
    [SerializeField] private Transform _centrePoint;
    [SerializeField] private float _maxRadius = 0.1f;
    [SerializeField] private Renderer _targetRenderer;

    void Start()
    {
        AutoCalculateRadius();
    }

    [ContextMenu("Auto Calculate Radius")]
    private void AutoCalculateRadius()
    {
        if (_targetRenderer == null)
            _targetRenderer = GetComponentInChildren<Renderer>();
        
        if (_targetRenderer != null)
        {
            Bounds bounds = _targetRenderer.bounds;
            _maxRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);
            Debug.Log($"Авторасчёт: MaxRadius установлен в {_maxRadius}");
        }
        else
        {
            Debug.LogWarning("Renderer не найден. Радиус не был рассчитан.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            OnHit(other.transform.position);
        }
    }

    private void OnHit(Vector3 hitPosition)
    {
        // 1. Рассчитываем дистанцию
        float distance = Vector3.Distance(_centrePoint.position, hitPosition);
        
        // 2. Определяем целые очки по зонам
        int points = 0;
        
        if (distance < _maxRadius * 0.1f) points = 100;      // Центр (10% радиуса)
        else if (distance < _maxRadius * 0.3f) points = 80; // Внутреннее кольцо
        else if (distance < _maxRadius * 0.5f) points = 60; // Среднее кольцо
        else if (distance < _maxRadius * 0.7f) points = 40; // Внешнее кольцо
        else if (distance < _maxRadius) points = 20;        // Край мишени
        // Если distance >= _maxRadius, points остаётся 0 (промах, хотя это маловероятно)
        
        Debug.Log($"Попал! Дистанция: {distance:F2}. Получили {points} очков");
        
        // 3. Уведомляем мир через событие (передаём GameObject этой мишени и целые очки)
        OnTargetHit?.Invoke(gameObject, points);
    }
}