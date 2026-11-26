using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PortalEndpoint))]
public sealed class PortalPlacementFootprint : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(1.2f, 2.1f);
    [SerializeField] private float depth = 0.08f;

    public Vector2 Size => size;
    public float Depth => depth;

    private void OnValidate()
    {
        size.x = Mathf.Max(0.01f, size.x);
        size.y = Mathf.Max(0.01f, size.y);
        depth = Mathf.Max(0.01f, depth);
    }
}
