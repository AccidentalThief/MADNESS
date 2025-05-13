using UnityEngine;

public class BallZoneHandler : MonoBehaviour
{
    public Transform aimPointA; // First setpoint
    public Transform aimPointB; // Second setpoint
    public float dropHeight = 1f; // Height to which the ball drops after locking
    public int rows = 6;
    public int cols = 7;
    public GameObject stationaryBallPrefab; // Reference to the stationary ball prefab
    private float[] columnHeights;
    private float zoneWidth;
    private Vector3 gridOrigin;

    void Start()
    {
        columnHeights = new float[cols];
        zoneWidth = (aimPointB.position.x - aimPointA.position.x) / (cols - 1);
        gridOrigin = new Vector3(aimPointA.position.x, aimPointA.position.y, aimPointA.position.z);
    }

    public void HandleBallLanding(GameObject ball)
    {
        int zoneIndex = GetZoneIndex(ball.transform.position.x);
        float targetX = GetZoneCenter(zoneIndex);
        float targetY = gridOrigin.y - columnHeights[zoneIndex] * dropHeight;

        columnHeights[zoneIndex]++;
    }

    int GetZoneIndex(float xPosition)
    {
        float normalizedX = Mathf.Clamp(xPosition, aimPointA.position.x, aimPointB.position.x);
        return Mathf.RoundToInt((normalizedX - aimPointA.position.x) / zoneWidth);
    }

    float GetZoneCenter(int index)
    {
        return aimPointA.position.x + index * zoneWidth;
    }

}
