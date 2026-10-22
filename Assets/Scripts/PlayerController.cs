
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private float horizontalMovement;
    private float verticalMovement;

    private Rigidbody rb;
    private Collider playerCollider;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Jumping Settings")]
    public float jumpForce = 5f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    private float lastGroundedTime = -10f;
    private float jumpPressedTime = -10f;

    private bool hasJumped = false;
    private bool jumpRequested = false;

    [Header("Ground Check Settings")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.3f;
    public float groundCheckRadius = 0.2f;
    public float groundedGracePeriod = 0.1f;
    public float castStartHeight = 0.1f;

    [Range(0f, 89f)]
    public float maxGroundSlope = 60f;

    private bool isGrounded;
    private RaycastHit groundHit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();

        if (playerCollider == null)
        {
            Debug.LogError(
                "PlayerController requires a Collider on the player.",
                this
            );
        }
    }

    void Update()
    {
        ReadMovementInput();

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressedTime = Time.time;
        }

        CheckGround();

        if (isGrounded)
        {
            lastGroundedTime = Time.time;

            // Reset the jump lock only after landing.
            hasJumped = false;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();

        bool jumpBuffered =
            Time.time - jumpPressedTime <= jumpBufferTime;

        bool canJump =
            Time.time - lastGroundedTime <= coyoteTime;

        if (jumpBuffered && canJump && !hasJumped)
        {
            jumpRequested = true;
        }

        if (jumpRequested && !hasJumped)
        {
            Vector3 velocity = rb.linearVelocity;

            rb.linearVelocity = new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );

            hasJumped = true;
            jumpRequested = false;
            jumpPressedTime = -10f;
            lastGroundedTime = -10f;
            isGrounded = false;
        }
    }

    private void CheckGround()
    {
        if (playerCollider == null)
        {
            isGrounded = false;
            return;
        }

        Bounds bounds = playerCollider.bounds;

        float radius = Mathf.Max(
            0.02f,
            Mathf.Min(
                groundCheckRadius,
                Mathf.Min(bounds.extents.x, bounds.extents.z) * 0.9f
            )
        );

        Vector3 origin = new Vector3(
            bounds.center.x,
            bounds.min.y + radius + castStartHeight,
            bounds.center.z
        );

        float castDistance =
            groundCheckDistance + castStartHeight;

        bool foundGround = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out groundHit,
            castDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        bool overlappingGround = Physics.CheckSphere(
            new Vector3(
                bounds.center.x,
                bounds.min.y + radius,
                bounds.center.z
            ),
            radius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        bool validGround = false;

        if (foundGround)
        {
            float slopeAngle = Vector3.Angle(
                groundHit.normal,
                Vector3.up
            );

            validGround = slopeAngle <= maxGroundSlope;
        }

        if (overlappingGround && !foundGround)
        {
            validGround = true;
        }

        if (validGround)
        {
            isGrounded = true;
        }
        else
        {
            // Grace period helps with rough terrain, but cannot
            // reset hasJumped by itself.
            isGrounded =
                Time.time - lastGroundedTime <= groundedGracePeriod;
        }
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
            return;

        float x =
            (Keyboard.current.dKey.isPressed ||
             Keyboard.current.rightArrowKey.isPressed ? 1f : 0f)
            -
            (Keyboard.current.aKey.isPressed ||
             Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);

        float z =
            (Keyboard.current.wKey.isPressed ||
             Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
            -
            (Keyboard.current.sKey.isPressed ||
             Keyboard.current.downArrowKey.isPressed ? 1f : 0f);

        horizontalMovement = x;
        verticalMovement = z;
    }

    private void MovePlayer()
    {
        // Normalize input so movement speed is consistent
        // for forward, backward, left, right, and diagonals.
        Vector3 movement = new Vector3(
            horizontalMovement,
            0f,
            verticalMovement
        );

        movement = Vector3.ClampMagnitude(movement, 1f);

        Vector3 velocity = rb.linearVelocity;

        rb.linearVelocity = new Vector3(
            movement.x * moveSpeed,
            velocity.y,
            movement.z * moveSpeed
        );
    }

    public void OnMoveInput()
    {
        ReadMovementInput();
    }

    public void OnJumpInput()
    {
        jumpPressedTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();

        if (col == null)
            return;

        Bounds bounds = col.bounds;

        float radius = Mathf.Max(
            0.02f,
            Mathf.Min(
                groundCheckRadius,
                Mathf.Min(bounds.extents.x, bounds.extents.z) * 0.9f
            )
        );

        Vector3 origin = new Vector3(
            bounds.center.x,
            bounds.min.y + radius + castStartHeight,
            bounds.center.z
        );

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            origin + Vector3.down *
            (groundCheckDistance + castStartHeight),
            radius
        );

        Gizmos.DrawLine(
            origin,
            origin + Vector3.down *
            (groundCheckDistance + castStartHeight)
        );
    }
}