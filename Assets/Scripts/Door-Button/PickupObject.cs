using UnityEngine;

namespace Door_Button
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class PickupObject : MonoBehaviour
    {
        private Rigidbody rb;
        private Collider cubeCollider;
        private Collider playerCollider;
        private RigidbodyConstraints originalConstraints;
        private bool originalGravity;
        private bool isHeld;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            cubeCollider = GetComponent<Collider>();
        }

        public void SetHeld(bool held, Collider player = null)
        {
            isHeld = held;

            if (held)
            {
                playerCollider = player;
                originalGravity = rb.useGravity;
                originalConstraints = rb.constraints;
                rb.useGravity = false;
                rb.constraints |= RigidbodyConstraints.FreezeRotation;
                Physics.IgnoreCollision(cubeCollider, playerCollider, true);
            }
            else
            {
                rb.useGravity = originalGravity;
                rb.constraints = originalConstraints;
                rb.linearVelocity = Vector3.zero;
            }
        }

        public void MoveTo(Vector3 target, float speed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(
                (target - rb.position) * 10f, speed);
        }

        private void FixedUpdate()
        {
            if (playerCollider == null) return;

            if (isHeld)
            {
                if (!Physics.GetIgnoreCollision(cubeCollider, playerCollider))
                    Physics.IgnoreCollision(cubeCollider, playerCollider, true);
            }
            else if (!cubeCollider.bounds.Intersects(playerCollider.bounds))
            {
                Physics.IgnoreCollision(cubeCollider, playerCollider, false);
                playerCollider = null;
            }
        }
    }
}