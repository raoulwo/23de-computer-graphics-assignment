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
            new Vector3(-0.5f, -0.5f, -0.5f), // V0
            new Vector3(0.5f, -0.5f, -0.5f), // V1
            new Vector3(0.5f, 0.5f, -0.5f), // V2
            new Vector3(-0.5f, 0.5f, -0.5f), // V3

            // Back face vertices
            new Vector3(-0.5f, -0.5f, 0.5f), // V4
            new Vector3(0.5f, -0.5f, 0.5f), // V5
            new Vector3(0.5f, 0.5f, 0.5f), // V6
            new Vector3(-0.5f, 0.5f, 0.5f), // V7

            // Right face vertices
            new Vector3(0.5f, -0.5f, -0.5f), // V1
            new Vector3(0.5f, -0.5f, 0.5f), // V5
            new Vector3(0.5f, 0.5f, 0.5f), // V6
            new Vector3(0.5f, 0.5f, -0.5f), // V2
            
            // Left face vertices
            new Vector3(-0.5f, -0.5f, 0.5f), // V4
            new Vector3(-0.5f, -0.5f, -0.5f), // V0
            new Vector3(-0.5f, 0.5f, -0.5f), // V3
            new Vector3(-0.5f, 0.5f, 0.5f), // V7
            
            // Top face vertices
            new Vector3(-0.5f, 0.5f, -0.5f), // V3
            new Vector3(0.5f, 0.5f, -0.5f), // V2
            new Vector3(0.5f, 0.5f, 0.5f), // V6
            new Vector3(-0.5f, 0.5f, 0.5f), // V7
            
            // Bottom face vertices
            new Vector3(-0.5f, -0.5f, 0.5f), // V4
            new Vector3(0.5f, -0.5f, 0.5f), // V5
            new Vector3(0.5f, -0.5f, -0.5f), // V1
            new Vector3(-0.5f, -0.5f, -0.5f), // V0
        };

        // NOTE: Winding order in Unity is **clock-wise**.
        var triangles = new int[]
        {
            // Front face triangles
            0, 2, 1, 0, 3, 2,

            // Back face triangles
            4, 5, 6, 4, 6, 7,

            // Right face triangles
            1, 6, 5, 1, 2, 6,
            
            // Left face triangles
            4, 3, 0, 4, 7, 3,
            
            // Top face triangles
            3, 6, 2, 3, 7, 6,
            
            // Bottom face triangles
            4, 1, 5, 4, 0, 1,
        };

        _mesh = new Mesh
        {
            name = "ProceduralBlock",
            vertices = vertices,
            triangles = triangles,
        };

        _meshFilter.sharedMesh = _mesh;
    }
}