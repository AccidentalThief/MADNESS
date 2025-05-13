using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float forwardSpeed = 5f;  // Speed at which the camera moves forward

    void Update()
    {
        // Move the camera forward at a constant speed
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
    }
}
