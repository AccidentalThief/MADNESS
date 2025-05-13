using UnityEngine;

public class Spin : MonoBehaviour
{
    public float minSpin = 50f;
    public float maxSpin = 100f;
    private float spin;

    void Start()
    {
        spin = Random.Range(minSpin, maxSpin);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left * spin * Time.deltaTime);
        if (transform.position.y <= -12) {
            Destroy(gameObject);
        }
            
    }
}
