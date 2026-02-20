using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed = 10f;
    public float maxSpeed = 20f;
    public float speedIncrement = 1f;
    public float speedIncrementTime = 3f;
    public float bounceMultiplier = 1.02f;
    public float wallBounceForce = 0.9f;
    public float minYVelocity = 5f;

    [Header("Effects")]
    public GameObject hitEffectPrefab;

    [Header("Boundaries")]
    public float leftWall = -8f;
    public float rightWall = 8f;
    public float topBoundary = 5f;
    public float bottomBoundary = -5f;

    Rigidbody2D rb;
    Vector2 lastVel;
    SimpleGameManager gm;
    bool gameStarted = false;
    float currentSpeed;
    Coroutine speedIncreaseCoroutine;
    float stuckTimer = 0f;
    const float STUCK_TIME_LIMIT = 1f;

    AIController.DifficultyLevel difficulty;

    // YENİ: Topa en son kimin vurduğunu takip edeceğiz (Power-Up'lar için)
    [HideInInspector]
    public string lastHitter = "";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("BallController: Rigidbody2D bulunamadı!", this);
        else
            rb.freezeRotation = true;
    }

    void Start()
    {
        difficulty = GameSettings.SelectedDifficulty;

        Camera cam = Camera.main;
        if (cam != null && cam.orthographic)
        {
            float r = GetComponent<CircleCollider2D>().radius;
            float h = cam.orthographicSize;
            float w = h * cam.aspect;
            leftWall = -w + r;
            rightWall = w - r;
            topBoundary = h - r;
            bottomBoundary = -h + r;
        }

        gm = Object.FindAnyObjectByType<SimpleGameManager>();
        if (gm == null)
            Debug.LogError("BallController: SimpleGameManager bulunamadı!", this);

        rb.linearVelocity = Vector2.zero;
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (!gameStarted || rb == null || Time.timeScale == 0f) return;

        CheckStuck();
        ControlSpeed();
        lastVel = rb.linearVelocity;
        CheckBoundaries();
    }

    public void StartGame()
    {
        gameStarted = true;
        currentSpeed = initialSpeed;
        lastHitter = ""; // Oyun başlarken son vuranı sıfırla

        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f),
                                  Random.value < 0.5f ? -1f : 1f).normalized;
        rb.linearVelocity = dir * currentSpeed;

        if (difficulty != AIController.DifficultyLevel.Easy)
        {
            if (speedIncreaseCoroutine != null) StopCoroutine(speedIncreaseCoroutine);
            speedIncreaseCoroutine = StartCoroutine(IncreaseSpeedOverTime());
        }
    }

    IEnumerator IncreaseSpeedOverTime()
    {
        float multiplier = difficulty == AIController.DifficultyLevel.Medium ? 2f : 3f;

        while (gameStarted && currentSpeed < maxSpeed)
        {
            yield return new WaitForSeconds(speedIncrementTime);
            if (!gameStarted) break;

            currentSpeed = Mathf.Min(currentSpeed + speedIncrement * multiplier, maxSpeed);
        }
    }

    void CheckStuck()
    {
        float v = rb.linearVelocity.magnitude;
        if (v < 1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= STUCK_TIME_LIMIT)
            {
                Vector2 rnd = new Vector2(Random.Range(-0.5f, 0.5f),
                                          Random.value < 0.5f ? -1f : 1f).normalized;
                rb.linearVelocity = rnd * currentSpeed;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    void ControlSpeed()
    {
        float v = rb.linearVelocity.magnitude;
        if (v > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        else if (v < currentSpeed * 0.8f)
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;

        if (Mathf.Abs(rb.linearVelocity.y) < minYVelocity)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                                            Mathf.Sign(rb.linearVelocity.y) * minYVelocity);
    }

    void CheckBoundaries()
    {
        Vector3 p = transform.position;

        if (p.x <= leftWall)
        {
            var vel = rb.linearVelocity;
            vel.x = Mathf.Abs(vel.x) * wallBounceForce;
            rb.linearVelocity = vel;
            p.x = leftWall + 0.1f;
        }
        else if (p.x >= rightWall)
        {
            var vel = rb.linearVelocity;
            vel.x = -Mathf.Abs(vel.x) * wallBounceForce;
            rb.linearVelocity = vel;
            p.x = rightWall - 0.1f;
        }

        transform.position = p;

        if (p.y > topBoundary) OnGoal("Player");
        else if (p.y < bottomBoundary) OnGoal("AI");
    }

    void OnGoal(string who)
    {
        gameStarted = false;
        rb.linearVelocity = Vector2.zero;
        if (speedIncreaseCoroutine != null)
            StopCoroutine(speedIncreaseCoroutine);

        if (gm != null)
        {
            if (who == "Player") gm.OnPlayerScore();
            else gm.OnAIScore();
        }
    }

    public void ResetBall()
    {
        gameStarted = false;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        currentSpeed = initialSpeed;
        stuckTimer = 0f;
        lastHitter = ""; // Raund bittiğinde sıfırla

        if (speedIncreaseCoroutine != null)
            StopCoroutine(speedIncreaseCoroutine);

        Invoke(nameof(StartGame), 1f);
    }

    public void StopBall()
    {
        gameStarted = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (speedIncreaseCoroutine != null)
            StopCoroutine(speedIncreaseCoroutine);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player") || col.collider.CompareTag("AI"))
        {
            // YENİ: Topa kimin vurduğunu kaydet
            lastHitter = col.collider.tag;

            Vector2 currentVel = rb.linearVelocity;
            Vector2 normal = col.contacts[0].normal;
            Vector2 contactPoint = col.contacts[0].point;

            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, contactPoint, Quaternion.identity);
                Destroy(effect, 1f);
            }

            Vector2 reflected = Vector2.Reflect(currentVel, normal) * bounceMultiplier;

            Rigidbody2D paddleRb = col.collider.GetComponent<Rigidbody2D>();
            if (paddleRb != null)
            {
                float spinFactor = 0.25f;
                reflected.x += paddleRb.linearVelocity.x * spinFactor;
            }

            if (Mathf.Abs(reflected.y) < minYVelocity)
                reflected.y = Mathf.Sign(reflected.y) * minYVelocity;

            rb.linearVelocity = reflected;

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeCamera(0.1f, 0.15f);
            }
        }
    }
}