using System;
using UnityEngine;

public enum BlockType
{
    Dirt,
    Stone,
    CraftingTable,
    Furnace,
}

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ProceduralBlock : MonoBehaviour
{
    private const string UrpLitShader = "Universal Render Pipeline/Lit";

    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    [SerializeField] private Texture2D textureAtlas;
    [SerializeField] private BlockType blockType;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private struct FaceTiles
    {
        public Vector2Int Front;
        public Vector2Int Back;
        public Vector2Int Right;
        public Vector2Int Left;
        public Vector2Int Top;
        public Vector2Int Bottom;
    }

    // We use `OnEnable` instead of `Start` so that the block is also rendered in
    // the editor view, not only during play.
    private void OnEnable()
    {
        BuildBlock();
    }

    // Build the block live any time we change a serialized variable.
    private void OnValidate()
    {
        BuildBlock();
    }

    private void BuildBlock()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();

        var material = new Material(Shader.Find(UrpLitShader));
        if (textureAtlas != null)
        {
            material.SetTexture(BaseMap, textureAtlas);
        }

        _meshRenderer.sharedMaterial = material;

        // NOTE: I've defined the vertex order for a single face the following way.
        // When looking straight at the face, the vertices are in order:
        //
        // 1. Bottom Left
        // 2. Bottom Right
        // 3. Top Right
        // 4. Top Left
        //
        // This is important for when we calculate UVs to map the 2D texture corners
        // to our 3D vertices so that the block textures are displayed correctly!
        var vertices = new[]
        {
            // Front face vertices
            new Vector3(-0.5f, -0.5f, -0.5f), // I0 (V0)
            new Vector3(0.5f, -0.5f, -0.5f), // I1 (V1)
            new Vector3(0.5f, 0.5f, -0.5f), // I2 (V2)
            new Vector3(-0.5f, 0.5f, -0.5f), // I3 (V3)

            // Back face vertices
            new Vector3(0.5f, -0.5f, 0.5f), // I4 (V5)
            new Vector3(-0.5f, -0.5f, 0.5f), // I5 (V4)
            new Vector3(-0.5f, 0.5f, 0.5f), // I6 (V7)
            new Vector3(0.5f, 0.5f, 0.5f), // I7 (V6)

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
        var triangles = new[]
        {
            // Front face triangles
            0, 2, 1, 0, 3, 2,

            // Back face triangles
            4, 6, 5, 4, 7, 6,

            // Right face triangles
            8, 10, 9, 8, 11, 10,

            // Left face triangles
            12, 14, 13, 12, 15, 14,

            // Top face triangles
            16, 18, 17, 16, 19, 18,

            // Bottom face triangles
            20, 22, 21, 20, 23, 22,
        };

        var normals = new[]
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

        var tiles = GetFaceTiles(blockType);

        var uvs = new Vector2[24];
        Array.Copy(GetTileUVs(tiles.Front), 0, uvs, 0, 4);
        Array.Copy(GetTileUVs(tiles.Back), 0, uvs, 4, 4);
        Array.Copy(GetTileUVs(tiles.Right), 0, uvs, 8, 4);
        Array.Copy(GetTileUVs(tiles.Left), 0, uvs, 12, 4);
        Array.Copy(GetTileUVs(tiles.Top), 0, uvs, 16, 4);
        Array.Copy(GetTileUVs(tiles.Bottom), 0, uvs, 20, 4);

        if (_mesh == null)
        {
            _mesh = new Mesh { name = "ProceduralBlock" };
        }
        else
        {
            // NOTE: We need to clear the mesh first before updating it live (else we'll leak memory).
            _mesh.Clear();
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.normals = normals;
        _mesh.uv = uvs;

        _meshFilter.sharedMesh = _mesh;
    }

    private static FaceTiles GetFaceTiles(BlockType type)
    {
        return type switch
        {
            BlockType.Dirt => new FaceTiles
            {
                Front = new Vector2Int(2, 15),
                Back = new Vector2Int(2, 15),
                Right = new Vector2Int(2, 15),
                Left = new Vector2Int(2, 15),
                Top = new Vector2Int(2, 15),
                Bottom = new Vector2Int(2, 15),
            },
            BlockType.Stone => new FaceTiles
            {
                Front = new Vector2Int(1, 15),
                Back = new Vector2Int(1, 15),
                Right = new Vector2Int(1, 15),
                Left = new Vector2Int(1, 15),
                Top = new Vector2Int(1, 15),
                Bottom = new Vector2Int(1, 15),
            },
            BlockType.CraftingTable => new FaceTiles
            {
                Front = new Vector2Int(12, 12),
                Back = new Vector2Int(12, 12),
                Right = new Vector2Int(11, 12),
                Left = new Vector2Int(11, 12),
                Top = new Vector2Int(11, 13),
                Bottom = new Vector2Int(4, 15),
            },
            BlockType.Furnace => new FaceTiles
            {
                Front = new Vector2Int(12, 13),
                Back = new Vector2Int(13, 13),
                Right = new Vector2Int(13, 13),
                Left = new Vector2Int(13, 13),
                Top = new Vector2Int(14, 12),
                Bottom = new Vector2Int(14, 12),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// This helper method maps the tile coordinates from 0-15 of our block atlas
    /// to corresponding UVs ranging from 0-1. U represents the horizontal axis,
    /// V the vertical axis. The origin being at the bottom left at (0,0),
    /// top right being at (1, 1).
    /// </summary>
    private static Vector2[] GetTileUVs(int tileX, int tileY)
    {
        // NOTE: The provided block atlas has dimensions of 16x16 tiles (blocks).
        // That's why we divide by 16.0f to get to the size of a single tile.
        const float tileSize = 1.0f / 16.0f;

        var uMin = tileX * tileSize;
        var vMin = tileY * tileSize;
        var uMax = (tileX + 1) * tileSize;
        var vMax = (tileY + 1) * tileSize;

        // NOTE: Return the UVs matching the order of our vertices (defined above).
        return new[]
        {
            new Vector2(uMin, vMin), // Bottom left
            new Vector2(uMax, vMin), // Bottom right
            new Vector2(uMax, vMax), // Top right
            new Vector2(uMin, vMax), // Top left
        };
    }

    /// <inheritdoc cref="GetTileUVs(int, int)" />
    private static Vector2[] GetTileUVs(Vector2Int tileCoords)
    {
        return GetTileUVs(tileCoords.x, tileCoords.y);
    }
}