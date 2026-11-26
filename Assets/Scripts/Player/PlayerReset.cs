using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReset : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private CharacterController characterController;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetPlayer();
        }
    }

    private void ResetPlayer()
    {
        GetComponent<Player.PlayerPickup>()?.DropHeldObject();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
        GetComponent<PlayerMovement>().ResetVerticalSpeed();
        GetComponent<PlayerLook>().ResetLook();

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}