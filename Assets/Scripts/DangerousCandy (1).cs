using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DangerousCandy : MonoBehaviour
{
    [Header("Tags")]
    public string playerTag = "Player";
    public string interfaceLimitTag = "InterfaceLimit";

    // Event other scripts can listen to
    public static event System.Action OnDangerousCandyCaught;

    private bool handled = false; // prevents calling twice

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
            PunishCatch();
            Destroy(gameObject);
        }
        else if (other.CompareTag(interfaceLimitTag))
        {
            handled = true;
            Destroy(gameObject); // missing a dangerous candy is fine
        }
    }

    private void PunishCatch()
    {
        OnDangerousCandyCaught?.Invoke();
    }
}
