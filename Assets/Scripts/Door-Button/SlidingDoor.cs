using UnityEngine;

namespace Door_Button
{
    public class SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Rigidbody left;
        [SerializeField] private Rigidbody right;
        [SerializeField] private DoorInput[] inputs;
        [SerializeField] private float openDistance = 1.5f;
        [SerializeField] private float speed = 2f;

        private bool isOpen;
        private Vector3 leftClosed;
        private Vector3 rightClosed;

        private void Awake()
        {
            leftClosed = left.transform.localPosition;
            rightClosed = right.transform.localPosition;
            RefreshInputs();
        }

        public void RefreshInputs()
        {
            isOpen = inputs != null && inputs.Length > 0;

            if (isOpen)
                foreach (DoorInput input in inputs)
                    if (input == null || !input.IsActive)
                    {
                        isOpen = false;
                        break;
                    }
        }

        private void FixedUpdate()
        {
            Move(left, leftClosed + Vector3.left * (isOpen ? openDistance : 0));
            Move(right, rightClosed + Vector3.right * (isOpen ? openDistance : 0));
        }

        private void Move(Rigidbody body, Vector3 localTarget)
        {
            Vector3 target = transform.TransformPoint(localTarget);
            Vector3 next = Vector3.MoveTowards(
                body.position, target, speed * Time.fixedDeltaTime);

            body.MovePosition(next);
        }
    }
}