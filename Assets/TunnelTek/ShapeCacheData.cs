using UnityEngine;

public class ShapeCacheData
{
    Vector3[] m_vertices;
    Vector3[] m_normals;
    Vector4[] m_tangents;
    Vector2[] m_uv;
    int[] m_indices;

    public ShapeCacheData(Mesh mesh)
    {
        m_vertices = mesh.vertices;
        m_normals  = mesh.normals;
        m_tangents = mesh.tangents;
        m_uv       = mesh.uv;
        m_indices  = mesh.GetIndices(0);
    }

    public int VertexCount 
    {
        get { return m_vertices.Length; } 
    }

    public int IndexCount 
    {
        get { return m_indices.Length; }
    }

    public void CopyVerticesTo(Vector3[] destination, int position)
    {
        System.Array.Copy(m_vertices, 0, destination, position, m_vertices.Length);
    }

    public void CopyNormalsTo(Vector3[] destination, int position)
    {
        System.Array.Copy(m_normals, 0, destination, position, m_normals.Length);
    }

    public void CopyTangentsTo(Vector4[] destination, int position)
    {
        System.Array.Copy(m_tangents, 0, destination, position, m_tangents.Length);
    }

    public void CopyUVTo(Vector2[] destination, int position)
    {
        System.Array.Copy(m_uv, 0, destination, position, m_uv.Length);
    }

    public void CopyIndicesTo(int[] destination, int position, int offset)
    {
        for (var i = 0; i < m_indices.Length; i++)
        {
            destination[position + i] = offset + m_indices[i];
        }
    }
}
