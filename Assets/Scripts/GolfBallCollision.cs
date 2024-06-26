using UnityEngine;

public class GolfBallCollision : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody golfBallRigidbody;
    private GameUIManager gameUIManager; // Reference to the GameUIManager

    private float collisionTimer = 0f;
    private bool isTimerActive = false;
    private const float respawnDelay = 2f;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        golfBallRigidbody = GetComponent<Rigidbody>();
        gameUIManager = FindObjectOfType<GameUIManager>(); // Find the GameUIManager in the scene
    }

    void Update()
    {
        if (isTimerActive)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer >= respawnDelay)
            {
                isTimerActive = false;
                collisionTimer = 0f;
                playerController.RespawnGolfBall();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GolfClub"))
        {
            golfBallRigidbody.isKinematic = false;
            Debug.Log("Collision between golf ball and golf club");

            // Get the calculated velocity from the PlayerController
            Vector3 clubHeadVelocity = playerController.GetManualClubHeadVelocity();
            Debug.Log("Club head velocity at collision: " + clubHeadVelocity);

            float scaleFactor = 0.15f;
            Vector3 scaledVelocity = clubHeadVelocity * scaleFactor;
            Debug.Log("Scaled club head velocity: " + scaledVelocity);

            float impulseMagnitude = scaledVelocity.magnitude * 1; // Calculate the impulse magnitude
            Debug.Log("Impulse magnitude: " + impulseMagnitude);
            Vector3 impulse = scaledVelocity.normalized * impulseMagnitude;
            Debug.Log("Impulse vector: " + impulse);

            golfBallRigidbody.AddForce(new Vector3(impulse.x, 0, impulse.z), ForceMode.Impulse);
            Debug.Log("Impulse applied to golf ball");

            // Update the shot number in the UI
            if (gameUIManager != null)
            {
                gameUIManager.UpdateShotNumber();
            }

            // Start the timer for respawning the golf ball
            isTimerActive = true;
            collisionTimer = 0f;
        }

        if (collision.gameObject.CompareTag("BottomTerrain"))
        {
            Debug.Log("Ball in Hole");
            if (gameUIManager != null)
            {
                gameUIManager.UpdateScore();
            }
        }
    }
}
