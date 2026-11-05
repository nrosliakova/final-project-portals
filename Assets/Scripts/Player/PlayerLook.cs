using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float sensitivity = 0.1f;

    private float verticalRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Mouse.current == null || playerCamera == null)
        {
            return;
        }

        Vector2 mouseMovement = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up * mouseMovement.x * sensitivity);

        verticalRotation -= mouseMovement.y * sensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}