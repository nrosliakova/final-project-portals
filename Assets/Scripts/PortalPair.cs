using UnityEngine;

public class PortalPair : MonoBehaviour
{
    [SerializeField] private PortalEndpoint bluePortal;
    [SerializeField] private PortalEndpoint orangePortal;

    public PortalEndpoint BluePortal => bluePortal;
    public PortalEndpoint OrangePortal => orangePortal;

    private void Awake()
    {
        SetPortals(bluePortal, orangePortal);
    }

    public void SetPortals(PortalEndpoint blue, PortalEndpoint orange)
    {
        if (bluePortal != null && bluePortal.Pair == this)
        {
            bluePortal.Pair = null;
        }

        if (orangePortal != null && orangePortal.Pair == this)
        {
            orangePortal.Pair = null;
        }

        bluePortal = blue;
        orangePortal = orange;

        if (bluePortal != null)
        {
            bluePortal.Pair = this;
        }

        if (orangePortal != null)
        {
            orangePortal.Pair = this;
        }
    }

    internal PortalEndpoint GetDestination(PortalEndpoint source)
    {
        if (!isActiveAndEnabled)
        {
            return null;
        }

        PortalEndpoint destination = source == bluePortal ? orangePortal : bluePortal;
        return destination != null && destination != source && destination.Pair == this &&
            destination.isActiveAndEnabled ? destination : null;
    }
}
