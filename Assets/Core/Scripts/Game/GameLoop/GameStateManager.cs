using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

/// <summary> Ядро системы управления состоянием игры </summary>
public class GameStateManager : MonoBehaviour
{
    // Singleton для глобального доступа
    public static GameStateManager Instance { get; private set; }
    
    // События для уведомления других систем
    public static event Action<int, float> OnGameEnded;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    
    // Текущее состояние
    private enum GameState { Playing, Paused, Ended }
    private GameState _currentState = GameState.Playing;
    
    // Данные уровня
    private int _finalScore;
    private float _levelTime;
    private bool _isTimerRunning = true;
    
    [SerializeField] private GameOverUI _gameOverUI;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        // Подсчёт времени игры
        if (_isTimerRunning)
        {
            _levelTime += Time.deltaTime;
        }
    }
    
    // === ПУБЛИЧНОЕ API ===
    
    /// <summary> Завершить игру с финальным счётом </summary>
    public void RequestGameEnd(int finalScore)
    {
        if(_currentState == GameState.Ended) return;

        _finalScore = finalScore;
        _isTimerRunning = false;

        HandleGameEnded();
    }
    
    public void RequestPause()
    {
        if(_currentState != GameState.Playing) return;
        _currentState = GameState.Paused;
        EnterPauseState();
        OnGamePaused?.Invoke();
    }
    
    public void RequestResume()
    {
        if(_currentState != GameState.Paused) return;

        _currentState = GameState.Playing;
        ExitPauseState();
        OnGameResumed?.Invoke();
    }
    
    public void RestartLevel()
    {
        ExitPauseState();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
);
    }
    public void ExitToMenu()
    {
        ExitPauseState();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    // === ВНУТРЕННЯЯ ЛОГИКА ===
    
    private void HandleGameEnded()
    {
        _currentState = GameState.Ended;

        OnGameEnded?.Invoke(_finalScore, _levelTime);

        EnterPauseState();

        if(_gameOverUI != null)
        {
            _gameOverUI.DisplayResults(_finalScore, _levelTime);
        }
        else Debug.Log("GameOverUI Не назначен");

    }
    
    private void EnterPauseState()
    {
        Time.timeScale = 0f;
    }
    
    private void ExitPauseState()
    {
        Time.timeScale = 1f;
    }
    
    // === ДЛЯ ТЕСТИРОВАНИЯ ===
    [ContextMenu("Тест: Завершить игру со 100 очками")]
    private void TestGameEnd() => RequestGameEnd(100);
    
    [ContextMenu("Тест: Поставить на паузу")]
    private void TestPause() => RequestPause();
    
    [ContextMenu("Тест: Снять с паузы")]
    private void TestResume() => RequestResume();
}