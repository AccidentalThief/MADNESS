using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private UpDownPlayerMovement player; // Reference to the player movement component

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Get the UpDownPlayerMovement component attached to the same GameObject
        player = GetComponent<UpDownPlayerMovement>();

        // If the player component is not found, log a warning
        if (player == null)
        {
            Debug.LogWarning("UpDownPlayerMovement component not found on " + gameObject.name);
        }
    }

    private void Update()
    {
        // Only set IsWalking if player is not null
        if (player != null)
        {
            animator.SetBool("IsWalking", player.IsWalking());
        }
    }
}
