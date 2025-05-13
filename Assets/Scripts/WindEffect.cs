using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class WindController : MonoBehaviour
{
    public float minForce = 5f; // Minimum wind force
    public float maxForce = 20f; // Maximum wind force
    public float changeInterval = 2f; // Interval for changing wind direction
    public float maxRotationAngle = 70f; // Maximum allowed rotation before falling
    public float playerRotationForce = 10f; // Force applied by player input
    public Text windUIText; // UI Text to display wind speed
    public Image windIcon; // UI Image for wind direction
    public Text fallMessageText; // UI Text to display fall message
    public Text winMessageText; // UI Text to display game win message
    public int playersInGame;
    public GameObject playerPrefab; // Prefab to spawn for each player

    [System.Serializable]
    public class PlayerObject {
        public Transform transform;
        public Renderer head;
        public Renderer body;
        public Rigidbody rigidbody;
    }

    public List<PlayerObject> players;
    public List<Transform> points;

    public TMP_Text winnerText;
    public GameObject backgroundPanel;
    public Animator animator;

    private float currentForce;
    private float timer;
    private bool gameOver = false;
    private bool condition = true;
    private List<PlayerObject> remainingPlayers;
    private float timer2 = 0f;

    public TMP_Text countdownText; // Assign in inspector
    public float countdownTime = 3f; // 3 seconds countdown

    private float countdownTimer;
    private bool countdownActive = false; // Start as false
    private bool showGo = false;
    private float goTimer = 0f;

    void Start()
    {
        playersInGame = Settings.Instance.playerNumber;
        players = new List<PlayerObject>();
        remainingPlayers = new List<PlayerObject>();
        for (int i = 0; i < playersInGame; i++) {
            // Spawn the player prefab at the corresponding point
            GameObject playerObj = Instantiate(playerPrefab, points[i].position, points[i].rotation);
            playerObj.name = $"Player {i+1}";
            // Find head and body renderers (assumes child objects named "Head" and "Body")
            Renderer headRenderer = playerObj.transform.Find("Head").GetComponent<Renderer>();
            Renderer bodyRenderer = playerObj.transform.Find("Body").GetComponent<Renderer>();
            Rigidbody rb = playerObj.GetComponent<Rigidbody>();
            // Set color
            headRenderer.material = Settings.Instance.playerColors[i];
            bodyRenderer.material = Settings.Instance.playerColors[i];
            // Create PlayerObject and add to lists
            PlayerObject po = new PlayerObject {
                transform = playerObj.transform,
                head = headRenderer,
                body = bodyRenderer,
                rigidbody = rb
            };
            players.Add(po);
            remainingPlayers.Add(po);
        }
        ChangeWindDirection();
        fallMessageText.gameObject.SetActive(false); // Ensure the message is initially hidden
        winMessageText.gameObject.SetActive(false); // Ensure win message is initially hidden

        // Show winnerText and backgroundPanel briefly before countdown
        if (backgroundPanel != null) backgroundPanel.SetActive(true);
        if (winnerText != null) winnerText.gameObject.SetActive(true);
        StartCoroutine(HideWinnerTextBeforeCountdown());
    }

    IEnumerator HideWinnerTextBeforeCountdown() {
        yield return new WaitForSeconds(11f); // Show for 11 seconds
        if (backgroundPanel != null) backgroundPanel.SetActive(false);
        if (winnerText != null) winnerText.gameObject.SetActive(false);
        // Now start the countdown
        countdownTimer = countdownTime;
        countdownActive = true;
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (countdownActive)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer > 0f) {
                if (countdownText != null)
                    countdownText.text = Mathf.Ceil(countdownTimer).ToString();
            } else {
                // Show 'GO!' for 1 second
                if (!showGo) {
                    showGo = true;
                    goTimer = 1f;
                    if (countdownText != null)
                        countdownText.text = "GO!";
                } else {
                    goTimer -= Time.deltaTime;
                    if (goTimer <= 0f) {
                        countdownActive = false;
                        showGo = false;
                        if (countdownText != null)
                            countdownText.gameObject.SetActive(false);
                        timer2 = 11f; // Allow wind and player movement
                    }
                }
            }
            return; // Skip the rest of Update while counting down
        }
        timer += Time.deltaTime;
        timer2 += Time.deltaTime;
        if (timer2 < 11f) {
            timer = 0f;
        }
        if (timer2 > 11f && condition)
        {
            condition = false;
            backgroundPanel.SetActive(false);
            winnerText.gameObject.SetActive(false);
        }
        if (timer >= changeInterval && !gameOver)
        {
            ChangeWindDirection();
            timer = 0f;
        }
        
        // Apply rotation to each player in the Z direction only
        if (timer2 >= 11f) {
            for (int i = remainingPlayers.Count - 1; i >= 0; i--)
        {
            PlayerObject player = remainingPlayers[i];
            Transform rotationPoint = points[i].transform; // Assuming each player has a child named "point"
            
            if (rotationPoint != null)
            {
                float rotationAmount = currentForce * Time.deltaTime;
                player.transform.RotateAround(rotationPoint.position, Vector3.forward, rotationAmount);
                
                // Player input for rotation
                if (i == 0) HandlePlayerInput(player.transform, rotationPoint, KeyCode.Q, KeyCode.W);
                if (i == 1) HandlePlayerInput(player.transform, rotationPoint, KeyCode.C, KeyCode.V);
                if (i == 2) HandlePlayerInput(player.transform, rotationPoint, KeyCode.Y, KeyCode.U);
                if (i == 3) HandlePlayerInput(player.transform, rotationPoint, KeyCode.M, KeyCode.Comma);
                
                // Check the current rotation in Z relative to the starting point
                float zRotation = player.transform.localEulerAngles.z;
                if (zRotation > 180f) zRotation -= 360f; // Normalize to range -180 to 180

                if (remainingPlayers.Count > 1 && Mathf.Abs(zRotation) > maxRotationAngle)
                {
                    Rigidbody rb = player.rigidbody;
                    if (rb != null)
                    {
                        rb.useGravity = true;
                    }
                    else {
                        Debug.Log("RigidBody Not Found!");
                    }
                }
                
                // Check if player has fallen below -100 Y coordinate
                if (player.transform.position.y < -30f)
                {
                    playersInGame--;
                    Settings.Instance.playerPlacement[playersInGame] = i;
                    remainingPlayers.RemoveAt(i);
                    Destroy(player.transform.gameObject);

                    // Check if only one player remains
                    if (remainingPlayers.Count == 1)
                    {
                        // MAXON LOAD NEXT SCENE HERE
                        winnerText.text = remainingPlayers[0].transform.name + " wins!";
                        backgroundPanel.SetActive(true);
                        winnerText.gameObject.SetActive(true);

                        // Example points for each placement (edit as needed)
                        int[] pointsForPlacement = { 5, 3, 2, 0 }; // 1st, 2nd, 3rd, 4th

                        // Assign points to each player based on their placement
                        for (int place = 0; place < Settings.Instance.playerNumber; place++) {
                            int playerIndex = Settings.Instance.playerPlacement[place];
                            Settings.Instance.playerPointsToAdd[playerIndex] = pointsForPlacement[place];
                        }

                        StartCoroutine(BigWin());
                    }
                }
            }
        }
        }
    }

    IEnumerator BigWin() {
        yield return new WaitForSeconds(2.5f);
        PlayAnimation();
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("WinScreen");
    }

    void PlayAnimation()
    {
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("End");
        }
        else
        {
            Debug.LogError("Animator component not assigned!");
        }
    }

    void HandlePlayerInput(Transform player, Transform rotationPoint, KeyCode leftKey, KeyCode rightKey)
    {
        float targetRotation = 0f;
        if (Input.GetKey(leftKey))
        {
            targetRotation = playerRotationForce;
        }
        else if (Input.GetKey(rightKey))
        {
            targetRotation = -playerRotationForce;
        }

        // Smoothly interpolate the rotation
        float currentRotation = player.localEulerAngles.z;
        if (currentRotation > 180f) currentRotation -= 360f; // Normalize to range -180 to 180
        
        float newRotation = Mathf.Lerp(currentRotation, currentRotation + targetRotation, Time.deltaTime * 5f);
        player.RotateAround(rotationPoint.position, Vector3.forward, newRotation - currentRotation);
    }

    void ChangeWindDirection()
    {
        // Choose a new random force within range
        currentForce = Random.Range(minForce, maxForce) * (Random.value > 0.5f ? 1f : -1f);
        
        // Update UI
        if (windUIText != null)
        {
            windUIText.text = "Wind: " + Mathf.Abs(currentForce).ToString("F1") + " mph";
        }
        
        if (windIcon != null)
        {
            // Rotate the wind icon to indicate direction
            windIcon.rectTransform.rotation = Quaternion.Euler(0, 0, currentForce > 0 ? 0 : 180);
        }
    }
}
