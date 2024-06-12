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

    private bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        Terrain terrainComponent = terrain.GetComponent<Terrain>();
        Vector3 terrainCenter = terrainComponent.terrainData.bounds.center;
        Vector3 worldCenterPosition = terrain.transform.position + terrainCenter;

        float height = terrainComponent.SampleHeight(new Vector3(worldCenterPosition.x, worldCenterPosition.z));

        ovrCameraRigInteractions.transform.position = new Vector3(worldCenterPosition.x+0.5f, height + 0.2f, worldCenterPosition.z+0.5f);
        golfClub.transform.position = new Vector3(worldCenterPosition.x, height + 2.5f, worldCenterPosition.z);
        golfBall.transform.position = new Vector3(worldCenterPosition.x, height + 1, worldCenterPosition.z);

        golfBallRigidbody = golfBall.GetComponent<Rigidbody>();
        golfBallRigidbody.isKinematic = false; // Disable physics at start


    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            hit = true;
        }
    }

    void FixedUpdate() 
    {
        if (hit)
        {
            hit = false;
            Vector3 forceVec = new Vector3(0, 0, 30f);
            golfBallRigidbody.AddForce(forceVec, ForceMode.Impulse);
        }
    }

    public void StartGame()
    {
        golfBallRigidbody.isKinematic = false; // Enable physics
        golfBallRigidbody.AddForce(Vector3.zero); // Ensure the ball starts static
    }
}
