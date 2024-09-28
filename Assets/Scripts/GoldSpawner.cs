using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    public GameObject goldPrefab; // Assign the gold bag prefab
    public Transform spawnPoint; // Where the gold will spawn (the cave's location)
    public float spawnInterval = 5f; // Time between spawns
    public int maxGoldInScene = 5; // Maximum number of gold bags allowed in the scene
    public float shootForce = 5f; // Force with which gold is shot out
    private int currentGoldCount = 0;

    void Start()
    {
        InvokeRepeating(nameof(ShootGold), spawnInterval, spawnInterval);
    }

    void ShootGold()
    {
        if (currentGoldCount < maxGoldInScene)
        {
            
            GameObject gold = Instantiate(goldPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Gold spawned at position: " + spawnPoint.position);

            
            Rigidbody2D rb = gold.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDirection = GetRandomDirection();
                rb.AddForce(shootDirection * shootForce, ForceMode2D.Impulse);
            }

            currentGoldCount++;
        }
    }

    public void GoldCollected()
    {
        
        currentGoldCount--;
    }

    Vector2 GetRandomDirection()
    {
        
        float randomAngle = Random.Range(0f, 100f);
        float y = Mathf.Sin(randomAngle);
        y = -Mathf.Abs(y);
        return new Vector2(Mathf.Cos(randomAngle), y);
    }
}
