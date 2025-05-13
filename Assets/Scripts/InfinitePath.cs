using System.Collections;
using UnityEngine;

public class InfinitePath : MonoBehaviour
{
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int playerCount = 2;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float spawnInterval = 7f;

    private bool gameRunning = true;
    private int pathCount = 0;

    void Start()
    {
        SpawnPlayers();
        StartCoroutine(SpawnPathCoroutine());
    }

    IEnumerator SpawnPathCoroutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (gameRunning)
        {
            SpawnPath();
            pathCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPath()
    {
        Vector3 spawnPosition = new Vector3(0, 0, pathCount * 70f);
        Quaternion spawnRotation = Quaternion.Euler(0, -90, 0);
        Instantiate(pathPrefab, spawnPosition, spawnRotation);
    }

    void SpawnPlayers()
    {
        int clampedPlayerCount = Mathf.Clamp(playerCount, 2, 4); // Ensure player count is between 2 and 4
        float spacing = 2.0f; // Distance between players

        for (int i = 0; i < clampedPlayerCount; i++)
        {
            Vector3 spawnPosition = new Vector3(i * spacing - (clampedPlayerCount - 1) * spacing / 2, 0f, -95);
            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            player.name = "Player" + (i + 1);
            player.transform.localScale = new Vector3(2, 2, 2);

            // Add necessary components
            UpDownPlayerMovement movement = player.AddComponent<UpDownPlayerMovement>();
            PlayerAnimator animator = player.AddComponent<PlayerAnimator>();
            CapsuleCollider capsuleCollider = player.AddComponent<CapsuleCollider>();
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.mass = 1000000f;

            // Configure Rigidbody settings
            rb.useGravity = false;  // Enable gravity so the player falls naturally
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevents unwanted rotation

            // Configure CapsuleCollider settings
            capsuleCollider.height = 2f;  // Adjust height as needed
            capsuleCollider.radius = 0.5f; // Adjust radius as needed
            capsuleCollider.center = new Vector3(0, 1f, 0); // Center it appropriately

            // Assign controls
            movement.AssignControls(i);


        }
    }
}
