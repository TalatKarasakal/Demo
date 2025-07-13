using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed    = 8f;
    public float acceleration = 20f;
    public float deceleration = 15f;
    public float boundaryLeft = -7f;
    public float boundaryRight=  7f;

    [Header("Hit Settings")]
    public float hitForce = 10f;
    public float hitRange = 1f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("PlayerController: Rigidbody2D bulunamadı!", this);
        else
            rb.freezeRotation = true;
    }

    void Update()
    {
        HandleMovement();
        HandleHit();
        ClampPosition();
    }

    void HandleMovement()
    {
        float input = 0f;
        #if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  input = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input = +1f;
        #endif

        float currentX = rb.linearVelocity.x;
        float desiredX = input * moveSpeed;
        float diff     = desiredX - currentX;
        float rate     = (Mathf.Approximately(desiredX, 0f) ? deceleration : acceleration);
        float change   = diff * rate * Time.deltaTime;

        rb.linearVelocity = new Vector2(currentX + change, rb.linearVelocity.y);
    }

    void HandleHit()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TryHit();
    }

    void TryHit()
    {
        var ball = GameObject.FindWithTag("Ball");
        if (ball == null) return;
        if (Vector2.Distance(transform.position, ball.transform.position) > hitRange) return;

        var bRb = ball.GetComponent<Rigidbody2D>();
        if (bRb == null) return;

        Vector2 dir = new Vector2(rb.linearVelocity.x * 0.3f + Random.Range(-0.2f,0.2f), 1f).normalized;
        bRb.linearVelocity = dir * hitForce;
        Debug.Log("Player hit the ball!");
    }

    void ClampPosition()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, boundaryLeft, boundaryRight);
        transform.position = p;
    }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0f, transform.position.y, 0f);
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}