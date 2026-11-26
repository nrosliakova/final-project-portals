using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVP
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private GameObject goalHUD;
        [SerializeField] private GameObject endScreen;
        [SerializeField] private Button restartButton;

        public event Action RestartRequested;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        public void ShowGoal()
        {
            goalHUD.SetActive(true);
            endScreen.SetActive(false);
        }

        public void ShowEndScreen()
        {
            goalHUD.SetActive(false);
            endScreen.SetActive(true);
        }

        public void OnRestartClicked()
        {
            RestartRequested?.Invoke();
        }
    }
}