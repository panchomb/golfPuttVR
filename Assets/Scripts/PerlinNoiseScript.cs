using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GolfTerrainGenerator : MonoBehaviour
{
    public int terrainWidth = 128;
    public int terrainHeight = 128;
    public int heightmapResolution = 257; // Example resolution, can be set in Unity inspector
    public float scale = 3f;
    public float offsetX = 100f;
    public float offsetY = 100f;
    public float heightMultiplier = 0.5f;
    public float heightBias = 0.25f;
    public TerrainLayer roughTexture;
    public TerrainLayer greenTexture;
    public int greenResolution = 100;
    public float perlinScale = 0.05f; // Adjusted scale for Perlin noise
    public float perlinInfluence = 5.0f;
    public GameObject pointPrefab; // Reference to the sphere prefab
    public GameObject flagPrefab;
    public float desiredHoleRadius = 0.5f; // Radius of the hole
    public float holeMargin = 160f; // Margin from the edge of the green
    public float gradientTextureIntensity = 0.3f;
    public TerrainLayer red;
    public TerrainLayer green;
    public TerrainLayer blue;

    private float greenRadius;
    private Terrain terrain;
    private Vector3 flagOffset = new Vector3((35.0f-26.078f), (4.95f-5.52f)-0.1f, (42.0f-40.71f));
    private bool showHeightmap = false;
    private float[,,] greenAlphaMap;
    private float[,,] heightmapColorAlphaMap;


    void Start()
    {
        terrain = GetComponent<Terrain>();

        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        // Calculate the green radius
        float halfDimension = Mathf.Min(terrainWidth, terrainHeight) / 2f;
        greenRadius = Random.Range(0.8f * halfDimension, 0.9f * halfDimension);

        GenerateTerrain();
        SaveGreenAlphaMap();
        GenerateHeightmapColorAlphaMap();
    }

    void GenerateTerrain()
    {
        float[,] heights = GenerateHeightmap();
        ApplyHeightmap(heights);
        ApplyTextures();
    }

    float[,] GenerateHeightmap()
    {
        float[,] heights = new float[heightmapResolution, heightmapResolution];
        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int y = 0; y < heightmapResolution; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / heightmapResolution * scale + offsetX;
        float yCoord = (float)y / heightmapResolution * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord) * heightMultiplier + heightBias;
    }

    void SaveGreenAlphaMap()
    {
        int alphaMapWidth = terrain.terrainData.alphamapWidth;
        int alphaMapHeight = terrain.terrainData.alphamapHeight;
        greenAlphaMap = terrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
    }


    void ApplyHeightmap(float[,] heights)
    {
        terrain.terrainData.heightmapResolution = heightmapResolution;
        terrain.terrainData.size = new Vector3(terrainWidth, 10, terrainHeight);
        terrain.terrainData.SetHeights(0, 0, heights);
    }
    void GenerateHeightmapColorAlphaMap()
    {
        int alphaMapWidth = terrain.terrainData.alphamapWidth;
        int alphaMapHeight = terrain.terrainData.alphamapHeight;
        heightmapColorAlphaMap = new float[alphaMapWidth, alphaMapHeight, 5];

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        // First pass to find min and max height values using terrain.SampleHeight
        for (int x = 0; x < alphaMapWidth; x++)
        {
            for (int y = 0; y < alphaMapHeight; y++)
            {
                float xPos = x * (terrainWidth / (float)alphaMapWidth);
                float yPos = y * (terrainHeight / (float)alphaMapHeight);
                float heightValue = terrain.SampleHeight(new Vector3(xPos, 0, yPos));
                if (heightValue < minHeight)
                {
                    minHeight = heightValue;
                }
                if (heightValue > maxHeight)
                {
                    maxHeight = heightValue;
                }
            }
        }

        // Second pass to apply min-max scaling and assign colors
        for (int x = 0; x < alphaMapWidth; x++)
        {
            for (int y = 0; y < alphaMapHeight; y++)
            {
                float xPos = x * (terrainWidth / (float)alphaMapWidth);
                float yPos = y * (terrainHeight / (float)alphaMapHeight);
                float heightValue = terrain.SampleHeight(new Vector3(xPos, 0, yPos));
                // Apply min-max scaling
                float scaledHeightValue = (heightValue - minHeight) / (maxHeight - minHeight);
                // Color gradient from dark blue to bright green
                Color gradientColor = Color.Lerp(Color.blue, Color.green, scaledHeightValue);

                heightmapColorAlphaMap[y, x, 0] = greenAlphaMap[y, x, 0] * 0.5f; // Rough channel
                heightmapColorAlphaMap[y, x, 1] = greenAlphaMap[y, x, 1] * 0.5f; // Green channel
                heightmapColorAlphaMap[y, x, 2] = gradientColor.r * gradientTextureIntensity; // Red channel
                heightmapColorAlphaMap[y, x, 3] = gradientColor.g * gradientTextureIntensity; // Green channel
                heightmapColorAlphaMap[y, x, 4] = gradientColor.b * gradientTextureIntensity; // Blue channel
            }
        }
    }


    void ApplyTextures()
    {
        TerrainLayer[] terrainLayers = new TerrainLayer[5];
        terrainLayers[0] = roughTexture;
        terrainLayers[1] = greenTexture;
        terrainLayers[2] = red;
        terrainLayers[3] = green;
        terrainLayers[4] = blue;

        terrain.terrainData.terrainLayers = terrainLayers;

        int alphaMapWidth = terrain.terrainData.alphamapWidth;
        int alphaMapHeight = terrain.terrainData.alphamapHeight;
        float[,,] alphaMap = new float[alphaMapWidth, alphaMapHeight, 5];

        Debug.Log("Starting green shape generation");

        Vector2 greenCenter = new Vector2(alphaMapWidth / 2, alphaMapHeight / 2);
        Vector2[] greenShape = lumpyCircle(greenCenter, greenRadius * alphaMapWidth / terrainWidth, greenResolution, perlinScale, perlinInfluence, offsetX, offsetY);

        Debug.Log("Green shape generated");

        List<Vector2> insideGreenPoints = new List<Vector2>();

        for (int x = 0; x < alphaMapWidth; x++)
        {
            for (int y = 0; y < alphaMapHeight; y++)
            {
                bool isInsideGreen = ContainsPoint(new Vector2(x, y), greenShape);
                if (isInsideGreen)
                {
                    alphaMap[y, x, 0] = 0; // Rough
                    alphaMap[y, x, 1] = 1; // Green
                    alphaMap[y, x, 2] = 0; // Red
                    alphaMap[y, x, 3] = 0; // Green
                    alphaMap[y, x, 4] = 0; // Blue
                    insideGreenPoints.Add(new Vector2(x * terrainWidth / alphaMapWidth, y * terrainHeight / alphaMapHeight));
                }
                else
                {
                    alphaMap[y, x, 0] = 1; // Rough
                    alphaMap[y, x, 1] = 0; // Green
                    alphaMap[y, x, 2] = 0; // Red
                    alphaMap[y, x, 3] = 0; // Green
                    alphaMap[y, x, 4] = 0; // Blue
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphaMap);

        Debug.Log("Green shape applied");

        // Place the hole
        PlaceHole(insideGreenPoints, greenShape);
    }

    void PlaceHole(List<Vector2> insideGreenPoints, Vector2[] greenContour)
    {
        int maxAttempts = 100;
        int attempt = 0;

        while (attempt < maxAttempts)
        {
            Vector2 randomPoint = insideGreenPoints[Random.Range(0, insideGreenPoints.Count)];

            if (!IsPointNearEdge(randomPoint, greenContour, holeMargin))
            {
                Vector3 holePosition = new Vector3(randomPoint.x, 0, randomPoint.y); // Terrain position
                holePosition.y = terrain.SampleHeight(holePosition) + terrain.GetPosition().y; // Adjust height to terrain
                // Generate a hole in the terrain
                MakeHoleInTerrain(holePosition);

                if (flagPrefab != null)
                {
                    Vector3 flagPosition = new Vector3(randomPoint.x, 0, randomPoint.y);
                    flagPosition -= flagOffset;
                    flagPosition.y = terrain.SampleHeight(flagPosition) + terrain.GetPosition().y;
                    //Instantiate(flagPrefab, flagPosition, Quaternion.identity);
                    flagPrefab.transform.position = flagPosition;
                }
                else {
                    Debug.LogWarning("No flag prefab assigned to the GolfTerrainGenerator script.");
                }
                return;
            }


            attempt++;
        }

        Debug.LogError("No valid points inside the green to place the hole after maximum attempts!");
    }

    void MakeHoleInTerrain(Vector3 holePosition)
    {
        int holeResolution = terrain.terrainData.holesResolution;
        bool[,] holes = terrain.terrainData.GetHoles(0, 0, holeResolution, holeResolution);
        Debug.Log("Hole resolution: " + holeResolution);

        // Initialize the holes array to true (no holes)
        for (int x = 0; x < holeResolution; x++)
        {
            for (int y = 0; y < holeResolution; y++)
            {
                holes[x, y] = true;
            }
        }

        // Calculate the desired radius in hole resolution units
        int holeRadiusInHoles = Mathf.RoundToInt((desiredHoleRadius / terrain.terrainData.size.x) * holeResolution);

        // Calculate the center of the terrain in hole resolution coordinates
        float holePosX = (holePosition.x / terrain.terrainData.size.x) * holeResolution;
        float holePosZ = (holePosition.z / terrain.terrainData.size.z) * holeResolution;

        Vector2 holeCenter = new Vector2(holePosX, holePosZ);

        // Loop through the area around the hole center and create a circular hole
        for (int x = -holeRadiusInHoles; x <= holeRadiusInHoles; x++)
        {
            for (int y = -holeRadiusInHoles; y <= holeRadiusInHoles; y++)
            {
                int xCoord = Mathf.RoundToInt(holePosX) + x;
                int yCoord = Mathf.RoundToInt(holePosZ) + y;

                if (xCoord >= 0 && xCoord < holeResolution && yCoord >= 0 && yCoord < holeResolution)
                {
                    // Check if the current point is within the desired radius
                    if (Vector2.Distance(new Vector2(xCoord, yCoord), holeCenter) <= holeRadiusInHoles/2)
                    {
                        holes[yCoord, xCoord] = false; // Create a hole
                    }
                }
            }
        }

        terrain.terrainData.SetHoles(0, 0, holes);
    }


    bool IsPointNearEdge(Vector2 point, Vector2[] greenContour, float margin)
    {
        foreach (Vector2 edgePoint in greenContour)
        {
            if (Vector2.Distance(point, edgePoint) < margin)
            {
                return true;
            }
        }
        return false;
    }

    public static Vector2[] lumpyCircle(Vector2 center, float radius, int resolution, float perlinScale, float perlinInfluence, float offX, float offY)
    {
        Debug.Log("Generating lumpy circle with center " + center + ", radius " + radius);
        // insert debugging pointPrefab at the center of the green
        Vector2[] vertices = new Vector2[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float angle = (2 * Mathf.PI * i) / resolution;
            float r = Mathf.PerlinNoise((Mathf.Sin(angle) * perlinScale) + offX, (Mathf.Cos(angle) * perlinScale) + offY) * perlinInfluence;
            r += radius;
            vertices[i] = new Vector2(center.x + r * Mathf.Sin(angle), center.y + r * Mathf.Cos(angle));
        }
        return vertices;
    }

    public static bool ContainsPoint(Vector2 p, Vector2[] polyPoints)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            showHeightmap = !showHeightmap;
            if (showHeightmap)
            {
                terrain.terrainData.SetAlphamaps(0, 0, heightmapColorAlphaMap);
            }
            else
            {
                terrain.terrainData.SetAlphamaps(0, 0, greenAlphaMap);
            }
        }
    }

}
