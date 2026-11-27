using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerGameplayControls))]
    public class PlayerMenuCall : MonoBehaviour
    {
        [SerializeField] private GameObject menu;
        private PlayerGameplayControls playerControls; 
        private bool IsMenuShowed { get; set; } = false;

        private void Awake()
        {
            playerControls = GetComponent<PlayerGameplayControls>();
            menu.SetActive(IsMenuShowed);
        }
        
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                MenuShowUpdate();
            }
        }

        private void MenuShowUpdate()
        {
            IsMenuShowed = !IsMenuShowed;
            menu.SetActive(IsMenuShowed);

            if (IsMenuShowed) playerControls.DisableGameplay();
            else playerControls.EnableGameplay();
        }
    }
}