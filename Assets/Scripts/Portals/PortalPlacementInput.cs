using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class PortalPlacementInput : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PortalPlacementService placementService;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask raycastMask = ~0;

    public PortalPlacementService PlacementService => placementService;
    public PortalSlot ActiveSlot { get; private set; } = PortalSlot.Blue;

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ActiveSlot = PortalSlot.Blue;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ActiveSlot = PortalSlot.Orange;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TryPlace(PortalSlot.Blue);
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            TryPlace(PortalSlot.Orange);
        }
    }

    public PortalPlacementCheck TryPlace(PortalSlot slot)
    {
        if (placementService == null || !TryGetAimHit(out RaycastHit hit))
        {
            return PortalPlacementCheck.Invalid(default, PortalPlacementFailure.MissingSurface);
        }

        return placementService.TryPlace(slot, hit);
    }

    public bool TryGetAimHit(out RaycastHit hit)
    {
        hit = default;
        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        return Physics.Raycast(
            ray,
            out hit,
            maxDistance,
            raycastMask,
            QueryTriggerInteraction.Ignore);
    }

    private void Reset()
    {
        playerCamera = GetComponentInChildren<Camera>();
        placementService = GetComponent<PortalPlacementService>();
    }

    private void OnValidate()
    {
        maxDistance = Mathf.Max(0.01f, maxDistance);
    }
}
