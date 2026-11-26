using UnityEngine;

namespace MVP
{
    public class GamePresenter : MonoBehaviour
    {
        [SerializeField] private GameMode model;
        [SerializeField] private GameView view;

        private void OnEnable()
        {
            model.StateChanged += UpdateView;
            view.RestartRequested += model.ReloadGame;

            UpdateView(model.CurrentState);
        }

        private void OnDisable()
        {
            model.StateChanged -= UpdateView;
            view.RestartRequested -= model.ReloadGame;
        }

        private void UpdateView(State state)
        {
            switch (state)
            {
                case State.Playing:
                    view.ShowGoal();
                    break;

                case State.Over:
                    view.ShowEndScreen();
                    break;
            }
        }
    }
}