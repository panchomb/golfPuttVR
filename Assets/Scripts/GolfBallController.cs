using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    public GameObject golfBallPrefab; // Assign the sphere prefab in the Inspector
    public float spawnHeight = 5.0f;  // Height to spawn the ball above the green
    public float angularDrag = 2.0f;  // Angular drag value to stop the ball
    public float frictionCoefficient = 0.2f; // Friction coefficient to decelerate the ball
    public Vector3 ballScale = new Vector3(1.0f, 1.0f, 1.0f); // Scale of the ball

    public float minVelocity = 0.1f; // Minimum velocity to stop the ball

    private Terrain terrain;
    private GameObject golfBall;
    private Rigidbody rb;

    [SerializeField]
    private Material ballMaterial;

    void Start()
    {
        terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            Debug.LogError("No active terrain found.");
            return;
        }

        // Retrieve the terrain width and height from the terrain data
        int terrainWidth = (int)terrain.terrainData.size.x;
        int terrainHeight = (int)terrain.terrainData.size.z;

        // Find the center of the green
        Vector3 greenCenter = new Vector3(terrainWidth / 2, 0, terrainHeight / 2);

        // Calculate the height of the terrain at the center of the green
        float terrainHeightAtCenter = terrain.SampleHeight(greenCenter);

        // Place the ball slightly above the green center
        Vector3 spawnPosition = new Vector3(greenCenter.x, terrainHeightAtCenter + spawnHeight, greenCenter.z);
        spawnPosition.y += spawnHeight;

        // Instantiate the golf ball at the spawn position
        golfBall = Instantiate(golfBallPrefab, spawnPosition, Quaternion.identity);
        golfBall.transform.localScale = ballScale;

        // Ensure the golf ball has a Rigidbody
        rb = golfBall.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = golfBall.AddComponent<Rigidbody>();
        }

        // Set the golf ball's gravity and angular drag
        rb.useGravity = true;
        rb.angularDrag = angularDrag;

        // Set material to ball
        golfBall.GetComponent<Renderer>().material = ballMaterial;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply friction force opposite to the current velocity
            Vector3 frictionForce = -rb.velocity.normalized * frictionCoefficient;
            rb.AddForce(frictionForce, ForceMode.Acceleration);

            // Log the current velocity
            //Debug.Log("Current Velocity: " + rb.velocity);

            // Stop the ball if its velocity is very low to prevent infinite sliding
            if (rb.velocity.magnitude < minVelocity)
            {
                rb.velocity = Vector3.zero;
                //Debug.Log("Ball stopped due to low velocity.");
            }
        }
    }
}
