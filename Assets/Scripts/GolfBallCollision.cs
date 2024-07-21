using Unity.VisualScripting;
using UnityEngine;

public class GolfBallCollision : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody golfBallRigidbody;
    private GameUIManager gameUIManager; // Reference to the GameUIManager
    private GameObject lastGolfBallInstance;

    private float collisionTimer = 0f;
    private bool isTimerActive = false;
    private const float respawnDelay = 3f;
    private bool hasHitBall = false;
    private Vector3 spawnBallPosition;
    private Quaternion initRotation = Quaternion.Euler(0, 0, 0);

    // Audio sources for the collision sounds
    public AudioSource golfHitAudioSource;
    public AudioSource ballInHoleAudioSource;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        golfBallRigidbody = GetComponent<Rigidbody>();
        gameUIManager = FindObjectOfType<GameUIManager>(); // Find the GameUIManager in the scene
        spawnBallPosition = playerController.GetSpawnBallPosition();

        // Ensure the audio sources are assigned
        if (golfHitAudioSource == null || ballInHoleAudioSource == null)
        {
            Debug.LogError("Audio sources are not assigned.");
        }

        lastGolfBallInstance = playerController.GetGolfBallInstance();
    }

    void Update()
    {
        if (!hasHitBall)
        {
            lastGolfBallInstance.transform.position = spawnBallPosition;
            lastGolfBallInstance.transform.rotation = initRotation;
        }

        if (isTimerActive)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer >= respawnDelay)
            {
                isTimerActive = false;
                collisionTimer = 0f;
                Rigidbody lastRB = lastGolfBallInstance.GetComponent<Rigidbody>();
                lastRB.isKinematic = true;
                lastGolfBallInstance = playerController.RespawnGolfBall();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (gameObject != lastGolfBallInstance) { return; }
        if (collision.gameObject.CompareTag("GolfClub"))
        {
            hasHitBall = true;
            golfBallRigidbody.isKinematic = false;

            // Get the calculated velocity from the PlayerController
            Vector3 clubHeadVelocity = playerController.GetManualClubHeadVelocity();


            float scaleFactor = 0.2f;
            Vector3 scaledVelocity = clubHeadVelocity * scaleFactor;


            float impulseMagnitude = scaledVelocity.magnitude * 1; // Calculate the impulse magnitude
            Vector3 impulse = scaledVelocity.normalized * impulseMagnitude;

            golfBallRigidbody.AddForce(new Vector3(impulse.x, 0, impulse.z), ForceMode.Impulse);

            // Play the golf hit sound
            if (golfHitAudioSource != null)
            {
                golfHitAudioSource.mute = false;
                golfHitAudioSource.Play();
                golfHitAudioSource.mute = true;
            }

            // Update the shot number in the UI
            if (gameUIManager != null)
            {
                gameUIManager.UpdateShotNumber();
            }

            // Start the timer for respawning the golf ball
            isTimerActive = true;
            collisionTimer = 0f;
        }

        if (collision.gameObject.CompareTag("Hole"))
        {
            Debug.Log("Ball in Hole");

            // Play the ball in hole sound only if not already in the hole
            if (ballInHoleAudioSource != null)
            {
                Debug.Log("[SOUND] playing ball in hole");
                ballInHoleAudioSource.Play();
            }

            if (gameUIManager != null)
            {
                gameUIManager.UpdateScore();
            }
        }
    }
}
