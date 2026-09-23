using UnityEngine;

public enum BlockType
{
    Dirt,
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ProceduralBlock : MonoBehaviour
{
    [SerializeField] private BlockType blockType;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private const string UrpLitShader = "Universal Render Pipeline/Lit";

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();

        _meshRenderer.sharedMaterial = new Material(Shader.Find(UrpLitShader));

        var vertices = new Vector3[]
        {
            // Front face vertices
            new Vector3(-0.5f, -0.5f, -0.5f), // I0 (V0)
            new Vector3(0.5f, -0.5f, -0.5f), // I1 (V1)
            new Vector3(0.5f, 0.5f, -0.5f), // I2 (V2)
            new Vector3(-0.5f, 0.5f, -0.5f), // I3 (V3)

            // Back face vertices
            new Vector3(-0.5f, -0.5f, 0.5f), // I4 (V4)
            new Vector3(0.5f, -0.5f, 0.5f), // I5 (V5)
            new Vector3(0.5f, 0.5f, 0.5f), // I6 (V6)
            new Vector3(-0.5f, 0.5f, 0.5f), // I7 (V7)

            // Right face vertices
            new Vector3(0.5f, -0.5f, -0.5f), // I8 (V1)
            new Vector3(0.5f, -0.5f, 0.5f), // I9 (V5)
            new Vector3(0.5f, 0.5f, 0.5f), // I10 (V6)
            new Vector3(0.5f, 0.5f, -0.5f), // I11 (V2)

            // Left face vertices
            new Vector3(-0.5f, -0.5f, 0.5f), // I12 (V4)
            new Vector3(-0.5f, -0.5f, -0.5f), // I13 (V0)
            new Vector3(-0.5f, 0.5f, -0.5f), // I14 (V3)
            new Vector3(-0.5f, 0.5f, 0.5f), // I15 (V7)

            // Top face vertices
            new Vector3(-0.5f, 0.5f, -0.5f), // I16 (V3)
            new Vector3(0.5f, 0.5f, -0.5f), // I17 (V2)
            new Vector3(0.5f, 0.5f, 0.5f), // I18 (V6)
            new Vector3(-0.5f, 0.5f, 0.5f), // I19 (V7)

            // Bottom face vertices
            new Vector3(-0.5f, -0.5f, 0.5f), // I20 (V4)
            new Vector3(0.5f, -0.5f, 0.5f), // I21 (V5)
            new Vector3(0.5f, -0.5f, -0.5f), // I22 (V1)
            new Vector3(-0.5f, -0.5f, -0.5f), // I23 (V0)
        };

        // NOTE: Winding order in Unity is **clock-wise**.
        var triangles = new int[]
        {
            // Front face triangles
            0, 2, 1, 0, 3, 2,

            // Back face triangles
            4, 5, 6, 4, 6, 7,

            // Right face triangles
            8, 10, 9, 8, 11, 10,

            // Left face triangles
            12, 14, 13, 12, 15, 14,

            // Top face triangles
            16, 18, 17, 16, 19, 18,

            // Bottom face triangles
            20, 22, 21, 20, 23, 22
        };

        var normals = new Vector3[]
        {
            // Front face normals
            Vector3.back, Vector3.back, Vector3.back, Vector3.back,

            // Back face normals
            Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,

            // Right face normals
            Vector3.right, Vector3.right, Vector3.right, Vector3.right,

            // Left face normals
            Vector3.left, Vector3.left, Vector3.left, Vector3.left,

            // Top face normals
            Vector3.up, Vector3.up, Vector3.up, Vector3.up,

            // Bottom face normals
            Vector3.down, Vector3.down, Vector3.down, Vector3.down,
        };

        _mesh = new Mesh
        {
            name = "ProceduralBlock",
            vertices = vertices,
            triangles = triangles,
            normals = normals,
        };

        _meshFilter.sharedMesh = _mesh;
    }
}