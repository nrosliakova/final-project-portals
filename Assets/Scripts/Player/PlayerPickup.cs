using UnityEngine;
using UnityEngine.InputSystem;
using Door_Button;
using TMPro;

namespace Player
{
    public class PlayerPickup : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float pickupDistance = 3f;
        [SerializeField] private float holdDistance = 2f;
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TMP_Text hintText;

        private PickupObject heldObject;
        private CharacterController controller;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            UpdateHint();

            if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
                return;

            if (heldObject != null)
            {
                heldObject.SetHeld(false);
                heldObject = null;
            }
            else if (Physics.Raycast(playerCamera.transform.position,
                         playerCamera.transform.forward, out RaycastHit hit, pickupDistance)
                     && hit.collider.TryGetComponent(out PickupObject cube))
            {
                heldObject = cube;
                heldObject.SetHeld(true, controller);
            }
        }

        
        private void FixedUpdate()
        {
            if (heldObject == null) return;

            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 toCube = heldObject.transform.position - transform.position;

            float minHeight = controller.bounds.min.y
                              + heldObject.GetComponent<Collider>().bounds.extents.y + 0.1f;

            // Drop if the cube moves behind or beneath the player
            if (Vector3.Dot(toCube, forward) < 0.5f ||
                heldObject.transform.position.y < minHeight - 0.2f)
            {
                heldObject.SetHeld(false);
                heldObject = null;
                return;
            }

            Vector3 target = playerCamera.transform.position
                             + playerCamera.transform.forward * holdDistance;

            // Keep the target in front of and above the player's feet
            float distance = Vector3.Dot(target - transform.position, forward);
            target += forward * Mathf.Max(0f, 1.2f - distance);
            target.y = Mathf.Max(target.y, minHeight);

            heldObject.MoveTo(target, maxSpeed);
        }

        public void DropHeldObject()
        {
            if (heldObject == null) return;

            heldObject.SetHeld(false);
            heldObject = null;
        }

        private void UpdateHint()
        {
            if (hintPanel == null || hintText == null || playerCamera == null) return;

            if (heldObject != null)
            {
                hintPanel.SetActive(true);
                hintText.text = "TO DROP AN OBJECT";
                return;
            }

            bool canPickUp = Physics.Raycast(
                playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit,
                pickupDistance
            ) && hit.collider.TryGetComponent(out PickupObject cube);

            hintPanel.SetActive(canPickUp);

            if (canPickUp)
            {
                hintText.text = "TO PICK UP AN OBJECT";
            }
        }

        private void OnDisable()
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }

    }
}