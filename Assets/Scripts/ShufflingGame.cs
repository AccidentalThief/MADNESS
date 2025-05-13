using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ShufflingGame : MonoBehaviour
{
    public Transform[] cups; // The 3 cups
    private int numberOfPlayers = (Settings.Instance != null) ? Settings.Instance.playerNumber : 4;
    public GameObject ball; // The ball
    public int numRounds = 5; // Total rounds
    private int[] playerScores = new int[4]; // Scores for each player
    private int currentRound = 1;
    private Transform[] cupPositions = new Transform[3]; // Tracks which cup is in each position
    private int ballPosition; // Index of the cup with the ball
    private int[] playerGuesses = new int[4]; // Store each player's guess
    public float shuffleSpeed = 1.5f; // Speed of cup movement
    public int shuffleCount = 10; // Number of swaps
    public float revealHeight = 5f; // Height the cups will move up to reveal the ball

    // UI and animation references
    public TMP_Text winnerText;
    public GameObject backgroundPanel;
    public Animator animator;
    public Text infoText; // Legacy UI Text for info output

    private float startDelay = 11f;
    private bool gameStarted = false;
    private bool waitingForGuesses = false;

    // Key mappings for players (corresponding to fixed positions: Left, Middle, Right)
    private KeyCode[,] playerKeys = new KeyCode[4, 3]
    {
        { KeyCode.A, KeyCode.S, KeyCode.D }, // Player 1
        { KeyCode.V, KeyCode.B, KeyCode.N }, // Player 2
        { KeyCode.I, KeyCode.O, KeyCode.P }, // Player 3
        { KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow } // Player 4
    };

    void Start()
    {
        if (cups == null || cups.Length < 3 || ball == null)
        {
            if (infoText != null) infoText.text += "\nCups or Ball not assigned in the Inspector!";
            return;
        }

        if (backgroundPanel != null) backgroundPanel.SetActive(true);
        if (winnerText != null) winnerText.gameObject.SetActive(true);
        StartCoroutine(GameIntroAndStart());
    }

    IEnumerator GameIntroAndStart()
    {
        yield return new WaitForSeconds(startDelay);
        if (backgroundPanel != null) backgroundPanel.SetActive(false);
        if (winnerText != null) winnerText.gameObject.SetActive(false);
        gameStarted = true;
        // Initialize cup positions
        for (int i = 0; i < cups.Length; i++)
            cupPositions[i] = cups[i];
        StartCoroutine(StartNewRound());
    }

    void RevealCupWithBall()
    {
        if (infoText != null) infoText.text += $"\nThe ball was under Cup {ballPosition + 1}";
        StartCoroutine(MoveCupUpAndDown(cupPositions[ballPosition])); // Reveal based on position
    }

    IEnumerator MoveCupUpAndDown(Transform cup)
    {
        Vector3 originalPosition = cup.position;
        Vector3 targetPosition = originalPosition + Vector3.up * revealHeight;

        // Unparent the ball before the cup moves up
        ball.transform.SetParent(null);

        // Move cup up
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            cup.position = Vector3.Lerp(originalPosition, targetPosition, time);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Move cup back down
        time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            cup.position = Vector3.Lerp(targetPosition, originalPosition, time);
            yield return null;
        }

        cup.position = originalPosition;
    }

    IEnumerator StartNewRound()
    {
        // Assign ball to a random cup
        ballPosition = Random.Range(0, cupPositions.Length);
        Transform ballCup = cupPositions[ballPosition];
        ball.transform.position = cupPositions[ballPosition].position + Vector3.up * 0.0001f;
        ball.transform.SetParent(cupPositions[ballPosition]);

        if (infoText != null) infoText.text = $"\n[Round {currentRound}] Ball starts under Cup {ballPosition + 1}";

        // Move the ball into position under the chosen cup
        yield return StartCoroutine(MoveCupUpAndDown(cupPositions[ballPosition]));

        // Make the ball invisible after showing it
        ball.SetActive(false); // Hides the ball by deactivating it

        // Now, unparent the ball before shuffling to keep it in place
        ball.transform.SetParent(null);

        yield return StartCoroutine(ShuffleCups());

        if (infoText != null) infoText.text = "\nShuffling complete! Players, make your guesses.";

        for (int i = 0; i < 4; i++) playerGuesses[i] = -1;

        waitingForGuesses = true;
        yield return new WaitForSeconds(1f);
        while (true)
        {
            int playersGuessed = 0;
            for (int i = 0; i < numberOfPlayers; i++)
                if (playerGuesses[i] != -1) playersGuessed++;
            if (playersGuessed >= numberOfPlayers)
                break;
            yield return null;
        }
        waitingForGuesses = false;
        yield return new WaitForSeconds(3f);

        // Now that the shuffle is complete, teleport the ball to the correct cup
        for (int i = 0; i < cupPositions.Length; i++)
        {
            if (ballCup == cupPositions[i])
            {
                ball.transform.position = cupPositions[i].position + Vector3.up * 0.2f;
                ballPosition = i;
            }
        }

        // Make the ball visible again
        ball.SetActive(true); // Re-enable the ball to make it visible

        RevealCupWithBall();

        infoText.text = "";
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerGuesses[i] == ballPosition)
            {
                playerScores[i]++;
                if (infoText != null) infoText.text += $"\nPlayer {i + 1} gets a point!";
            }
        }

        yield return new WaitForSeconds(4f);
        NextRound();
    }

    IEnumerator ShuffleCups()
    {
        if (infoText != null) infoText.text = "\nShuffling cups...";

        for (int i = 0; i < shuffleCount; i++)
        {
            // Randomly pick two different cups to swap
            int cupA = Random.Range(0, cups.Length);
            int cupB = Random.Range(0, cups.Length);

            // Ensure that the two chosen cups are not the same
            while (cupA == cupB)
            {
                cupB = Random.Range(0, cups.Length);
            }

            // Swap the cups using the SwapCups coroutine
            yield return StartCoroutine(SwapCups(cupPositions[cupA], cupPositions[cupB]));

            // Update the cupPositions array to reflect the swapped positions
            Transform temp = cupPositions[cupA];
            cupPositions[cupA] = cupPositions[cupB];
            cupPositions[cupB] = temp;

            // Optionally wait a small amount of time between swaps
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator SwapCups(Transform cup1, Transform cup2)
    {
        Vector3 center = (cup1.position + cup2.position) / 2f; // Find midpoint
        float radius = Vector3.Distance(cup1.position, center); // Calculate radius
        float angle = 0;
        float speed = shuffleSpeed * Mathf.PI; // Control rotation speed

        Vector3 cup1StartPos = cup1.position;
        Vector3 cup2StartPos = cup2.position;

        bool ballUnderCup1 = ball.transform.parent == cup1;
        bool ballUnderCup2 = ball.transform.parent == cup2;

        while (angle < Mathf.PI)
        {
            angle += Time.deltaTime * speed;

            float x1 = center.x + radius * Mathf.Cos(angle); // Cup1 rotates clockwise
            float z1 = center.z + radius * Mathf.Sin(angle);

            float x2 = center.x + radius * Mathf.Cos(angle + Mathf.PI); // Cup2 rotates counterclockwise
            float z2 = center.z + radius * Mathf.Sin(angle + Mathf.PI);

            cup1.position = new Vector3(x1, cup1StartPos.y, z1);
            cup2.position = new Vector3(x2, cup2StartPos.y, z2);

            // Move the ball if it's under one of the cups
            if (ballUnderCup1)
                ball.transform.position = cup1.position + Vector3.up * 0.2f;
            else if (ballUnderCup2)
                ball.transform.position = cup2.position + Vector3.up * 0.2f;

            yield return null;
        }

        // Ensure cups are perfectly swapped at the end
        cup1.position = cup2StartPos;
        cup2.position = cup1StartPos;
    }

    void NextRound()
    {
        if (currentRound < numRounds)
        {
            currentRound++;
            StartCoroutine(StartNewRound());
        }
        else
        {
            // Find winner(s)
            int maxScore = playerScores[0];
            int winnerIndex = 0;
            for (int i = 1; i < numberOfPlayers; i++)
            {
                if (playerScores[i] > maxScore)
                {
                    maxScore = playerScores[i];
                    winnerIndex = i;
                }
            }

            // Set placements and points
            int[] sortedIndices = new int[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++) sortedIndices[i] = i;
            System.Array.Sort(sortedIndices, (a, b) => playerScores[b].CompareTo(playerScores[a]));

            // Assign points based on place, handling ties
            int[] placePoints = new int[] { 5, 3, 1, 0 };
            int[] playerPlaces = new int[numberOfPlayers]; // 0=1st, 1=2nd, 2=3rd, 3=4th
            int currentPlace = 0;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (i > 0 && playerScores[sortedIndices[i]] < playerScores[sortedIndices[i - 1]])
                {
                    currentPlace = i;
                }
                playerPlaces[sortedIndices[i]] = currentPlace;
            }
            for (int i = 0; i < numberOfPlayers; i++)
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
                winnerText.text = $"Player {winnerIndex + 1} Wins!";
                winnerText.gameObject.SetActive(true);
            }
            if (backgroundPanel != null) backgroundPanel.SetActive(true);
            StartCoroutine(EndGameSequence());
        }
    }

    IEnumerator EndGameSequence()
    {
        yield return new WaitForSeconds(2.5f);
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("End");
            yield return new WaitForSeconds(0.5f);
        }
        SceneManager.LoadScene("WinScreen");
    }

    void Update()
    {
        if (waitingForGuesses)
        {
            for (int player = 0; player < numberOfPlayers; player++)
            {
                if (playerGuesses[player] == -1)
                {
                    for (int pos = 0; pos < 3; pos++)
                    {
                        if (Input.GetKeyDown(playerKeys[player, pos]))
                        {
                            playerGuesses[player] = pos;
                            if (infoText != null) infoText.text += $"\nPlayer {player + 1} guessed Position {pos + 1}";
                            break;
                        }
                    }
                }
            }
        }
    }
}



