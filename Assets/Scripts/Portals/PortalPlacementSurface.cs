using UnityEngine;

[DisallowMultipleComponent]
public sealed class PortalPlacementSurface : MonoBehaviour
{
    [SerializeField] private Transform placementPoint;
    [SerializeField] private Vector2 availableSize = new Vector2(1.2f, 2.1f);
    [SerializeField] private float wallOffset = 0.02f;
    [SerializeField] private Collider placementCollider;

    public Vector2 AvailableSize => availableSize;

    public Pose GetPlacementPose()
    {
        Transform anchor = placementPoint != null ? placementPoint : transform;
        return new Pose(anchor.position + anchor.forward * wallOffset, anchor.rotation);
    }

    public bool OwnsCollider(Collider candidate)
    {
        if (candidate == null)
        {
            return false;
        }

        if (placementCollider != null && candidate == placementCollider)
        {
            return true;
        }

        return candidate.transform == transform || candidate.transform.IsChildOf(transform);
    }

    private void Reset()
    {
        placementPoint = transform;
        placementCollider = GetComponent<Collider>();
    }

    private void OnValidate()
    {
        availableSize.x = Mathf.Max(0.01f, availableSize.x);
        availableSize.y = Mathf.Max(0.01f, availableSize.y);
        wallOffset = Mathf.Max(0f, wallOffset);
    }
}
