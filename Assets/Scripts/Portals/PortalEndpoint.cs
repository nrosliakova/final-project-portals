using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public sealed class PortalEndpoint : MonoBehaviour
{
    internal PortalPair Pair { get; set; }
    internal BoxCollider Entrance { get; private set; }

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
        Entrance = GetComponent<BoxCollider>();
    }

    internal Vector3 GetExitCenter(float clearance)
    {
        Vector3 front = transform.TransformPoint(
            Entrance.center + Vector3.forward * (Entrance.size.z * 0.5f));
        return front + transform.forward * clearance;
    }

    internal bool OverlapsTraveller(Vector3 position, float clearance)
    {
        return Entrance != null && Entrance.bounds.SqrDistance(position) <= clearance * clearance;
    }
}
