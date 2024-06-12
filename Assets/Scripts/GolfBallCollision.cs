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
        if (!hasCollided && collision.gameObject.CompareTag("GolfClub"))
        {
            Debug.Log("Collision between golf ball and golf club");
            playerController.StartGame();
            hasCollided = true;
        }
    }
}
