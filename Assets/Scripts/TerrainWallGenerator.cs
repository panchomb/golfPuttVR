using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainWallGenerator : MonoBehaviour
{
    public float wallHeight = 10f; // Height of the wall
    public float wallThickness = 1f; // Thickness of the wall
    public Material wallMaterial; // Material to apply to the wall

    private Terrain terrain;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        GenerateWalls();
    }

    void GenerateWalls()
    {
        Vector3 terrainSize = terrain.terrainData.size;

        // Create the four walls
        CreateWall(new Vector3(terrainSize.x / 2, wallHeight / 2, 0), new Vector3(terrainSize.x, wallHeight, wallThickness), true); // Front
        CreateWall(new Vector3(terrainSize.x / 2, wallHeight / 2, terrainSize.z), new Vector3(terrainSize.x, wallHeight, wallThickness), true); // Back
        CreateWall(new Vector3(0, wallHeight / 2, terrainSize.z / 2), new Vector3(wallThickness, wallHeight, terrainSize.z), false); // Left
        CreateWall(new Vector3(terrainSize.x, wallHeight / 2, terrainSize.z / 2), new Vector3(wallThickness, wallHeight, terrainSize.z), false); // Right
    }

    void CreateWall(Vector3 position, Vector3 scale, bool isHorizontal)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.parent = transform;

        if (wallMaterial != null)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            renderer.material = wallMaterial;
            
            // Adjust the tiling of the material to ensure it repeats correctly
            if (isHorizontal)
            {
                renderer.material.mainTextureScale = new Vector2(scale.x / wallThickness, scale.y / wallThickness);
            }
            else
            {
                renderer.material.mainTextureScale = new Vector2(scale.z / wallThickness, scale.y / wallThickness);
            }
        }
    }
}
    