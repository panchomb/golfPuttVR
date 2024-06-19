using UnityEngine;

public class GolfBallCollision : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody golfBallRigidbody;
    private bool hasCollided = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        golfBallRigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GolfClub"))
        {
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

            hasCollided = true;
        }
    }

    public void ResetCollisionFlag()
    {
        hasCollided = false;
        Debug.Log("Collision flag reset.");
    }
}
