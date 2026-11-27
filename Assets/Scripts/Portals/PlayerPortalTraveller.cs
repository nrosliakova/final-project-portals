using System;
using Managers;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
public sealed class PlayerPortalTraveller : MonoBehaviour, IPortalTraveller
{
    [SerializeField] private AudioClip teleportClip;

    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private PortalEndpoint blockedPortal;

    public event Action BeforeTeleport;

    public Transform TravellerTransform => transform;

    public float ClearanceRadius
    {
        get
        {
            Vector3 scale = transform.lossyScale;
            float radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
            return (characterController.radius + characterController.skinWidth) * radiusScale;
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        if (blockedPortal != null &&
            !blockedPortal.OverlapsTraveller(transform.position, ClearanceRadius))
        {
            blockedPortal = null;
        }
    }

    private void OnDisable()
    {
        ResetTransitState();
    }

    public bool CanUsePortal(PortalEndpoint portal)
    {
        return isActiveAndEnabled && portal != null && portal != blockedPortal;
    }

    public bool TryApplyPortalTransfer(PortalTransfer transfer)
    {
        if (!CanUsePortal(transfer.Source) || transfer.Destination == null)
        {
            return false;
        }

        BeforeTeleport?.Invoke();
        blockedPortal = transfer.Destination;
        playerMovement.Teleport(transfer.Position, transfer.RotationDelta);
        Physics.SyncTransforms();

        SoundFXManager soundManager = SoundFXManager.instance;
        if (teleportClip != null && soundManager != null)
        {
            soundManager.PlaySoundFXClip(teleportClip, transform, 1f);
        }

        return true;
    }

    public void NotifyExitedPortal(PortalEndpoint portal)
    {
        if (portal == blockedPortal)
        {
            blockedPortal = null;
        }
    }

    public void ResetTransitState()
    {
        blockedPortal = null;
    }
}
