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
		if (characterController != null)
		{
			characterController.enabled = false;
		}

		transform.position = startPosition;
		transform.rotation = startRotation;

		if (characterController != null)
		{
			characterController.enabled = true;
		}
	}
}