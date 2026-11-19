using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PortalEndpoint))]
public sealed class PortalTraversalTrigger : MonoBehaviour
{
    [SerializeField] private float crossingInset = 0.08f;

    private PortalEndpoint endpoint;
    private readonly Dictionary<IPortalTraveller, float> signedDistances =
        new Dictionary<IPortalTraveller, float>();

    private void Awake()
    {
        endpoint = GetComponent<PortalEndpoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IPortalTraveller traveller = FindTraveller(other);
        if (!CanTrack(traveller))
        {
            return;
        }

        signedDistances[traveller] = Mathf.Max(GetSignedDistance(traveller), 0.001f);
    }

    private void OnTriggerStay(Collider other)
    {
        IPortalTraveller traveller = FindTraveller(other);
        if (!CanTrack(traveller))
        {
            if (traveller != null)
            {
                signedDistances.Remove(traveller);
            }

            return;
        }

        float currentDistance = GetSignedDistance(traveller);
        if (!signedDistances.TryGetValue(traveller, out float previousDistance))
        {
            signedDistances[traveller] = currentDistance;
            return;
        }

        if (previousDistance > 0f && currentDistance <= 0f)
        {
            PortalEndpoint destination = endpoint.Pair.GetDestination(endpoint);
            if (destination != null)
            {
                PortalTransfer transfer = new PortalTransfer(
                    endpoint,
                    destination,
                    destination.GetExitCenter(traveller.ClearanceRadius),
                    GetRotationDelta(traveller.TravellerTransform.forward, destination));

                if (traveller.TryApplyPortalTransfer(transfer))
                {
                    signedDistances.Remove(traveller);
                    return;
                }
            }
        }

        signedDistances[traveller] = currentDistance;
    }

    private void OnTriggerExit(Collider other)
    {
        IPortalTraveller traveller = FindTraveller(other);
        if (traveller == null)
        {
            return;
        }

        signedDistances.Remove(traveller);
        traveller.NotifyExitedPortal(endpoint);
    }

    private void OnDisable()
    {
        signedDistances.Clear();
    }

    private void OnValidate()
    {
        crossingInset = Mathf.Max(0.001f, crossingInset);
    }

    private bool CanTrack(IPortalTraveller traveller)
    {
        return isActiveAndEnabled && endpoint != null && endpoint.Pair != null && traveller != null &&
            traveller.CanUsePortal(endpoint) && endpoint.Pair.GetDestination(endpoint) != null;
    }

    private float GetSignedDistance(IPortalTraveller traveller)
    {
        BoxCollider entrance = endpoint.Entrance;
        Vector3 triggerFront = endpoint.transform.TransformPoint(
            entrance.center + Vector3.forward * (entrance.size.z * 0.5f));
        float triggerFrontOffset = Vector3.Dot(
            triggerFront - endpoint.transform.position,
            endpoint.transform.forward);
        float transitionOffset = Mathf.Max(
            0f,
            triggerFrontOffset + traveller.ClearanceRadius - crossingInset);
        return Vector3.Dot(
            traveller.TravellerTransform.position - endpoint.transform.position,
            endpoint.transform.forward) - transitionOffset;
    }

    private Quaternion GetRotationDelta(
        Vector3 travellerForward,
        PortalEndpoint destination)
    {
        Quaternion portalRotation = destination.transform.rotation *
            Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(endpoint.transform.rotation);
        Vector3 mappedForward = portalRotation * travellerForward;
        Vector3 currentPlanar = Vector3.ProjectOnPlane(travellerForward, Vector3.up);
        Vector3 mappedPlanar = Vector3.ProjectOnPlane(mappedForward, Vector3.up);

        if (currentPlanar.sqrMagnitude < 0.0001f || mappedPlanar.sqrMagnitude < 0.0001f)
        {
            return Quaternion.identity;
        }

        float yaw = Vector3.SignedAngle(currentPlanar, mappedPlanar, Vector3.up);
        return Quaternion.AngleAxis(yaw, Vector3.up);
    }

    private static IPortalTraveller FindTraveller(Collider other)
    {
        MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IPortalTraveller traveller)
            {
                return traveller;
            }
        }

        return null;
    }
}
