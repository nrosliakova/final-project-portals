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

    private void Update()
    {
        if (placementInput == null || !placementInput.TryGetAimHit(out RaycastHit hit))
        {
            SetVisible(false);
            return;
        }

        PortalPlacementSurface surface = hit.collider.GetComponentInParent<PortalPlacementSurface>();
        bool isValid = false;
        Pose pose;

        if (surface != null && placementInput.PlacementService != null)
        {
            PortalPlacementCheck check = placementInput.PlacementService.CheckPlacement(
                placementInput.ActiveSlot,
                surface);
            pose = check.Pose;
            isValid = check.IsValid;
        }
        else
        {
            pose = GetInvalidSurfacePose(hit);
        }

        if (previewTransform != null)
        {
            previewTransform.SetPositionAndRotation(pose.position, pose.rotation);
        }

        SetMaterial(isValid ? validMaterial : invalidMaterial);
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

    private Pose GetInvalidSurfacePose(RaycastHit hit)
    {
        Vector3 up = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) > 0.99f
            ? Vector3.forward
            : Vector3.up;
        Quaternion rotation = Quaternion.LookRotation(hit.normal, up);
        return new Pose(hit.point + hit.normal * invalidSurfaceOffset, rotation);
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
