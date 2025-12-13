using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Только для тестирования! Управление через контекстное меню в инспекторе </summary>
public class TestGameEndTrigger : MonoBehaviour
{
    [Header("Настройки теста")]
    [SerializeField] private int _testScore = 750;
    [SerializeField] private float _testTime = 65.5f;
    
    [Header("Ссылки (опционально)")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private GameOverUI _gameOverUI;
    
    private void Start()
    {
        // Автоматически находим ссылки если не назначены
        if (_gameStateManager == null)
            _gameStateManager = FindObjectOfType<GameStateManager>();
        
        if (_gameOverUI == null)
            _gameOverUI = FindObjectOfType<GameOverUI>();
    }
    
    // ===== КОНТЕКСТНОЕ МЕНЮ В ИНСПЕКТОРЕ =====
    
    [ContextMenu("▶ Завершить игру")]
    private void TestGameEnd()
    {
        if (_gameStateManager != null)
        {
            Debug.Log($"[Тест] Завершаем игру со счётом: {_testScore}");
            _gameStateManager.RequestGameEnd(_testScore);
        }
        else
        {
            Debug.LogError("[Тест] GameStateManager не найден!");
        }
    }
    
    [ContextMenu("⏸ Поставить на паузу")]
    private void TestPause()
    {
        if (_gameStateManager != null)
        {
            Debug.Log("[Тест] Ставим игру на паузу");
            _gameStateManager.RequestPause();
        }
        else
        {
            Debug.LogError("[Тест] GameStateManager не найден!");
        }
    }
    
    [ContextMenu("▶▶ Снять с паузы")]
    private void TestResume()
    {
        if (_gameStateManager != null)
        {
            Debug.Log("[Тест] Снимаем паузу");
            _gameStateManager.RequestResume();
        }
        else
        {
            Debug.LogError("[Тест] GameStateManager не найден!");
        }
    }
    
    [ContextMenu("🔄 Рестарт уровня")]
    private void TestRestart()
    {
        if (_gameStateManager != null)
        {
            Debug.Log("[Тест] Перезапускаем уровень");
            _gameStateManager.RestartLevel();
        }
        else
        {
            Debug.LogError("[Тест] GameStateManager не найден!");
        }
    }
    
    [ContextMenu("🚪 Выход в меню")]
    private void TestExitToMenu()
    {
        if (_gameStateManager != null)
        {
            Debug.Log("[Тест] Выходим в главное меню");
            _gameStateManager.ExitToMenu();
        }
        else
        {
            Debug.LogError("[Тест] GameStateManager не найден!");
        }
    }
    
    [ContextMenu("🎯 Показать UI окно")]
    private void TestShowUI()
    {
        if (_gameOverUI != null)
        {
            Debug.Log($"[Тест] Показываем UI с очками: {_testScore}, время: {_testTime:F2}");
            _gameOverUI.DisplayResults(_testScore, _testTime);
        }
        else
        {
            Debug.LogError("[Тест] GameOverUI не найден!");
        }
    }
    
    [ContextMenu("👁 Скрыть UI окно")]
    private void TestHideUI()
    {
        if (_gameOverUI != null)
        {
            Debug.Log("[Тест] Скрываем UI");
            _gameOverUI.Hide();
        }
        else
        {
            Debug.LogError("[Тест] GameOverUI не найден!");
        }
    }
    
    [ContextMenu("📊 Тест всей системы")]
    private void TestFullSystem()
    {
        Debug.Log("=== НАЧАЛО ТЕСТА СИСТЕМЫ ===");
        
        TestPause();
        
        // Ждём немного (в реальности через корутину, но в тесте просто логируем)
        Debug.Log("... пауза установлена (2 сек) ...");
        
        TestResume();
        Debug.Log("... пауза снята ...");
        
        TestGameEnd();
        Debug.Log("... игра завершена, UI показан ...");
        
        Debug.Log("=== ТЕСТ ЗАВЕРШЁН ===");
    }
    
    // ===== ОБЩИЕ НАСТРОЙКИ =====
    
    [ContextMenu("⚙ Настроить тест: Высокий счёт")]
    private void SetupHighScoreTest()
    {
        _testScore = 950;
        _testTime = 45.2f;
        Debug.Log($"[Тест] Настроен высокий счёт: {_testScore} очков");
    }
    
    [ContextMenu("⚙ Настроить тест: Низкий счёт")]
    private void SetupLowScoreTest()
    {
        _testScore = 250;
        _testTime = 120.8f;
        Debug.Log($"[Тест] Настроен низкий счёт: {_testScore} очков");
    }
    
    // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====
    
    /// <summary> Быстрое создание тестовой сцены </summary>
    [ContextMenu("🛠 Создать тестовую сцену")]
    private void CreateTestScene()
    {
        Debug.Log("Создание тестовой сцены...");
        
        // Создаём GameStateManager если нет
        if (_gameStateManager == null)
        {
            var go = new GameObject("GameStateManager");
            _gameStateManager = go.AddComponent<GameStateManager>();
            Debug.Log("Создан GameStateManager");
        }
        
        // Создаём Canvas и UI если нет
        if (_gameOverUI == null)
        {
            var canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<Canvas>();
            
            var panelGO = new GameObject("GameOverPanel");
            panelGO.transform.SetParent(canvasGO.transform);
            _gameOverUI = panelGO.AddComponent<GameOverUI>();
            
            Debug.Log("Создан Canvas и GameOverUI");
        }
        
        Debug.Log("Тестовая сцена готова! Настройте UI элементы в инспекторе.");
    }
}