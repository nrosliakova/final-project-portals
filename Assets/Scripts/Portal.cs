using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class Portal : MonoBehaviour
{
    internal PortalPair Pair { get; set; }

    private BoxCollider entrance;

    private void Reset()
    {
        BoxCollider trigger = GetComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.center = new Vector3(0f, 0f, 0.09f);
        trigger.size = new Vector3(1.2f, 2.1f, 0.18f);

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    private void Awake()
    {
        entrance = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || Pair == null || other is not CharacterController controller ||
            !controller.TryGetComponent(out PlayerMovement player))
        {
            return;
        }

        Portal destination = Pair.GetDestination(this);
        if (destination == null)
        {
            return;
        }

        float yaw = Vector3.SignedAngle(-transform.forward, destination.transform.forward, Vector3.up);
        player.Teleport(destination.GetExitCenter(controller), Quaternion.AngleAxis(yaw, Vector3.up));
    }

    private Vector3 GetExitCenter(CharacterController controller)
    {
        Vector3 scale = controller.transform.lossyScale;
        float radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float clearance = (controller.radius + controller.skinWidth) * radiusScale;
        Vector3 front = transform.TransformPoint(entrance.center + Vector3.forward * (entrance.size.z * 0.5f));
        return front + transform.forward * clearance;
    }
}
