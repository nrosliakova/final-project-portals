using UnityEngine;

public class PortalPair : MonoBehaviour
{
    [SerializeField] private Portal bluePortal;
    [SerializeField] private Portal orangePortal;

    private void Awake()
    {
        SetPortals(bluePortal, orangePortal);
    }

    public void SetPortals(Portal blue, Portal orange)
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

    internal Portal GetDestination(Portal source)
    {
        if (!isActiveAndEnabled)
        {
            return null;
        }

        Portal destination = source == bluePortal ? orangePortal : bluePortal;
        return destination != null && destination != source && destination.Pair == this &&
            destination.isActiveAndEnabled ? destination : null;
    }
}
