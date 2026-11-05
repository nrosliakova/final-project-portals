using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalSpeed;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 movement = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
            {
                movement += transform.forward;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                movement -= transform.forward;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                movement += transform.right;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                movement -= transform.right;
            }
        }

        movement = movement.normalized * moveSpeed;

        if (characterController.isGrounded && verticalSpeed < 0)
        {
            verticalSpeed = -2f;
        }

        verticalSpeed += gravity * Time.deltaTime;
        movement.y = verticalSpeed;

        characterController.Move(movement * Time.deltaTime);
    }
}