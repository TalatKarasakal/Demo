using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed     = 5f;
    public float maxSpeed         = 15f;
    public float bounceMultiplier = 1.05f;
    public float wallBounceForce  = 0.9f;

    [Header("Boundaries")]
    public float leftWall    = -8f;
    public float rightWall   =  8f;
    public float topBoundary =  5f;
    public float bottomBoundary = -5f;

    Rigidbody2D      rb;
    Vector2          lastVel;
    SimpleGameManager gm;

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
        gm = Object.FindAnyObjectByType<SimpleGameManager>();
        if (gm == null)
            Debug.LogError("BallController: SimpleGameManager bulunamadı!", this);

        rb.linearVelocity = Vector2.zero;
        ResetBall();
    }

    void Update()
    {
        if (rb == null || Time.timeScale == 0f) return;

        float speed = rb.linearVelocity.magnitude;
        if (speed > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        else if (speed < 1f && speed > 0.1f)
            rb.linearVelocity = rb.linearVelocity.normalized * 2f;

        lastVel = rb.linearVelocity;
        CheckBoundaries();
    }

    public void StartGame()
    {
        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f),
                                  Random.value < .5f ? -1f : 1f).normalized;
        rb.linearVelocity = dir * initialSpeed;
    }

    void CheckBoundaries()
    {
        Vector3 p = transform.position;

        // Yan duvar sekmesi
        if (p.x <= leftWall || p.x >= rightWall)
        {
            Vector2 v = lastVel;
            v.x = -v.x * wallBounceForce;
            rb.linearVelocity = v;
            p.x = Mathf.Clamp(p.x, leftWall + .1f, rightWall - .1f);
            transform.position = p;
        }

        // Üst-alt gol kontrolü
        if (p.y > topBoundary)
            OnGoal("Player");
        else if (p.y < bottomBoundary)
            OnGoal("AI");
    }

    void OnGoal(string who)
    {
        rb.linearVelocity = Vector2.zero;
        if (gm != null)
        {
            if (who == "Player") gm.OnPlayerScore();
            else                 gm.OnAIScore();
        }
        Debug.Log($"{who} scored!");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (rb == null || Time.timeScale == 0f) return;

        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("AI"))
        {
            Vector2 refl = Vector2.Reflect(lastVel, col.contacts[0].normal)
                           * bounceMultiplier;

            var prb = col.gameObject.GetComponent<Rigidbody2D>();
            if (prb != null) refl.x += prb.linearVelocity.x * 0.3f;

            if (Mathf.Abs(refl.y) < 2f) 
                refl.y = Mathf.Sign(refl.y) * 2f;

            float ang = Mathf.Atan2(refl.y, refl.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(ang) > 75f && Mathf.Abs(ang) < 105f)
            {
                float cl = Mathf.Sign(ang) * 75f;
                float m = refl.magnitude;
                refl = new Vector2(
                    Mathf.Cos(cl * Mathf.Deg2Rad) * m, 
                    Mathf.Sin(cl * Mathf.Deg2Rad) * m
                );
            }

            rb.linearVelocity = refl;
            Debug.Log("Ball hit " + col.gameObject.name);
        }
    }

    public void ResetBall()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        Invoke(nameof(StartGame), 0.5f);
    }

    public void StopBall()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
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