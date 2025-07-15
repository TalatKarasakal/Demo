using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed = 10f; // Başlangıç hızı artırıldı
    public float maxSpeed = 20f;
    public float speedIncrement = 1f; // Hız artışı artırıldı
    public float speedIncrementTime = 3f; // Hızlanma süresi kısaltıldı
    public float bounceMultiplier = 1.02f;
    public float wallBounceForce = 0.9f;
    public float minYVelocity = 5f; // Y ekseni minimum hızı artırıldı

    [Header("Boundaries")]
    public float leftWall = -8f;
    public float rightWall = 8f;
    public float topBoundary = 5f;
    public float bottomBoundary = -5f;

    private Rigidbody2D rb;
    private Vector2 lastVel;
    private SimpleGameManager gm;
    private bool gameStarted = false;
    private float currentSpeed;
    private Coroutine speedIncreaseCoroutine;
    private float stuckTimer = 0f;
    private const float STUCK_TIME_LIMIT = 1f; // Donma limiti düşürüldü

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
        Camera cam = Camera.main;
        if (cam != null && cam.orthographic)
        {
            float ballRadius = GetComponent<CircleCollider2D>().radius;
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            leftWall = -halfWidth + ballRadius;
            rightWall = halfWidth - ballRadius;
            topBoundary = halfHeight - ballRadius;
            bottomBoundary = -halfHeight + ballRadius;
        }
        gm = Object.FindAnyObjectByType<SimpleGameManager>();
        if (gm == null)
            Debug.LogError("BallController: SimpleGameManager bulunamadı!", this);

        rb.linearVelocity = Vector2.zero;
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (rb == null || Time.timeScale == 0f) return;

        CheckStuck();
        ControlSpeed();
        lastVel = rb.linearVelocity;
        CheckBoundaries();
    }

    void CheckStuck()
    {
        if (!gameStarted) return;

        float speed = rb.linearVelocity.magnitude;
        if (speed < 1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= STUCK_TIME_LIMIT)
            {
                Vector2 randomDir = new Vector2(Random.Range(-0.5f, 0.5f), Random.value < 0.5f ? -1f : 1f).normalized;
                rb.linearVelocity = randomDir * currentSpeed;
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
        float speed = rb.linearVelocity.magnitude;

        if (speed > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else if (speed < currentSpeed * 0.8f && gameStarted)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }

        if (gameStarted && Mathf.Abs(rb.linearVelocity.y) < minYVelocity)
        {
            float newY = Mathf.Sign(rb.linearVelocity.y) * minYVelocity;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        currentSpeed = initialSpeed;
        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.value < 0.5f ? -1f : 1f).normalized;
        rb.linearVelocity = dir * currentSpeed;

        if (speedIncreaseCoroutine != null)
            StopCoroutine(speedIncreaseCoroutine);
        speedIncreaseCoroutine = StartCoroutine(IncreaseSpeedOverTime());
    }

    IEnumerator IncreaseSpeedOverTime()
    {
        while (gameStarted && currentSpeed < maxSpeed)
        {
            yield return new WaitForSeconds(speedIncrementTime);
            if (gameStarted)
            {
                currentSpeed = Mathf.Min(currentSpeed + speedIncrement, maxSpeed);
            }
        }
    }

    void CheckBoundaries()
    {
        Vector3 p = transform.position;

        // Yan duvar sekmesi düzeltildi
        if (p.x <= leftWall)
        {
            Vector2 v = rb.linearVelocity;
            v.x = Mathf.Abs(v.x) * wallBounceForce;
            rb.linearVelocity = v;
            p.x = leftWall + 0.1f;
            transform.position = p;
        }
        else if (p.x >= rightWall)
        {
            Vector2 v = rb.linearVelocity;
            v.x = -Mathf.Abs(v.x) * wallBounceForce;
            rb.linearVelocity = v;
            p.x = rightWall - 0.1f;
            transform.position = p;
        }

        if (p.y > topBoundary)
        {
            OnGoal("Player");
        }
        else if (p.y < bottomBoundary)
        {
            OnGoal("AI");
        }
    }

    void OnGoal(string who)
    {
        gameStarted = false;
        rb.linearVelocity = Vector2.zero;

        if (speedIncreaseCoroutine != null)
        {
            StopCoroutine(speedIncreaseCoroutine);
            speedIncreaseCoroutine = null;
        }

        if (gm != null)
        {
            if (who == "Player") gm.OnPlayerScore();
            else gm.OnAIScore();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!gameStarted) return;

        // Sadece paddle çarpışmaları
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("AI"))
        {
            Vector2 incoming = lastVel;  // son velocity
            Vector2 normal = col.contacts[0].normal;
            Vector2 reflect = Vector2.Reflect(incoming, normal) * bounceMultiplier;

            // Paddle hareketinin x etkisi
            Rigidbody2D prb = col.rigidbody;
            if (prb != null)
                reflect.x += prb.linearVelocity.x * 0.5f;

            // Y eksenini mutlaka minYVelocity seviyesine çek
            if (Mathf.Abs(reflect.y) < minYVelocity)
                reflect.y = Mathf.Sign(reflect.y) * minYVelocity;

            // Hızın aşırı düşmemesi için minimum toplam magnitude
            if (reflect.magnitude < currentSpeed * 0.5f)
                reflect = reflect.normalized * (currentSpeed * 0.5f);

            rb.linearVelocity = reflect;
            return;
        }

    }

    public void ResetBall()
    {
        gameStarted = false;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        currentSpeed = initialSpeed;
        stuckTimer = 0f;

        if (speedIncreaseCoroutine != null)
        {
            StopCoroutine(speedIncreaseCoroutine);
            speedIncreaseCoroutine = null;
        }

        Invoke(nameof(StartGame), 1f);
    }

    public void StopBall()
    {
        gameStarted = false;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (speedIncreaseCoroutine != null)
        {
            StopCoroutine(speedIncreaseCoroutine);
            speedIncreaseCoroutine = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(leftWall, -10), new Vector3(leftWall, 10));
        Gizmos.DrawLine(new Vector3(rightWall, -10), new Vector3(rightWall, 10));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10, topBoundary), new Vector3(10, topBoundary));
        Gizmos.DrawLine(new Vector3(-10, bottomBoundary), new Vector3(10, bottomBoundary));
    }
}