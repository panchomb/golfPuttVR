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
            Vector3 clubVelocity = playerController.GetManualClubVelocity();
            Debug.Log("Club velocity at collision: " + clubVelocity);

            // Scale up the velocity for stronger interaction
            float scaleFactor = 0.25f;
            Vector3 scaledVelocity = clubVelocity * scaleFactor;
            Debug.Log("Scaled club velocity: " + scaledVelocity);

            float impulseMagnitude = scaledVelocity.magnitude * 1; // Calculate the impulse magnitude
            Debug.Log("Impulse magnitude: " + impulseMagnitude);
            Vector3 impulse = scaledVelocity.normalized * impulseMagnitude;
            Debug.Log("Impulse vector: " + impulse);

            golfBallRigidbody.isKinematic = false; // Ensure the golf ball is not kinematic
            golfBallRigidbody.velocity = Vector3.zero; // Reset velocity
            golfBallRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity

            golfBallRigidbody.AddForce(new Vector3(impulse.x, impulse.y * 0.5f, impulse.z), ForceMode.Impulse);
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
