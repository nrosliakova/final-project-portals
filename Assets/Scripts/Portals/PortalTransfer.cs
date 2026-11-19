using UnityEngine;

public readonly struct PortalTransfer
{
    public PortalEndpoint Source { get; }
    public PortalEndpoint Destination { get; }
    public Vector3 Position { get; }
    public Quaternion RotationDelta { get; }

    public PortalTransfer(
        PortalEndpoint source,
        PortalEndpoint destination,
        Vector3 position,
        Quaternion rotationDelta)
    {
        Source = source;
        Destination = destination;
        Position = position;
        RotationDelta = rotationDelta;
    }
}
