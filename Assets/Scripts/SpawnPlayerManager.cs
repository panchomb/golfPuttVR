using UnityEngine;

public class SpawnPlayerManager : MonoBehaviour
{
    public GameObject player;
    public GameObject uiCanvas;
    public Terrain terrain;
    public float uiDistanceFromPlayer = 2.0f;

    void Start()
    {
        if (player == null || uiCanvas == null || terrain == null)
        {
            Debug.LogError("Player, UI Canvas, or Terrain not assigned in the inspector");
            return;
        }

        // Position the player in the middle of the terrain
        PositionPlayerInCenter();

        // Position the UI in front of the player
        PositionUIInFrontOfPlayer();
    }

    void PositionPlayerInCenter()
    {
        Vector3 playerPosition = new Vector3(0, 0, 0);
        player.transform.position = playerPosition;
    }

    void PositionUIInFrontOfPlayer()
    {
        Vector3 uiPosition = player.transform.position + player.transform.forward * uiDistanceFromPlayer;
        uiCanvas.transform.position = new Vector3(uiPosition.x, player.transform.position.y + 1.0f, uiPosition.z);
    }
}
