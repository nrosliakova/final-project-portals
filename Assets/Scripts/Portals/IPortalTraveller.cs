using UnityEngine;

public interface IPortalTraveller
{
    Transform TravellerTransform { get; }
    float ClearanceRadius { get; }
    bool CanUsePortal(PortalEndpoint portal);
    bool TryApplyPortalTransfer(PortalTransfer transfer);
    void NotifyExitedPortal(PortalEndpoint portal);
}
