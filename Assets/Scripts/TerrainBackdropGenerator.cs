using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBackdropGenerator : MonoBehaviour
{
    public Terrain mainTerrain; // Reference to the main terrain
    public TerrainData backdropTerrainData; // TerrainData for the backdrop terrain
    public GameObject[] treePrefabs; // Array of tree prefabs to be used for the backdrop
    public int treeDensity = 1000; // Number of trees to place
    public float treeDistance = 50f; // Distance from the wall where trees start
    public float treeAreaWidth = 100f; // Width of the tree area
    public float minTreeScale = 0.5f; // Minimum scale for the trees
    public float maxTreeScale = 2.0f; // Maximum scale for the trees

    void Start()
    {
        // Initialize the random seed with a unique value
        Random.InitState(System.DateTime.Now.Millisecond);

        GenerateBackdrop();
    }

    void GenerateBackdrop()
    {
        Vector3 mainTerrainSize = mainTerrain.terrainData.size;
        float backdropWidth = mainTerrainSize.x + 2 * treeAreaWidth;
        float backdropHeight = mainTerrainSize.z + 2 * treeAreaWidth;

        // Create a new terrain for the backdrop
        TerrainData newTerrainData = Instantiate(backdropTerrainData);
        newTerrainData.size = new Vector3(backdropWidth, mainTerrainSize.y, backdropHeight);

        GameObject backdropTerrainObject = Terrain.CreateTerrainGameObject(newTerrainData);
        Terrain backdropTerrain = backdropTerrainObject.GetComponent<Terrain>();

        backdropTerrain.transform.position = new Vector3(
            mainTerrain.transform.position.x - treeAreaWidth,
            2.0f,
            mainTerrain.transform.position.z - treeAreaWidth
        );

        // Add trees to the backdrop terrain
        SetTreePrototypes(backdropTerrain);
        PlaceTrees(backdropTerrain);
    }

    void SetTreePrototypes(Terrain terrain)
    {
        List<TreePrototype> treePrototypes = new List<TreePrototype>();
        foreach (GameObject treePrefab in treePrefabs)
        {
            TreePrototype treePrototype = new TreePrototype();
            treePrototype.prefab = treePrefab;
            treePrototypes.Add(treePrototype);
        }
        terrain.terrainData.treePrototypes = treePrototypes.ToArray();
    }

    void PlaceTrees(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        List<TreeInstance> trees = new List<TreeInstance>();

        for (int i = 0; i < treeDensity; i++)
        {
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            float y = terrainData.GetHeight((int)(x * terrainData.heightmapResolution), (int)(z * terrainData.heightmapResolution)) / terrainData.size.y;

            // Ensure trees are placed outside the main terrain area
            Vector3 worldPosition = new Vector3(x * terrainData.size.x, y * terrainData.size.y, z * terrainData.size.z) + terrain.transform.position;
            if (worldPosition.x < mainTerrain.transform.position.x ||
                worldPosition.x > mainTerrain.transform.position.x + mainTerrain.terrainData.size.x ||
                worldPosition.z < mainTerrain.transform.position.z ||
                worldPosition.z > mainTerrain.transform.position.z + mainTerrain.terrainData.size.z)
            {
                // Randomly select a tree prefab
                int randomIndex = Random.Range(0, treePrefabs.Length);

                // Create a TreeInstance with random scale
                float scale = Random.Range(minTreeScale, maxTreeScale);
                float randomRotation = Random.Range(0f, 360f);

                TreeInstance tree = new TreeInstance
                {
                    position = new Vector3(x, y, z),
                    widthScale = scale,
                    heightScale = scale,
                    color = Color.white,
                    lightmapColor = Color.white,
                    prototypeIndex = randomIndex
                };

                // Set rotation using the prefab's transform
                tree.rotation = randomRotation;

                trees.Add(tree);
            }
        }

        terrainData.treeInstances = trees.ToArray();
    }
}
