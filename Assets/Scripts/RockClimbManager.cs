using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro; // Add this if not already present

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject goodRockPrefab;
    [SerializeField] private GameObject badRockPrefab;
    [SerializeField] private int numberOfLayers = 10;
    [SerializeField] private float startX = -15f;
    [SerializeField] private float playerXSpacing = 10f;
    [SerializeField] private float spacingY = 5f;
    [SerializeField] public int numberOfPlayers;
    public TMP_Text winnerText;
    public GameObject backgroundPanel;

    private List<List<Transform>> allRockLayers; // List of layers, each containing a list of rocks for each player
    private List<List<bool>> goodRockSides;     // Keeps track of which side (left/right) is good for each layer per player
    private List<Climber> climbers;

    private KeyCode[][] playerControls = new KeyCode[][]
    {
        new KeyCode[] { KeyCode.Q, KeyCode.W},
        new KeyCode[] { KeyCode.X, KeyCode.C},
        new KeyCode[] { KeyCode.Y, KeyCode.U},
        new KeyCode[] { KeyCode.N, KeyCode.M}
    };

    void Awake()
    {
        numberOfPlayers = (Settings.Instance != null) ? Settings.Instance.playerNumber : 4;
        if (goodRockPrefab == null)
        {
            goodRockPrefab = Resources.Load<GameObject>("Prefabs/GoodRockPrefab"); // Replace with your prefab name
            Debug.LogWarning("Good Rock Prefab not assigned. Ensure you have a prefab named 'GoodRockPrefab' in your Resources folder.");
        }
        if (badRockPrefab == null)
        {
            badRockPrefab = Resources.Load<GameObject>("Prefabs/BadRockPrefab");   // Replace with your prefab name
            Debug.LogWarning("Bad Rock Prefab not assigned. Ensure you have a prefab named 'BadRockPrefab' in your Resources folder.");
        }
    }

    void Start()
    {
        numberOfPlayers = (Settings.Instance != null) ? Settings.Instance.playerNumber : 4;
        if (winnerText != null) winnerText.gameObject.SetActive(true);
        if (backgroundPanel != null) backgroundPanel.SetActive(true);
        StartCoroutine(GameIntroAndStart());
    }

    private IEnumerator GameIntroAndStart()
    {
        yield return new WaitForSeconds(11f);
        if (winnerText != null) winnerText.gameObject.SetActive(false);
        if (backgroundPanel != null) backgroundPanel.SetActive(false);
        if (goodRockPrefab == null || badRockPrefab == null) yield break;
        GenerateRockLayers();
        InitializePlayers();
        SetupCameras();
    }

    private void GenerateRockLayers()
    {
        allRockLayers = new List<List<Transform>>();
        goodRockSides = new List<List<bool>>();

        for (int i = 0; i < numberOfLayers; i++)
        {
            List<Transform> layerRocks = new List<Transform>();
            List<bool> layerGoodSides = new List<bool>();
            float posY = i * spacingY;

            for (int j = 0; j < numberOfPlayers; j++)
            {
                bool isLeftGood = Random.value > 0.5f;

                // Positions before adjusting with raycast
                Vector3 leftPos = new Vector3(startX + (j * playerXSpacing) - 1, posY, 18);
                Vector3 rightPos = new Vector3(startX + (j * playerXSpacing) + 1, posY, 18);

                // Adjust positions with raycasting
                leftPos = AdjustPositionWithRaycast(leftPos);
                rightPos = AdjustPositionWithRaycast(rightPos);

                GameObject leftRockPrefab = isLeftGood ? goodRockPrefab : badRockPrefab;
                GameObject leftRockInstance = Instantiate(leftRockPrefab, leftPos, Quaternion.identity);
                layerRocks.Add(leftRockInstance.transform);

                GameObject rightRockPrefab = isLeftGood ? badRockPrefab : goodRockPrefab;
                GameObject rightRockInstance = Instantiate(rightRockPrefab, rightPos, Quaternion.identity);
                layerRocks.Add(rightRockInstance.transform);

                layerGoodSides.Add(isLeftGood);
            }
            allRockLayers.Add(layerRocks);
            goodRockSides.Add(layerGoodSides);
        }
    }

    private Vector3 AdjustPositionWithRaycast(Vector3 originalPosition)
    {
        RaycastHit hit;
        Vector3 rayOrigin = originalPosition + Vector3.back * 2f;
        Vector3 rayDirection = Vector3.forward;

        Debug.DrawRay(rayOrigin, rayDirection * 5f, Color.green, 2f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 5f))
        {
            originalPosition = hit.point;
            originalPosition += hit.normal * 0.5f;
            return originalPosition;
        }
        else
        {
            Debug.LogWarning("Raycast failed to adjust rock position.");
            return originalPosition;
        }
    }

    private void InitializePlayers()
    {
        climbers = new List<Climber>();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject player = GameObject.Find("Player" + (i + 1));
            Climber climberComponent = player.GetComponent<Climber>();
            if (climberComponent != null)
            {
                climbers.Add(climberComponent);

                // Extract the rock holds for this specific player
                List<Transform> playerRocks = new List<Transform>();
                for (int layerIndex = 0; layerIndex < numberOfLayers; layerIndex++)
                {
                    playerRocks.Add(allRockLayers[layerIndex][i * 2]);     // Left rock for this player on this layer
                    playerRocks.Add(allRockLayers[layerIndex][i * 2 + 1]); // Right rock for this player on this layer
                }

                climberComponent.SetGoodRockSides(goodRockSides);
                climberComponent.Initialize(playerControls[i][0], playerControls[i][1], playerRocks.ToArray(), this, i);
                 // Pass the good rock information
            }
            else
            {
                Debug.LogError("Player" + (i + 1) + " does not have a Climber script attached!");
            }
        }
    }

    private void SetupCameras()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject player = GameObject.Find("Player" + (i + 1));
            Transform target = player.transform.Find("Capsule") ?? player.transform;

            GameObject camObj = new GameObject("PlayerCamera" + (i + 1));
            Camera cam = camObj.AddComponent<Camera>();
            camObj.transform.SetParent(null);

            CameraFollow followScript = camObj.AddComponent<CameraFollow>();
            followScript.Initialize(target);

            if (numberOfPlayers == 2)
            {
                cam.fieldOfView =80f; // Wider FOV for 2 players
            }
            else if (numberOfPlayers == 3)
            {
                cam.fieldOfView = 100f; // Slightly wider FOV for 3 players
            }
            else
            {
                cam.fieldOfView = 40f; // Default FOV for other numbers (including 4)
            }
            switch (numberOfPlayers)
            {
                case 2:
                    cam.rect = new Rect(i * 0.5f, 0f, 0.5f, 1f); // Vertical split
                    break;
                case 3:
                    cam.rect = new Rect(i * (1f / 3f), 0f, 1f / 3f, 1f); // Vertical split
                    break;
                case 4:
                    int row = i / 2;
                    int col = i % 2;
                    cam.rect = new Rect(col * 0.5f, 1f - (row + 1) * 0.5f, 0.5f, 0.5f); // 2x2 grid
                    break;
                default:
                    cam.rect = new Rect(0f, 0f, 1f, 1f); // Full screen for other numbers
                    break;
            }
        }
    }

   public void PlayerWon(int playerIndex)
{
    Debug.Log("Player " + (playerIndex + 1) + " has won the mini-game!");

    // Calculate placements and points
    int numPlayers = numberOfPlayers;
    int[] playerScores = new int[numPlayers];
    for (int i = 0; i < numPlayers; i++)
        playerScores[i] = (i == playerIndex) ? 1 : 0;

    int[] sortedIndices = new int[numPlayers];
    for (int i = 0; i < numPlayers; i++) sortedIndices[i] = i;
    System.Array.Sort(sortedIndices, (a, b) => playerScores[b].CompareTo(playerScores[a]));

    int[] placePoints = new int[] { 5, 3, 1, 0 };
    int[] playerPlaces = new int[numPlayers];
    int currentPlace = 0;
    for (int i = 0; i < numPlayers; i++)
    {
        if (i > 0 && playerScores[sortedIndices[i]] < playerScores[sortedIndices[i - 1]])
        {
            currentPlace = i;
        }
        playerPlaces[sortedIndices[i]] = currentPlace;
    }
    for (int i = 0; i < numPlayers; i++)
    {
        if (Settings.Instance != null)
        {
            Settings.Instance.playerPlacement[i] = sortedIndices[i];
            int place = playerPlaces[i];
            int points = (place < placePoints.Length) ? placePoints[place] : 0;
            if (playerScores[i] == 0)
                points = 0;
            Settings.Instance.playerPointsToAdd[i] = points;
        }
    }

    // Show winner UI
    if (winnerText != null)
    {
        winnerText.text = $"Player {playerIndex + 1} Wins!";
        winnerText.gameObject.SetActive(true);
    }
    if (backgroundPanel != null) backgroundPanel.SetActive(true);

    // Optionally, start a coroutine to transition to the win screen
    StartCoroutine(EndGameSequence());
}

private IEnumerator EndGameSequence()
{
    yield return new WaitForSeconds(2.5f);
    UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen");
}

    // Public access to the goodRockSides if needed elsewhere
    public List<List<bool>> GetGoodRockSides()
    {
        return goodRockSides;
    }

    // Public access to allRockLayers if needed elsewhere
    public List<List<Transform>> GetAllRockLayers()
    {
        return allRockLayers;
    }
}