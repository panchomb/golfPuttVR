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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            if (rb.velocity.magnitude < minVelocity)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
