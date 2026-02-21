using UnityEngine;
using System.Collections;

public class GuclendirmeUretici : MonoBehaviour
{
    [Header("Power-Up Prefabları")]
    public GameObject[] GuclendirmePrefabs;

    [Header("Spawn Ayarları")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 12f;

    // Topun uçtuğu sınırlar (Topun çıkabildiği alanın biraz içi olsun ki alınabilsin)
    public float spawnAreaWidth = 6f;
    public float spawnAreaHeight = 3f;

    private OyunYoneticisi gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<OyunYoneticisi>();

        // Arcade mod aktifse periyodik olarak Power-Up yaratmaya başla
        if (OyunAyarlari.IsArcadeMode)
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
                SpawnRandomGuclendirme();
            }
        }
    }

    void SpawnRandomGuclendirme()
    {
        if (GuclendirmePrefabs == null || GuclendirmePrefabs.Length == 0) return;

        // Rastgele bir konum belirle (sahanın ortalarında bir yer)
        float randomX = Random.Range(-spawnAreaWidth, spawnAreaWidth);
        float randomY = Random.Range(-spawnAreaHeight, spawnAreaHeight);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        // Rastgele bir prefab seç
        int randomIndex = Random.Range(0, GuclendirmePrefabs.Length);
        GameObject selectedPrefab = GuclendirmePrefabs[randomIndex];

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}