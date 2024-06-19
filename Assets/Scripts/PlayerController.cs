using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ovrCameraRigInteractions;
    public GameObject terrain;
    public GameObject golfClub;
    public GameObject golfBall;

    private Rigidbody golfBallRigidbody;
    private Rigidbody golfClubRigidbody;
    private Vector3 initialGolfBallPosition;
    private GolfBallCollision golfBallCollisionScript;
    private Vector3 currentClubPosition;
    private Vector3 previousClubPosition;
    private Vector3 clubVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Terrain terrainComponent = terrain.GetComponent<Terrain>();
        Vector3 terrainCenter = terrainComponent.terrainData.bounds.center;
        Vector3 worldCenterPosition = terrain.transform.position + terrainCenter;

        float height = terrainComponent.SampleHeight(new Vector3(worldCenterPosition.x, worldCenterPosition.y, worldCenterPosition.z));
        Debug.Log("[DEBUG] height is " + height);
     
        ovrCameraRigInteractions.transform.position = new Vector3(worldCenterPosition.x, height - 0.3f, worldCenterPosition.z);
        golfClub.transform.position = new Vector3(worldCenterPosition.x, height + 5.5f, worldCenterPosition.z);
        golfBall.transform.position = new Vector3(worldCenterPosition.x, height + 1, worldCenterPosition.z);

        golfBallRigidbody = golfBall.GetComponent<Rigidbody>();
        golfClubRigidbody = golfClub.GetComponent<Rigidbody>();
        initialGolfBallPosition = golfBall.transform.position;

        // Get the GolfBallCollision script
        golfBallCollisionScript = golfBall.GetComponent<GolfBallCollision>();

        Debug.Log("Start method executed. Golf ball and club positions set.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RespawnGolfBall();
        }
    }

    void FixedUpdate()
    {
        // Manual velocity calculation for kinematic golf club
        Vector3 currentClubPosition = golfClub.transform.position;
        clubVelocity = (currentClubPosition - previousClubPosition) / Time.fixedDeltaTime;
        previousClubPosition = currentClubPosition;
    }

    void RespawnGolfBall()
    {
        // Position the golf ball under the player's position on top of the terrain
        Vector3 playerPosition = ovrCameraRigInteractions.transform.position;
        Terrain terrainComponent = terrain.GetComponent<Terrain>();
        float terrainHeight = terrainComponent.SampleHeight(new Vector3(playerPosition.x, playerPosition.z));
        Vector3 respawnPosition = new Vector3(playerPosition.x, terrainHeight + 2.0f, playerPosition.z);

        // Move the golf ball to the new position
        golfBall.transform.position = respawnPosition;

        // Reset the hasCollided flag in the GolfBallCollision script
        golfBallCollisionScript.ResetCollisionFlag();

        Debug.Log("Golf ball respawned at position: " + respawnPosition);
    }

    public Vector3 GetClubVelocity()
    {
        return golfClubRigidbody.velocity;
    }

    public Vector3 GetManualClubVelocity()
    {
        return this.clubVelocity;
    }
}
