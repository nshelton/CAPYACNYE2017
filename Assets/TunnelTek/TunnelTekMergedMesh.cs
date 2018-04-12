//
//NGS: Mesh combiner based on Keijiro's BulkMesh to cut down on draw calls
//

using UnityEngine;

public class TunnelTekMergedMesh
{

    #region Public Constants

    // NGS: Seems like nobody can actually agree on the max number of verts in a Unity draw call.
    // using this number to be safe
    public const int DRAWCALL_MAX_VERTEX_COUNT = 65534;

    // NGS: Tell unity the mesh is HUGE so that it never gets culled. 
    // otherwise it will be culled based on unit cube which is wrong
    public const float DISABLE_CULLING_BOUNDS = 10000;

    #endregion

    #region Public Properties

    //NGS: Single combined mesh.
    private Mesh m_mesh;

    public Mesh Mesh 
    { 
        get { return m_mesh; } 
    }

    private int m_copyCount;

    #endregion

    #region Public Methods

    public TunnelTekMergedMesh(Mesh shape, int nSegments, int nSides)
    {
        RebuildMesh(shape, nSegments, nSides);
    }

    public void RebuildMesh(Mesh shape, int nSegments, int nSides)
    {
        if ( nSegments * nSides * shape.vertexCount > DRAWCALL_MAX_VERTEX_COUNT)
        {
            Debug.LogFormat("Too many verts {0} requested for TunnelTek. Max {1}", 
            nSegments * nSides * shape.vertexCount, DRAWCALL_MAX_VERTEX_COUNT);
        }

        DuplicateMesh(shape, nSegments, nSides);
    }

    #endregion

    #region Private Methods

    //NGS: Mesh combiner functoin
    void DuplicateMesh(Mesh shape, int nSegments, int nSides)
    {
        ShapeCacheData cache = new ShapeCacheData(shape);

        //NGS: Count the number of vertices and indices in the shape cache.
        var vc_shapes = cache.VertexCount;
        var ic_shapes = cache.IndexCount;
 
        //NGS: vertex Count, Index Count
        var vc = 0;
        var ic = 0;

        int numCopies = nSegments * nSides;

        for (m_copyCount = 0; m_copyCount < numCopies; m_copyCount++)
        {
            if (vc + cache.VertexCount > DRAWCALL_MAX_VERTEX_COUNT)
            {
                Debug.LogFormat("Too many verts for one draw call. only got {0} copies instead of {1} requested", m_copyCount, numCopies);
                break;
            } 
            vc += cache.VertexCount;
            ic += cache.IndexCount;
        }

        //NGS: Create vertex arrays.
        var vertices = new Vector3[vc];
        var normals  = new Vector3[vc];
        var tangents = new Vector4[vc];
        var uv       = new Vector2[vc];
        var uv2      = new Vector2[vc];
        var indicies = new int[ic];

        for (int v_i = 0, i_i = 0, n = 0; v_i < vc;)
        {
            cache.CopyVerticesTo(vertices, v_i);
            cache.CopyNormalsTo (normals,  v_i);
            cache.CopyTangentsTo(tangents, v_i);
            cache.CopyUVTo      (uv,       v_i);
            cache.CopyIndicesTo (indicies, i_i, v_i);

            var coord = new Vector2(
                (float)(n % nSides) / (float)nSides,    //NGS: side 0-1
                Mathf.Floor((float)n / (float)nSides) / (float)nSegments //NGS: segment 0-1
            );

            for (var i = 0; i < cache.VertexCount; i++)
            {
                uv2[v_i + i] = coord;
            } 

            v_i += cache.VertexCount;
            i_i += cache.IndexCount;
            n++;
        }

        m_mesh = new Mesh();

        m_mesh.vertices = vertices;
        m_mesh.normals  = normals;
        m_mesh.tangents = tangents;
        m_mesh.uv       = uv;
        m_mesh.uv2      = uv2;

        m_mesh.SetIndices(indicies, MeshTopology.Triangles, 0);

        //NGS: This only for temporary use. Don't save.
        m_mesh.hideFlags = HideFlags.DontSave;

        //NGS: Avoid being culled.
        m_mesh.bounds = new Bounds(Vector3.zero, Vector3.one * DISABLE_CULLING_BOUNDS);
    }

    #endregion
}