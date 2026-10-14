using UnityEngine;

public class CandySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The prefab to spawn (e.g. Candy).")]
    public GameObject prefab;

    [Tooltip("Optional second prefab that can spawn instead of the main one.")]
    public GameObject secondPrefab;

    [Tooltip("Chance (0 to 1) that the second prefab spawns instead of the main one. 0.2 = 20%.")]
    [Range(0f, 1f)]
    public float secondPrefabChance = 0.2f;

    [Tooltip("How fast spawning happens. Higher = faster (spawns per second).")]
    public float spawnSpeed = 1f;

    [Header("Difficulty")]
    [Tooltip("How much spawnSpeed increases per score point. 0.01 = +1 spawn/sec every 100 points.")]
    public float speedIncreasePerPoint = 0.01f;

    [Tooltip("The spawn speed will never go above this.")]
    public float maxSpawnSpeed = 5f;

    private float timer = 0f;
    private int currentScore = 0;

    void OnEnable()  { GameManager.OnScoreChanged += OnScoreChanged; }
    void OnDisable() { GameManager.OnScoreChanged -= OnScoreChanged; }

    private void OnScoreChanged(int newScore)
    {
        currentScore = newScore;
    }

    // Base speed + bonus from the score, capped at the max
    private float CurrentSpawnSpeed =>
        Mathf.Min(spawnSpeed + currentScore * speedIncreasePerPoint, maxSpawnSpeed);

    void Update()
    {
        if (prefab == null || transform.childCount == 0 || spawnSpeed <= 0f) return;

        timer += Time.deltaTime;
        float interval = 1f / CurrentSpawnSpeed; // speed 2 => one spawn every 0.5s

        if (timer >= interval)
        {
            timer -= interval;
            Spawn();
        }
    }

    private void Spawn()
    {
        // Pick one of the child objects at random
        Transform spawnPoint = transform.GetChild(Random.Range(0, transform.childCount));

        // Random roll: sometimes spawn the second prefab instead of the main one
        GameObject toSpawn = prefab;
        if (secondPrefab != null && Random.value < secondPrefabChance)
        {
            toSpawn = secondPrefab;
        }

        Instantiate(toSpawn, spawnPoint.position, Quaternion.identity);
    }
}
