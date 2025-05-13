using UnityEngine;

public class DudeSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Spin spin;
    public float spawnAreaWidth = 26f;
    public float spawnRate = 4f;

    private float nextSpawnTime = 0f;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnPrefab();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void SpawnPrefab()
    {
        float randomX = Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);

        Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, randomRotation);

        if (spawnedObject.GetComponent<Collider>() == null)
        {
            spawnedObject.AddComponent<CapsuleCollider>();
        }

        if (spawnedObject.GetComponent<Rigidbody>() == null)
        {
            spawnedObject.AddComponent<Rigidbody>();
        }

        if (spawnedObject.GetComponent<Spin>() == null) 
        {
            spawnedObject.AddComponent<Spin>();
        }

        spawnedObject.gameObject.tag = "Dude";

        // Randomize Color for all renderers except eyes
        RandomizeRenderersExceptEyes(spawnedObject);
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