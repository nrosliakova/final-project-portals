using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVP
{
    public enum State
    {
        Playing = 0,
        Over = 1
    }
    public class GameMode : MonoBehaviour
    {
        [SerializeField] private PlayerGameplayControls playerControls;
        public State CurrentState { get; private set; } = State.Playing;

        public event Action<State> StateChanged;

        [SerializeField] private Transform completionZone;
        [SerializeField] private Vector3 halfExtents = Vector3.one;
        [SerializeField] private LayerMask playerLayer;

        private void FixedUpdate()
        {
            if (CurrentState == State.Playing)
                IsGameOver();
        }

        public bool IsGameOver()
        {
            if (CurrentState == State.Over)
                return true;

            if (Physics.CheckBox(
                    completionZone.position,
                    halfExtents,
                    completionZone.rotation,
                    playerLayer))
            {
                CurrentState = State.Over;
                playerControls.DisableGameplay();
                
                StateChanged?.Invoke(CurrentState);
            }

            return CurrentState == State.Over;
        }

        public void ReloadGame()
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
