using Managers;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class PortalPlacementInput : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PortalPlacementService placementService;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask raycastMask = ~0;
    [SerializeField] private AudioClip placementSuccessClip;
    [SerializeField] private AudioClip placementFailureClip;

    private PlayerPickup playerPickup;

    public PortalPlacementService PlacementService => placementService;
    public PortalSlot ActiveSlot { get; private set; } = PortalSlot.Blue;
    public bool CanPlacePortals => isActiveAndEnabled &&
        (playerPickup == null || !playerPickup.IsHoldingObject);

    private void Awake()
    {
        playerPickup = GetComponent<PlayerPickup>();
    }

    private void LateUpdate()
    {
        if (!CanPlacePortals || Mouse.current == null)
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
            TryPlaceWithFeedback(PortalSlot.Blue);
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            TryPlaceWithFeedback(PortalSlot.Orange);
        }
    }

    private void TryPlaceWithFeedback(PortalSlot slot)
    {
        PortalPlacementCheck result = TryPlace(slot);
        AudioClip clip = result.IsValid ? placementSuccessClip : placementFailureClip;
        SoundFXManager soundManager = SoundFXManager.instance;

        if (clip != null && soundManager != null)
        {
            soundManager.PlaySoundFXClip(clip, transform, 1f);
        }
    }

    public PortalPlacementCheck TryPlace(PortalSlot slot)
    {
        if (!CanPlacePortals)
        {
            return PortalPlacementCheck.Invalid(default, PortalPlacementFailure.InputBlocked);
        }

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
