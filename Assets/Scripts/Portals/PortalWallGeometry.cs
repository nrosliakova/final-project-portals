using System;
using UnityEngine;

internal sealed class PortalWallGeometry
{
    private const float PlaneTolerance = 0.005f;

    private readonly Vector3[] vertices = Array.Empty<Vector3>();
    private readonly int[] triangles = Array.Empty<int>();
    private readonly bool[] allowedTriangles = Array.Empty<bool>();

    public Mesh Mesh { get; }

    public PortalWallGeometry(MeshCollider collider, Material allowedMaterial)
    {
        Mesh = collider.sharedMesh;
        if (!Mesh.isReadable || !collider.TryGetComponent(out MeshFilter filter) ||
            filter.sharedMesh != Mesh || !collider.TryGetComponent(out MeshRenderer renderer))
        {
            return;
        }

        Material[] materials = renderer.sharedMaterials;
        if (materials.Length != Mesh.subMeshCount || Array.IndexOf(materials, allowedMaterial) < 0)
        {
            return;
        }

        vertices = Mesh.vertices;
        triangles = Mesh.triangles;
        allowedTriangles = new bool[triangles.Length / 3];
        int firstTriangle = 0;
        for (int submesh = 0; submesh < Mesh.subMeshCount; submesh++)
        {
            // RaycastHit.triangleIndex addresses the mesh's flattened triangle list.
            int count = (int)Mesh.GetIndexCount(submesh) / 3;
            if (materials[submesh] == allowedMaterial)
            {
                for (int triangle = firstTriangle; triangle < firstTriangle + count; triangle++)
                {
                    allowedTriangles[triangle] = true;
                }
            }
            firstTriangle += count;
        }
    }

    public bool AllowsTriangle(int index)
    {
        return index >= 0 && index < allowedTriangles.Length && allowedTriangles[index];
    }

    public bool Fits(RaycastHit hit, Quaternion portalRotation, Vector2 size)
    {
        Matrix4x4 meshToPortal = Matrix4x4.TRS(hit.point, portalRotation, Vector3.one).inverse *
            hit.collider.transform.localToWorldMatrix;
        Vector2 halfSize = size * 0.5f;
        float coveredArea = 0f;
        Span<Vector2> polygon = stackalloc Vector2[8];
        Span<Vector2> scratch = stackalloc Vector2[8];

        for (int triangle = 0; triangle < allowedTriangles.Length; triangle++)
        {
            if (!allowedTriangles[triangle])
            {
                continue;
            }

            int index = triangle * 3;
            Vector3 a = meshToPortal.MultiplyPoint3x4(vertices[triangles[index]]);
            Vector3 b = meshToPortal.MultiplyPoint3x4(vertices[triangles[index + 1]]);
            Vector3 c = meshToPortal.MultiplyPoint3x4(vertices[triangles[index + 2]]);
            if (Mathf.Abs(a.z) > PlaneTolerance || Mathf.Abs(b.z) > PlaneTolerance ||
                Mathf.Abs(c.z) > PlaneTolerance || Vector3.Cross(b - a, c - a).z <= 0f)
            {
                continue;
            }

            polygon[0] = a;
            polygon[1] = b;
            polygon[2] = c;
            int count = Clip(polygon, 3, scratch, Vector2.right, halfSize.x);
            count = Clip(scratch, count, polygon, Vector2.left, halfSize.x);
            count = Clip(polygon, count, scratch, Vector2.up, halfSize.y);
            count = Clip(scratch, count, polygon, Vector2.down, halfSize.y);
            for (int point = 1; point + 1 < count; point++)
            {
                Vector2 edge = polygon[point] - polygon[0];
                Vector2 next = polygon[point + 1] - polygon[0];
                coveredArea += (edge.x * next.y - edge.y * next.x) * 0.5f;
            }
        }

        // A portal that straddles a dark face, a gap or a corner loses coverage.
        return coveredArea >= size.x * size.y - 0.0001f;
    }

    private static int Clip(
        ReadOnlySpan<Vector2> polygon, int count, Span<Vector2> output, Vector2 axis, float edge)
    {
        if (count == 0)
        {
            return 0;
        }

        int written = 0;
        Vector2 previous = polygon[count - 1];
        float previousDistance = edge - Vector2.Dot(previous, axis);
        for (int index = 0; index < count; index++)
        {
            Vector2 point = polygon[index];
            float distance = edge - Vector2.Dot(point, axis);
            if ((distance >= 0f) != (previousDistance >= 0f))
            {
                float fraction = previousDistance / (previousDistance - distance);
                output[written++] = Vector2.LerpUnclamped(previous, point, fraction);
            }
            if (distance >= 0f)
            {
                output[written++] = point;
            }
            previous = point;
            previousDistance = distance;
        }
        return written;
    }
}
