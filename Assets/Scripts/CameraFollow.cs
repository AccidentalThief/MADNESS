using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;  // This will be the cape or texture
    private Vector3 offset;

    // Initialize the camera with a target (cape/texture)
    public void Initialize(Transform capeTransform)
    {
        target = capeTransform;  // Assign the cape/texture's transform here
        offset = new Vector3(0, 2, -10);  // Adjust offset as needed
    }

    private void Start()
    {
        if (target != null)
        {
            // Set the initial camera position to the target's position
            transform.position = new Vector3(target.position.x+1, target.position.y + offset.y, target.position.z + offset.z);
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Only follow the y-axis, leave x and z fixed
            transform.position = new Vector3(transform.position.x, target.position.y + offset.y, transform.position.z);
        }
    }
}
