using UnityEngine;

public class WinnerCameraController : MonoBehaviour
{
    public Transform target;              // The winner character
    public Animator cameraAnimator;       // The animator component
    public float delayBeforeOrbit = 5f;   // Time after scene starts to begin transition
    public float transitionDuration = 2f; // Smooth transition into orbit
    public float orbitSpeed = 10f;        // Degrees per second
    public float orbitRadius = 3f;
    public float orbitHeight = 2f;

    private float timer = 0f;
    private float blendTimer = 0f;
    private bool isBlending = false;
    private bool isOrbiting = false;

    private Vector3 blendStartPos;
    private Quaternion blendStartRot;
    private Vector3 orbitStartPos;
    private Quaternion orbitStartRot;
    private Vector3 targetStartPos;

    private float angle; // Radians

    void Start()
    {
        targetStartPos = target.position;
        // Calculate initial angle based on current position
        Vector3 offset = transform.position - target.position;
        angle = Mathf.Atan2(offset.z, offset.x); // z and x — make sure it's on the horizontal plane
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Begin blending transition
        if (!isBlending && !isOrbiting && timer >= delayBeforeOrbit)
        {
            if (cameraAnimator != null)
            {
                cameraAnimator.enabled = false;
            }
            isBlending = true;
            blendStartPos = transform.position;
            blendStartRot = transform.rotation;

            orbitStartPos = GetOrbitPosition(angle);
            orbitStartRot = Quaternion.LookRotation((target.position + Vector3.up * 1.5f) - orbitStartPos);
        }

        if (isBlending)
        {
            blendTimer += Time.deltaTime;
            float t = Mathf.Clamp01(blendTimer / transitionDuration);

            transform.position = Vector3.Lerp(blendStartPos, orbitStartPos, t);
            transform.rotation = Quaternion.Slerp(blendStartRot, orbitStartRot, t);

            if (t >= 1f)
            {
                isBlending = false;
                isOrbiting = true;
            }
        }

        if (isOrbiting)
        {
            angle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;

            Vector3 orbitPos = GetOrbitPosition(angle);
            transform.position = orbitPos;
            transform.LookAt(targetStartPos + Vector3.up * 1.5f);
        }
    }

    Vector3 GetOrbitPosition(float a)
    {
        float x = targetStartPos[0] + Mathf.Cos(a) * orbitRadius;
        float z = targetStartPos[2] + Mathf.Sin(a) * orbitRadius;
        float y = targetStartPos[1] + orbitHeight;
        return new Vector3(x, y, z);
    }
}
