using System.Collections.Generic;
using UnityEngine;

namespace Door_Button
{
    public class PressureButton : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour target;
        [SerializeField] private Rigidbody body;

        [Header("Weight detection")]
        [SerializeField] private float minWeight = 1f;
        [SerializeField] private Vector3 boxOffset = new(0, 0.15f, 0);
        [SerializeField] private Vector3 boxSize = new(1, 0.3f, 1);
        [SerializeField] private LayerMask detectionLayers = ~0;
        [SerializeField] private float releaseDelay = 0.2f;
        private float timeWithoutWeight;

        [Header("Movement")]
        [SerializeField] private float pressDepth = 0.1f;
        [SerializeField] private float speed = 1f;

        private IActivatable activatable;
        private Vector3 initialPosition;
        private bool isPressed;
        private bool isActive;

        private readonly HashSet<Rigidbody> detectedBodies = new();

        private void Awake()
        {
            activatable = target as IActivatable;
            initialPosition = body.position;
        }

        private void FixedUpdate()
        {
            detectedBodies.Clear();
            float weight = 0f;

            Vector3 center = transform.TransformPoint(boxOffset);
            Vector3 halfExtents = Vector3.Scale(
                boxSize, transform.lossyScale) * 0.5f;

            Collider[] hits = Physics.OverlapBox(
                center, halfExtents, transform.rotation,
                detectionLayers, QueryTriggerInteraction.Ignore);
            
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent<CharacterController>(out var controller))
                {
                    weight += 10f;
                    continue;
                }

                Rigidbody rb = hit.attachedRigidbody;

                if (rb == null || rb == body || !detectedBodies.Add(rb))
                    continue;

                weight += rb.mass;
                Debug.Log("Weight: " + weight);
            }

            bool pressedNow = weight > minWeight;

            if (pressedNow && !isPressed)
            {
                isActive = !isActive;
                activatable?.SetActive(isActive);
            }

            isPressed = pressedNow;

            Vector3 destination = initialPosition +
                Vector3.down * (isPressed ? pressDepth : 0f);

            body.MovePosition(Vector3.MoveTowards(
                body.position, destination,
                speed * Time.fixedDeltaTime));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxOffset, boxSize);
        }
    }
}