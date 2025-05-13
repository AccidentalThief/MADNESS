using UnityEngine;
using UnityEngine.UI;

public class BasketballShooter : MonoBehaviour
{
    public GameObject basketballPrefabPlayer1; // Assign Player 1's basketball prefab
    public GameObject basketballPrefabPlayer2; // Assign Player 2's basketball prefab
    public Transform shootPointPlayer1; // Shoot point for Player 1
    public Transform shootPointPlayer2; // Shoot point for Player 2
    public float shootForce = 10f;
    public GameObject aimIndicator; // Assign an object to show landing position
    public Transform aimPointA; // First position for aim indicator
    public Transform aimPointB; // Second position for aim indicator
    public float gravity = 9.81f;
    public Text turnIndicatorText; // UI element to display current player's turn
    public float indicatorSpeed = 4f;

    private bool movingToB = true;
    private bool aimMoving = true;
    private bool isPlayerOneTurn = true;

    void Start()
    {
        UpdateTurnIndicator();
    }

    void Update()
    {
        if (aimMoving)
        {
            MoveAimIndicator();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootBall();
            isPlayerOneTurn = !isPlayerOneTurn; // Switch turn
            aimMoving = true; // Restart aim indicator movement
            UpdateTurnIndicator();
        }
    }

    void MoveAimIndicator()
    {
        if (aimIndicator == null || aimPointA == null || aimPointB == null) return;

        float step = indicatorSpeed * Time.deltaTime;
        if (movingToB)
        {
            aimIndicator.transform.position = Vector3.MoveTowards(aimIndicator.transform.position, aimPointB.position, step);
            if (Vector3.Distance(aimIndicator.transform.position, aimPointB.position) < 0.1f)
            {
                movingToB = false;
            }
        }
        else
        {
            aimIndicator.transform.position = Vector3.MoveTowards(aimIndicator.transform.position, aimPointA.position, step);
            if (Vector3.Distance(aimIndicator.transform.position, aimPointA.position) < 0.1f)
            {
                movingToB = true;
            }
        }
    }

    void ShootBall()
    {
        Transform shootPoint = isPlayerOneTurn ? shootPointPlayer1 : shootPointPlayer2;
        GameObject basketballPrefab = isPlayerOneTurn ? basketballPrefabPlayer1 : basketballPrefabPlayer2;

        if (shootPoint == null || aimIndicator == null) return;

        GameObject basketball = Instantiate(basketballPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = basketball.GetComponent<Rigidbody>();
        basketball.GetComponent<Renderer>().material = isPlayerOneTurn ? Settings.Instance.playerColors[0] : Settings.Instance.playerColors[1];

        if (rb != null)
        {
            Vector3 displacement = aimIndicator.transform.position - shootPoint.position;
            float timeToTarget = Mathf.Sqrt((2 * displacement.y) / gravity);
            float horizontalSpeed = displacement.magnitude / timeToTarget;

            Vector3 velocity = displacement.normalized * horizontalSpeed;
            velocity.y = Mathf.Sqrt(2 * gravity * displacement.y);

            rb.linearVelocity = velocity;
        }
    }

    void UpdateTurnIndicator()
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = isPlayerOneTurn ? "Player 1's Turn" : "Player 2's Turn";
        }
    }
}
