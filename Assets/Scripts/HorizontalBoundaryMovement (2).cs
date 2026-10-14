using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HorizontalBoundaryMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Boundary Settings")]
    [Tooltip("GameObject that defines the allowed horizontal area (needs a Collider2D or a Renderer/SpriteRenderer).")]
    public GameObject boundaryArea;

    [Header("Proximity Animation")]
    [Tooltip("Name of the bool parameter in the Animator.")]
    public string boolName = "IsNear";

    [Tooltip("How close another collider must be to trigger the animation.")]
    public float detectionRadius = 2f;

    [Tooltip("Only colliders on these layers are detected (e.g. a 'Candy' layer).")]
    public LayerMask detectionLayers = ~0; // everything by default

    [Header("Game Over Animation")]
    [Tooltip("Name of the bool parameter in the Animator played when all hearts are lost.")]
    public string gameOverBoolName = "IsDead";

    private bool isDead = false;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Collider2D boundaryCollider;
    private Renderer boundaryRenderer;
    private Animator animator;
    private readonly Collider2D[] results = new Collider2D[10];

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>(); // works if the Animator is on the player or a child sprite

        if (boundaryArea == null)
        {
            Debug.LogWarning("Boundary Area GameObject is not assigned!", this);
            return;
        }

        boundaryCollider = boundaryArea.GetComponent<Collider2D>();
        if (boundaryCollider == null)
            boundaryRenderer = boundaryArea.GetComponent<Renderer>();

        if (boundaryCollider == null && boundaryRenderer == null)
            Debug.LogWarning("Boundary Area needs a Collider2D or a Renderer to define its size!", this);
    }

    void OnEnable()  { GameManager.OnGameOver += HandleGameOver; }
    void OnDisable() { GameManager.OnGameOver -= HandleGameOver; }

    // Called when all hearts are lost
    private void HandleGameOver()
    {
        isDead = true;
        if (animator == null) return;

        // The game freezes with Time.timeScale = 0, so let the Animator ignore it
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetBool(boolName, false);
        animator.SetBool(gameOverBoolName, true);
    }

    void Update()
    {
        UpdateProximityAnimation();
    }

    // Sets the Animator bool to true while another collider is within the detection radius
    private void UpdateProximityAnimation()
    {
        if (animator == null || isDead) return;

        bool isNear = false;

        int count = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRadius, results, detectionLayers);
        for (int i = 0; i < count; i++)
        {
            Transform other = results[i].transform;

            // Ignore the player itself (and its children) and the boundary area
            if (other == transform || other.IsChildOf(transform)) continue;
            if (boundaryArea != null && other == boundaryArea.transform) continue;

            isNear = true;
            break;
        }

        animator.SetBool(boolName, isNear);
    }

    // World-space bounds of the boundary object (already includes its position, scale and size)
    private bool TryGetBoundaryBounds(out Bounds bounds)
    {
        if (boundaryCollider != null) { bounds = boundaryCollider.bounds; return true; }
        if (boundaryRenderer != null) { bounds = boundaryRenderer.bounds; return true; }
        bounds = default;
        return false;
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Vector2 pos = rb.position;

        // No input -> don't touch the transform
        if (Mathf.Approximately(moveInput, 0f)) return;

        float newX = pos.x + moveInput * moveSpeed * Time.fixedDeltaTime;

        if (TryGetBoundaryBounds(out Bounds area))
        {
            // Player's half width (updated every frame in case the scale changes)
            float halfWidth = playerCollider != null ? playerCollider.bounds.extents.x : 0f;

            float minX = area.min.x + halfWidth;
            float maxX = area.max.x - halfWidth;

            // Block movement that would leave the area; stick exactly to the edge instead
            if (newX < minX || newX > maxX)
                newX = Mathf.Clamp(newX, minX, maxX);
        }

        // Only X changes, Y stays the same
        rb.MovePosition(new Vector2(newX, pos.y));
    }

    // Shows the detection radius in the Scene view when the player is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
