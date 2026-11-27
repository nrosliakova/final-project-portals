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
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip dropSound;
        [SerializeField] private AudioClip collisionSound;
        [SerializeField] private float minImpactSpeed = 1f;
        private AudioSource audioSource;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            cubeCollider = GetComponent<Collider>();
            audioSource = GetComponent<AudioSource>();
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

                if (audioSource != null && pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }
            }
            else
            {
                rb.useGravity = originalGravity;
                rb.constraints = originalConstraints;
                rb.linearVelocity = Vector3.zero;

                if (audioSource != null && dropSound != null)
                {
                    audioSource.PlayOneShot(dropSound);
                }
            }
        }

        public void MoveTo(Vector3 target, float speed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(
                (target - rb.position) * 10f, speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (audioSource != null && collisionSound != null &&
                collision.relativeVelocity.magnitude >= minImpactSpeed)
            {
                audioSource.PlayOneShot(collisionSound);
            }
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