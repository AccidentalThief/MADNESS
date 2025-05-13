using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Climber : MonoBehaviour
{
    public KeyCode leftKey;
    public KeyCode rightKey;
    public Transform[] rockHolds;          // Array of ALL rock holds for this player, layer by layer (left then right)
    public GameManager gameManager;       // Reference to the game manager
    public int playerIndex;              // Player ID

    private int currentLayerIndex = 0;   // Index of the current layer (0-based)
    private bool isClimbing = true;      // Flag to determine if the player is still climbing
    private float moveSpeed = 3f;        // Movement speed for smooth transitions

    private float cooldownTime = 0.2f;   // Cooldown time between moves
    private float timeSinceLastMove = 0f; // Time passed since the last movement

    private List<List<bool>> goodRockSides; // Information about which side is good on each layer

    public void Initialize(KeyCode left, KeyCode right, Transform[] holds, GameManager manager, int index)
    {
        leftKey = left;
        rightKey = right;
        rockHolds = holds;
        gameManager = manager;
        playerIndex = index;
        currentLayerIndex = 0;

        // Initial position on the first good rock of the first layer
        if (goodRockSides != null && goodRockSides.Count > 0 && playerIndex >= 0 && playerIndex < goodRockSides[0].Count && rockHolds != null && rockHolds.Length > 0)
        {
            int firstGoodRockIndex = goodRockSides[0][playerIndex] ? 0 : 1; // 0 for left, 1 for right in the first layer's holds
            if (firstGoodRockIndex < rockHolds.Length)
            {
                transform.position = new Vector3(rockHolds[firstGoodRockIndex].position.x, rockHolds[firstGoodRockIndex].position.y, transform.position.z);
            }
            else
            {
                Debug.LogError($"Climber {playerIndex}: Initial good rock index out of bounds of rockHolds.");
            }
        }
        else
        {
            Debug.LogError($"Climber {playerIndex}: Missing goodRockSides data or invalid playerIndex/rockHolds during initialization.");
        }
    }
    public void SetGoodRockSides(List<List<bool>> sides)
    {
        goodRockSides = sides;
    }
    private void Update()
    {
        if (gameManager == null || playerIndex < 0 || (gameManager.numberOfPlayers > 0 && playerIndex >= gameManager.numberOfPlayers))
        {
            return; // Don't run climbing logic if not a valid player
        }

        if (isClimbing)
        {
            HandleClimbing();
        }

        timeSinceLastMove += Time.deltaTime;
    }

    private void HandleClimbing()
    {
        if (gameManager == null || playerIndex < 0 || (gameManager.numberOfPlayers > 0 && playerIndex >= gameManager.numberOfPlayers) || goodRockSides == null || currentLayerIndex >= goodRockSides.Count)
        {
            return; // Don't run climbing logic if not valid or data is missing
        }

        if (timeSinceLastMove >= cooldownTime)
        {
            if (currentLayerIndex < goodRockSides.Count - 1)
            {
                if (Input.GetKeyDown(leftKey))
                {
                    TryMove(true);
                }
                else if (Input.GetKeyDown(rightKey))
                {
                    TryMove(false);
                }
            }
        }
    }

    private void TryMove(bool isLeftInput)
    {
        if (goodRockSides == null || currentLayerIndex >= goodRockSides.Count - 1 || playerIndex < 0 || playerIndex >= goodRockSides[0].Count)
        {
            return; // Safety check for null or out-of-bounds data
        }

        if (currentLayerIndex < goodRockSides.Count - 1) // Ensure we're not on the top layer
        {
            int nextGoodRockIndexOnLayer = goodRockSides[currentLayerIndex + 1][playerIndex] ? 0 : 1; // Look at the NEXT layer
            int pressedRockIndexOnLayer = isLeftInput ? 0 : 1;

            // If the pressed direction matches the good rock on the NEXT layer
            if (pressedRockIndexOnLayer == nextGoodRockIndexOnLayer)
            {
                // Moved towards the good rock on the next layer, so move up
                currentLayerIndex++;
                int nextRockHoldIndex = (currentLayerIndex * 2) + pressedRockIndexOnLayer; // Calculate the correct index in rockHolds
                if (nextRockHoldIndex < rockHolds.Length)
                {
                    Vector3 targetPos = new Vector3(rockHolds[nextRockHoldIndex].position.x, rockHolds[nextRockHoldIndex].position.y, transform.position.z);
                    StartCoroutine(SmoothMove(targetPos));

                    // Check for win condition
                    if (currentLayerIndex == goodRockSides.Count - 1)
                    {
                        isClimbing = false;
                        gameManager.PlayerWon(playerIndex);
                    }
                }
                else
                {
                    Debug.LogError($"Climber {playerIndex}: nextRockHoldIndex out of bounds of rockHolds during MoveUp.");
                }
            }
            else
            {
                // Pressed the wrong direction (not the good rock on the next layer), so fall
                if (currentLayerIndex > 0)
                {
                    int previousGoodRockIndex = goodRockSides[currentLayerIndex - 1][playerIndex] ? ((currentLayerIndex - 1) * 2) : ((currentLayerIndex - 1) * 2 + 1);
                    if (previousGoodRockIndex < rockHolds.Length)
                    {
                        Vector3 targetPos = new Vector3(rockHolds[previousGoodRockIndex].position.x, rockHolds[previousGoodRockIndex].position.y, transform.position.z);
                        StartCoroutine(SmoothMove(targetPos));
                        currentLayerIndex--;
                    }
                    else
                    {
                        Debug.LogError($"Climber {playerIndex}: previousGoodRockIndex out of bounds of rockHolds during MoveDown.");
                    }
                }
                else
                {
                    // On the first layer and pressed wrong, stay put (or other desired behavior)
                    int currentGoodRockIndex = goodRockSides[0][playerIndex] ? 0 : 1;
                    if (currentGoodRockIndex < rockHolds.Length)
                    {
                        Vector3 targetPos = new Vector3(rockHolds[currentGoodRockIndex].position.x, rockHolds[currentGoodRockIndex].position.y, transform.position.z);
                        StartCoroutine(SmoothMove(targetPos));
                    }
                    else
                    {
                        Debug.LogError($"Climber {playerIndex}: currentGoodRockIndex out of bounds of rockHolds on first layer.");
                    }
                }
            }
        }
        else
        {
            // Already at the top layer, no further upward movement
        }

        // Reset the cooldown timer
        timeSinceLastMove = 0f;
    }

    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < 0.2f)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, (elapsedTime / 0.2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}