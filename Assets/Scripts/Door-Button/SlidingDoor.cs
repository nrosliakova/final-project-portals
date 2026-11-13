using UnityEngine;

namespace Door_Button
{

    public class SlidingDoor : MonoBehaviour, IActivatable
    {
        [SerializeField] private Rigidbody left;
        [SerializeField] private Rigidbody right;
        [SerializeField] private bool isOpen;
        [SerializeField] private float openDistance = 1.5f;
        [SerializeField] private float speed = 2f;
        
        [Header("Door indicator")]
        [SerializeField] private Renderer indicator;
        [SerializeField] private Color openColor = Color.green;
        [SerializeField] private Color closedColor = Color.red;

        private MaterialPropertyBlock propertyBlock;

        private Vector3 leftClosed;
        private Vector3 rightClosed;

        private void Awake()
        {
            leftClosed = left.transform.localPosition;
            rightClosed = right.transform.localPosition;
            UpdateIndicator();
        }

        public void SetActive(bool active)
        {
            isOpen = active;
            UpdateIndicator();
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
        
        private void UpdateIndicator()
        {
            if (indicator == null) return;

            propertyBlock ??= new MaterialPropertyBlock();
            indicator.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", isOpen ? openColor : closedColor);
            indicator.SetPropertyBlock(propertyBlock);
        }
    }

}