using UnityEngine;
using System.Collections;

public class KABOOM : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Spin spin;
    public int dudes = 50;
    public float explosionForce = 1000f;
    public float explosionRadius = 5f;

    private bool hasStarted = false; // Flag to track if Start has run

    void Start()
    {
        hasStarted = true;
        StartCoroutine(SpawnWithDelay());
    }

    void OnEnable()
    {
        if (!hasStarted) {
            return;
        }
        StartCoroutine(SpawnWithDelay());
    }

    IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(1.8f);
        for (int i = 0; i < dudes; i++)
        {
            SpawnPrefab();
        }
    }

    void SpawnPrefab()
    {
        Vector3 spawnPosition = transform.position;
        Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, randomRotation);

        if (spawnedObject.GetComponent<Collider>() == null)
        {
            spawnedObject.AddComponent<CapsuleCollider>();
        }

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }

        if (spawnedObject.GetComponent<Spin>() == null) 
        {
            spawnedObject.AddComponent<Spin>();
        }

        spawnedObject.gameObject.tag = "Dude";

        RandomizeRenderersExceptEyes(spawnedObject);
        
        // Apply explosive force in a flipped circular pattern (facing the user)
        float angle = Random.Range(0f, 360f);
        Vector3 explosionDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        rb.AddForce(explosionDirection * explosionForce);
    }

    void RandomizeRenderersExceptEyes(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material.name.Contains("Eye") || renderer.gameObject.name.Contains("Eye"))
            {
                renderer.material.color = Color.black;
            }
            else
            {
                renderer.material.color = randomColor;
            }
        }
    }
}