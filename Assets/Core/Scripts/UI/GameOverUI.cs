using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary> Управление окном завершения игры </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Текстовые поля")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _messageText;
    
    [Header("Кнопки")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;
    
    [Header("Сообщения")]
    [SerializeField] private string[] _goodMessages = 
    { 
        "Потрясающе! Ты мастер!", 
        "Невероятный результат!",
        "Идеальная точность!"
    };
    
    [SerializeField] private string[] _averageMessages = 
    { 
        "Хорошая работа!", 
        "Неплохо!",
        "Можно лучше, но уже хорошо!"
    };
    
    [SerializeField] private string[] _badMessages = 
    { 
        "Попрактикуйся ещё!", 
        "В следующий раз получится!",
        "Не сдавайся!"
    };
    
    [Header("Настройки анимации")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private bool _useFadeAnimation = true;
    
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if(_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if(_restartButton != null)
        {
            _restartButton.onClick.AddListener(() =>
            {
                GameStateManager.Instance.RestartLevel();
            });
        }

        if(_menuButton != null)
        {
            _menuButton.onClick.AddListener(() =>
            {
                GameStateManager.Instance.ExitToMenu();
            });
        }

        Hide();
    }
    
    /// <summary> Показать окно с результатами </summary>
    public void DisplayResults(int score, float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Обновляем текстовые поля
        if (_scoreText != null)
            _scoreText.text = $"ОЧКИ: {score}";
        
        if (_timeText != null)
            _timeText.text = $"ВРЕМЯ: {minutes:00}:{seconds:00}";
        
        if (_messageText != null)
            _messageText.text = GetRandomMessage(score);
        
        // Показываем окно
        if (_useFadeAnimation && gameObject.activeInHierarchy)
        {
            StartCoroutine(ShowAnimated());
        }
        else
        {
            Show();
        }
    }
    
    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    
    private IEnumerator ShowAnimated()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        
        float timer = 0;
        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // unscaled для работы на паузе
            _canvasGroup.alpha = Mathf.Lerp(0, 1, timer / _fadeDuration);
            yield return null;
        }
        
        _canvasGroup.alpha = 1;
    }
    
    private string GetRandomMessage(int score)
    {
        string[] messages;
        
        if (score >= 800)
            messages = _goodMessages.Length > 0 ? _goodMessages : new[] { "Отлично!" };
        else if (score >= 400)
            messages = _averageMessages.Length > 0 ? _averageMessages : new[] { "Хорошо!" };
        else
            messages = _badMessages.Length > 0 ? _badMessages : new[] { "Продолжайте!" };
        
        return messages[Random.Range(0, messages.Length)];
    }
    
    // Для отладки в инспекторе
    [ContextMenu("Тест: Показать окно")]
    private void TestShow()
    {
        DisplayResults(750, 85.5f);
    }
    
    [ContextMenu("Тест: Скрыть окно")]
    private void TestHide()
    {
        Hide();
    }
}