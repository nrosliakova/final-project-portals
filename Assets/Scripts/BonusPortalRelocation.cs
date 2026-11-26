using UnityEngine;
using UnityEngine.InputSystem;

// Optional Week 1 task for the Level_1_Bonus_PortalRelocation scene
[RequireComponent(typeof(Portal), typeof(BoxCollider))]
public class BonusPortalRelocation : MonoBehaviour
{
    [Header("Bonus: portal relocation (M)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private BoxCollider entrance;
    private bool atPointB;

    private void Awake()
    {
        entrance = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.mKey.wasPressedThisFrame)
        {
            return;
        }

        Transform destination = atPointB ? pointA : pointB;
        if (destination == null || PlayerOverlapsEntranceAt(destination))
        {
            return;
        }
        
        transform.SetPositionAndRotation(destination.position, destination.rotation);
        Physics.SyncTransforms();
        atPointB = !atPointB;
    }

    private bool PlayerOverlapsEntranceAt(Transform destination)
    {
        Vector3 scale = transform.lossyScale;
        Vector3 center = destination.position + destination.rotation * Vector3.Scale(entrance.center, scale);
        Vector3 halfSize = Vector3.Scale(entrance.size * 0.5f, scale);
        Collider[] overlaps = Physics.OverlapBox(center, halfSize, destination.rotation,
            Physics.AllLayers, QueryTriggerInteraction.Ignore);
        
        foreach (Collider other in overlaps)
        {
            if (other is CharacterController && other.TryGetComponent<PlayerMovement>(out _))
            {
                return true;
            }
        }

        return false;
    }
}
