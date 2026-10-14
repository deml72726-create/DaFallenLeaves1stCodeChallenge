using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Candy : MonoBehaviour
{
    [Header("Tags")]
    public string playerTag = "Player";
    public string interfaceLimitTag = "InterfaceLimit";

    [Header("Score")]
    public int pointValue = 10;

    // Events other scripts can listen to (the int is this candy's point value)
    public static event System.Action<int> OnCandyCaught;
    public static event System.Action<int> OnCandyMissed;

    private bool handled = false; // prevents calling both functions / calling twice

    // Works if the colliders are solid
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    // Works if one of the colliders is set to "Is Trigger"
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        if (handled) return;

        if (other.CompareTag(playerTag))
        {
            handled = true;
            RewardCatch();
            Destroy(gameObject);
        }
        else if (other.CompareTag(interfaceLimitTag))
        {
            handled = true;
            MissedCatch();
            Destroy(gameObject);
        }
    }

    private void RewardCatch()
    {
        OnCandyCaught?.Invoke(pointValue);
    }

    private void MissedCatch()
    {
        OnCandyMissed?.Invoke(pointValue);
    }
}
