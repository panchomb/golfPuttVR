using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ovrCameraRigInteractions;
    public GameObject terrain;
    public GameObject golfClub;
    public GameObject golfBallPrefab;
    public Transform clubHead; // Reference to the ClubHead empty GameObject
    public GameObject gameCanvas;
    public GameObject flagPrefab;

    private Rigidbody golfBallRigidbody;
    private GolfBallCollision golfBallCollisionScript;
    private Vector3 currentClubHeadPosition;
    private Vector3 previousClubHeadPosition;
    private Vector3 clubHeadVelocity;
    private Vector3 golfBallSpawnPosition;
    private bool hasHitBall;

    private GameObject golfBallInstance;

    private int score = 0;
    private Vector3 flagOffset = new Vector3((35.0f - 26.078f), (4.95f - 5.52f) - 0.1f, (42.0f - 40.71f));


    // Start is called before the first frame update
    void Start()
    {
        Terrain terrainComponent = terrain.GetComponent<Terrain>();
        Vector3 terrainCenter = terrainComponent.terrainData.bounds.center;
        Vector3 worldCenterPosition = terrain.transform.position + terrainCenter;

        float height = terrainComponent.SampleHeight(new Vector3(worldCenterPosition.x, worldCenterPosition.y, worldCenterPosition.z));
        Debug.Log("[DEBUG] height is " + height);

        ovrCameraRigInteractions.transform.position = new Vector3(worldCenterPosition.x, height - 0.1f, worldCenterPosition.z);
        Vector3 holePosition = FindHolePosition();

        Vector3 holeMinusPlayer = holePosition - ovrCameraRigInteractions.transform.position;
        Vector3 holeDirection = holeMinusPlayer / holeMinusPlayer.magnitude;

        Quaternion rotation = Quaternion.LookRotation(holeDirection);
        Quaternion additionalRotation = Quaternion.Euler(0, 90, 0);
        Quaternion combinedRotation = rotation * additionalRotation;

        // Set x and z components to 0
        combinedRotation.x = 0;
        combinedRotation.z = 0;

        // Normalize the quaternion to ensure it's a valid rotation
        combinedRotation = Quaternion.Normalize(combinedRotation);

        ovrCameraRigInteractions.transform.rotation = combinedRotation;
        Vector3 newPlayerForward = ovrCameraRigInteractions.transform.forward;
        float distanceFromBall = 0.5f;
        ovrCameraRigInteractions.transform.position -= newPlayerForward * distanceFromBall;


        golfClub.transform.position = new Vector3(ovrCameraRigInteractions.transform.position.x, height + 0.5f, ovrCameraRigInteractions.transform.position.z);
        Rigidbody golfClubRB = golfClub.GetComponent<Rigidbody>();
        // golfClubRB.isKinematic = true;
        //golfBall.transform.position = new Vector3(worldCenterPosition.x, height + 1, worldCenterPosition.z);
        //golfBallRigidbody = golfBall.GetComponent<Rigidbody>();
        //initialGolfBallPosition = golfBall.transform.position;

        // Instantiate the golf ball as a new object
        golfBallSpawnPosition = new Vector3(worldCenterPosition.x, height + 0.025f, worldCenterPosition.z);
        golfBallInstance = Instantiate(golfBallPrefab, golfBallSpawnPosition, Quaternion.identity);
        golfBallRigidbody = golfBallInstance.GetComponent<Rigidbody>();

        // Get the GolfBallCollision script
        golfBallCollisionScript = golfBallInstance.GetComponent<GolfBallCollision>();

        hasHitBall = false;

        // Position the Canvas in front of the player
        PositionCanvasInFrontOfPlayer();

        Debug.Log("Start method executed. Golf ball and club positions set.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RespawnGolfBall();
        }
    }

    public GameObject GetGolfBallInstance()
    {
        return golfBallInstance;
    }
    void FixedUpdate()
    {
        // Manual velocity calculation for the golf club head
        currentClubHeadPosition = clubHead.position;
        clubHeadVelocity = (currentClubHeadPosition - previousClubHeadPosition) / Time.fixedDeltaTime;
        previousClubHeadPosition = currentClubHeadPosition;
    }

    public GameObject RespawnGolfBall()
    {
        // Instantiate a new golf ball at the respawn position
        golfBallInstance = Instantiate(golfBallPrefab, golfBallSpawnPosition, Quaternion.identity);
        golfBallRigidbody = golfBallInstance.GetComponent<Rigidbody>();

        // Get the GolfBallCollision script for the new instance
        golfBallCollisionScript = golfBallInstance.GetComponent<GolfBallCollision>();

        Debug.Log("Golf ball respawned at position: " + golfBallSpawnPosition);
        return golfBallInstance;
    }

    public void BallInHole()
    {
        this.score += 1;
    }

    public Vector3 GetManualClubHeadVelocity()
    {
        return this.clubHeadVelocity;
    }

    public Vector3 GetSpawnBallPosition()
    {
        return golfBallSpawnPosition;
    }
    private void PositionCanvasInFrontOfPlayer()
    {
        if (gameCanvas != null)
        {
            Vector3 playerPosition = ovrCameraRigInteractions.transform.position;
            Vector3 playerForward = ovrCameraRigInteractions.transform.forward;
            float distanceFromPlayer = 3.0f; // Distance in front of the player
            float height = 1.0f;

            gameCanvas.transform.position = playerPosition + playerForward * distanceFromPlayer;
            gameCanvas.transform.position += new Vector3(0, height, 0); 
            gameCanvas.transform.rotation = Quaternion.LookRotation(playerForward);
        }
        else
        {
            Debug.LogWarning("Game Canvas is not assigned in the PlayerController script.");
        }
    }
    private Vector3 FindHolePosition()
    {
        if (flagPrefab != null)
        {
            return flagPrefab.transform.position;
        }
        else
        {
            Debug.LogWarning("Flag prefab is not assigned or not instantiated.");
            return Vector3.zero;
        }
    }
}
