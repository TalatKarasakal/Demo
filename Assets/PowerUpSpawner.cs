using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Prefabları")]
    public GameObject[] powerUpPrefabs;

    [Header("Spawn Ayarları")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 12f;
    
    // Topun uçtuğu sınırlar (Topun çıkabildiği alanın biraz içi olsun ki alınabilsin)
    public float spawnAreaWidth = 6f;
    public float spawnAreaHeight = 3f;

    private SimpleGameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<SimpleGameManager>();

        // Arcade mod aktifse periyodik olarak Power-Up yaratmaya başla
        if (GameSettings.IsArcadeMode)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Oyun oynanıyorsa ve Arcade moddaysak spawnla
            if (gameManager != null && gameManager.IsGameActive())
            {
                SpawnRandomPowerUp();
            }
        }
    }

    void SpawnRandomPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        // Rastgele bir konum belirle (sahanın ortalarında bir yer)
        float randomX = Random.Range(-spawnAreaWidth, spawnAreaWidth);
        float randomY = Random.Range(-spawnAreaHeight, spawnAreaHeight);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        // Rastgele bir prefab seç
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        GameObject selectedPrefab = powerUpPrefabs[randomIndex];

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}