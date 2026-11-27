using UnityEngine;

[DisallowMultipleComponent]
public sealed class PortalPlacementPreview : MonoBehaviour
{
    [SerializeField] private PortalPlacementInput placementInput;
    [SerializeField] private Transform previewTransform;
    [SerializeField] private Renderer[] previewRenderers;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private float invalidSurfaceOffset = 0.02f;

    private void LateUpdate()
    {
        if (placementInput == null || !placementInput.CanPlacePortals ||
            placementInput.PlacementService == null || !placementInput.TryGetAimHit(out RaycastHit hit))
        {
            SetVisible(false);
            return;
        }

        PortalPlacementCheck check = placementInput.PlacementService.CheckPlacement(
            placementInput.ActiveSlot, hit);

        if (previewTransform != null)
        {
            Vector3 position = check.Failure == PortalPlacementFailure.MissingSurface
                ? hit.point + hit.normal * invalidSurfaceOffset
                : check.Pose.position;
            previewTransform.SetPositionAndRotation(position, check.Pose.rotation);
        }

        SetMaterial(check.IsValid ? validMaterial : invalidMaterial);
        SetVisible(true);
    }

    private void OnDisable()
    {
        SetVisible(false);
    }

    private void OnValidate()
    {
        invalidSurfaceOffset = Mathf.Max(0f, invalidSurfaceOffset);
    }

    private void SetVisible(bool visible)
    {
        if (previewRenderers == null)
        {
            return;
        }

        foreach (Renderer previewRenderer in previewRenderers)
        {
            if (previewRenderer != null)
            {
                previewRenderer.enabled = visible;
            }
        }
    }

    private void SetMaterial(Material material)
    {
        if (material == null || previewRenderers == null)
        {
            return;
        }

        foreach (Renderer previewRenderer in previewRenderers)
        {
            if (previewRenderer != null)
            {
                previewRenderer.sharedMaterial = material;
            }
        }
    }
}
