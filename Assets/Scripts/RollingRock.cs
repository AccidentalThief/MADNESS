using UnityEngine;

public class RollingRock : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 5f; // Starting speed of the ball
    [SerializeField] private float rotationSpeed = 200f; // How fast the ball rotates
    [SerializeField] private Vector3 direction = Vector3.forward; // Direction of movement
    private float currentSpeed;
    private float elapsedTime = 0f;

    void Start()
    {
        // Set the initial speed
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        // Update elapsed time
        elapsedTime += Time.deltaTime;

        // Move the ball in the specified direction with exponential speed
        transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

        // Rotate the ball around its local X-axis to simulate rolling
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }

    // This function will be called when the ball collides with the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Logic to handle player losing
            Destroy(collision.gameObject); // Destroy the player object
            Debug.Log("Player hit the ball and lost!");
            // You can also call a Game Over method here if you want to handle game over logic.
        }
    }
}
