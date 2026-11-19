using System.Collections.Generic;
using UnityEngine;

public enum PortalSlot
{
    Blue,
    Orange
}

public enum PortalPlacementFailure
{
    None,
    MissingPair,
    MissingPortal,
    MissingFootprint,
    MissingSurface,
    SurfaceOccupied,
    SurfaceTooSmall,
    PortalAreaBlocked,
    ExitAreaBlocked
}

public readonly struct PortalPlacementCheck
{
    public bool IsValid { get; }
    public Pose Pose { get; }
    public PortalPlacementFailure Failure { get; }

    private PortalPlacementCheck(bool isValid, Pose pose, PortalPlacementFailure failure)
    {
        IsValid = isValid;
        Pose = pose;
        Failure = failure;
    }

    public static PortalPlacementCheck Valid(Pose pose)
    {
        return new PortalPlacementCheck(true, pose, PortalPlacementFailure.None);
    }

    public static PortalPlacementCheck Invalid(Pose pose, PortalPlacementFailure failure)
    {
        return new PortalPlacementCheck(false, pose, failure);
    }
}

[DisallowMultipleComponent]
public sealed class PortalPlacementService : MonoBehaviour
{
    [Header("Controlled pair")]
    [SerializeField] private PortalPair controlledPair;
    [SerializeField] private PortalPlacementSurface initialBlueSurface;
    [SerializeField] private PortalPlacementSurface initialOrangeSurface;

    [Header("Validation")]
    [SerializeField] private LayerMask blockingLayers = ~0;
    [SerializeField] private float exitRadius = 0.5f;
    [SerializeField] private float exitHeight = 2f;
    [SerializeField] private float exitMargin = 0.05f;

    private readonly Dictionary<PortalPlacementSurface, PortalEndpoint> occupiedSurfaces =
        new Dictionary<PortalPlacementSurface, PortalEndpoint>();
    private readonly Dictionary<PortalEndpoint, PortalPlacementSurface> portalSurfaces =
        new Dictionary<PortalEndpoint, PortalPlacementSurface>();

    private Pose initialBluePose;
    private Pose initialOrangePose;
    private bool initialized;

    public PortalPair ControlledPair => controlledPair;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized || controlledPair == null)
        {
            return;
        }

        PortalEndpoint blue = controlledPair.BluePortal;
        PortalEndpoint orange = controlledPair.OrangePortal;
        if (blue == null || orange == null)
        {
            return;
        }

        initialBluePose = new Pose(blue.transform.position, blue.transform.rotation);
        initialOrangePose = new Pose(orange.transform.position, orange.transform.rotation);
        RegisterInitialSurface(blue, initialBlueSurface);
        RegisterInitialSurface(orange, initialOrangeSurface);
        initialized = true;
    }

    public PortalPlacementCheck CheckPlacement(PortalSlot slot, PortalPlacementSurface surface)
    {
        Initialize();

        Pose pose = surface != null ? surface.GetPlacementPose() : default;
        if (controlledPair == null)
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.MissingPair);
        }

        PortalEndpoint portal = GetPortal(slot);
        if (portal == null)
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.MissingPortal);
        }

        if (surface == null)
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.MissingSurface);
        }

        if (occupiedSurfaces.TryGetValue(surface, out PortalEndpoint occupant) && occupant != portal)
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.SurfaceOccupied);
        }

        PortalPlacementFootprint footprint = portal.GetComponent<PortalPlacementFootprint>();
        if (footprint == null)
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.MissingFootprint);
        }

        Vector2 requiredSize = footprint.Size;
        if (surface.AvailableSize.x < requiredSize.x || surface.AvailableSize.y < requiredSize.y)
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.SurfaceTooSmall);
        }

        if (HasBlockingPortalOverlap(portal, footprint, surface, pose))
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.PortalAreaBlocked);
        }

        if (HasBlockingExitOverlap(portal, footprint, surface, pose))
        {
            return PortalPlacementCheck.Invalid(pose, PortalPlacementFailure.ExitAreaBlocked);
        }

        return PortalPlacementCheck.Valid(pose);
    }

    public PortalPlacementCheck TryPlace(PortalSlot slot, PortalPlacementSurface surface)
    {
        PortalPlacementCheck check = CheckPlacement(slot, surface);
        if (!check.IsValid)
        {
            return check;
        }

        PortalEndpoint portal = GetPortal(slot);
        if (portalSurfaces.TryGetValue(portal, out PortalPlacementSurface oldSurface) &&
            oldSurface != null && oldSurface != surface)
        {
            occupiedSurfaces.Remove(oldSurface);
        }

        portal.transform.SetPositionAndRotation(check.Pose.position, check.Pose.rotation);
        portalSurfaces[portal] = surface;
        occupiedSurfaces[surface] = portal;
        Physics.SyncTransforms();
        return check;
    }

    public void ResetPlacement()
    {
        Initialize();
        if (!initialized)
        {
            return;
        }

        PortalEndpoint blue = controlledPair.BluePortal;
        PortalEndpoint orange = controlledPair.OrangePortal;
        blue.transform.SetPositionAndRotation(initialBluePose.position, initialBluePose.rotation);
        orange.transform.SetPositionAndRotation(initialOrangePose.position, initialOrangePose.rotation);

        occupiedSurfaces.Clear();
        portalSurfaces.Clear();
        RegisterInitialSurface(blue, initialBlueSurface);
        RegisterInitialSurface(orange, initialOrangeSurface);
        Physics.SyncTransforms();
    }

    public PortalEndpoint GetPortal(PortalSlot slot)
    {
        if (controlledPair == null)
        {
            return null;
        }

        return slot == PortalSlot.Blue ? controlledPair.BluePortal : controlledPair.OrangePortal;
    }

    public PortalEndpoint GetOccupant(PortalPlacementSurface surface)
    {
        return surface != null && occupiedSurfaces.TryGetValue(surface, out PortalEndpoint occupant)
            ? occupant
            : null;
    }

    private void RegisterInitialSurface(PortalEndpoint portal, PortalPlacementSurface surface)
    {
        if (portal == null || surface == null)
        {
            return;
        }

        if (occupiedSurfaces.TryGetValue(surface, out PortalEndpoint occupant) && occupant != portal)
        {
            Debug.LogWarning($"Placement surface {surface.name} is assigned to more than one portal.", this);
            return;
        }

        occupiedSurfaces[surface] = portal;
        portalSurfaces[portal] = surface;
    }

    private bool HasBlockingPortalOverlap(
        PortalEndpoint portal,
        PortalPlacementFootprint footprint,
        PortalPlacementSurface surface,
        Pose pose)
    {
        Vector2 size = footprint.Size;
        float depth = footprint.Depth;
        Vector3 center = pose.position + pose.rotation * Vector3.forward * (depth * 0.5f);
        Vector3 halfExtents = new Vector3(size.x * 0.5f, size.y * 0.5f, depth * 0.5f);
        Collider[] overlaps = Physics.OverlapBox(
            center,
            halfExtents,
            pose.rotation,
            blockingLayers,
            QueryTriggerInteraction.Collide);

        return ContainsBlockingCollider(overlaps, portal, surface);
    }

    private bool HasBlockingExitOverlap(
        PortalEndpoint portal,
        PortalPlacementFootprint footprint,
        PortalPlacementSurface surface,
        Pose pose)
    {
        float radius = Mathf.Max(0.01f, exitRadius);
        float height = Mathf.Max(radius * 2f, exitHeight);
        Vector3 forward = pose.rotation * Vector3.forward;
        Vector3 center = pose.position + forward *
            (footprint.Depth + radius + exitMargin);
        float segment = Mathf.Max(0f, height * 0.5f - radius);
        Vector3 point0 = center + Vector3.up * segment;
        Vector3 point1 = center - Vector3.up * segment;
        Collider[] overlaps = Physics.OverlapCapsule(
            point0,
            point1,
            radius,
            blockingLayers,
            QueryTriggerInteraction.Ignore);

        return ContainsBlockingCollider(overlaps, portal, surface);
    }

    private static bool ContainsBlockingCollider(
        Collider[] overlaps,
        PortalEndpoint movingPortal,
        PortalPlacementSurface targetSurface)
    {
        foreach (Collider candidate in overlaps)
        {
            if (candidate == null || targetSurface.OwnsCollider(candidate))
            {
                continue;
            }

            PortalEndpoint candidatePortal = candidate.GetComponentInParent<PortalEndpoint>();
            if (candidatePortal == movingPortal)
            {
                continue;
            }

            if (candidatePortal != null)
            {
                return true;
            }

            if (candidate.isTrigger)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void OnValidate()
    {
        exitRadius = Mathf.Max(0.01f, exitRadius);
        exitHeight = Mathf.Max(exitRadius * 2f, exitHeight);
        exitMargin = Mathf.Max(0f, exitMargin);
    }
}
