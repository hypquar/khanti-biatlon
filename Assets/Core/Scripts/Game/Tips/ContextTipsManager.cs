using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ContextTips
{
    public class ContextTipsManager : MonoBehaviour
    {
        public static ContextTipsManager Instance { get; private set; }

        public UnityEvent OnLastTip;

        [SerializeField] private List<string> _tips;
        [SerializeField] private RectTransform _tipsUi;
        [SerializeField] private TextMeshProUGUI _tipsDisplay;

        private string _currentContextTip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }

            DontDestroyOnLoad(Instance);

            _currentContextTip = _tips[0];
        }

        private void Start()
        {
            UpdateTipDisplay();
        }

        public void ShowNextTip()
        {
            int currentTipIndex = _tips.IndexOf(_currentContextTip);
            if (currentTipIndex + 1 < _tips.Count)
            {
                _currentContextTip = _tips[_tips.IndexOf(_currentContextTip) + 1];
                UpdateTipDisplay();
            }
            else
            {
                HideTips();
            }
        }

        public void UpdateTipDisplay()
        {
            if (_currentContextTip != null)
            {
                _tipsDisplay.text = _currentContextTip;
            }
        }

        public void ShowTips()
        {
            _tipsUi.gameObject.SetActive(true);
        }

        public void HideTips()
        {
            _tipsUi.gameObject.SetActive(false);
        }
    }
}
