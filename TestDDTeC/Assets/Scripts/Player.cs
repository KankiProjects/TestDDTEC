using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField, Min(0f)] private float jumpForce = 5f;
    [SerializeField, Min(0f)] private float maxChargeBonus = 8f;
    [SerializeField, Min(0.01f)] private float chargeTime = 1f;
    [SerializeField, Range(0.5f, 1f)] private float squashScaleY = 0.75f;
    [SerializeField, Min(0f)] private float airControl = 8f;
    [SerializeField, Min(0f)] private float maxAirSpeed = 8f;
    [SerializeField] private Transform movementCamera;
    [SerializeField] private Transform visual;

    private Rigidbody body;
    private RigidbodyConstraints originalConstraints;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector3 originalScale;
    private Vector2 moveInput;
    private float chargeTimer;
    private bool isGrounded;
    private bool launchPending;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        originalConstraints = body.constraints;

        if (visual == null && transform.childCount > 0)
        {
            visual = transform.GetChild(0);
        }

        if (visual == null)
        {
            Debug.LogError("Player needs a visual child assigned to Visual.", this);
        }
        else
        {
            originalScale = visual.localScale;
        }

        if (movementCamera == null && Camera.main != null)
        {
            movementCamera = Camera.main.transform;
        }

        if (movementCamera == null)
        {
            Debug.LogError("Player movement needs a camera. Assign Movement Camera or tag the active camera as MainCamera.", this);
        }

        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
        jumpAction?.Dispose();
    }

    private void Update()
    {
        moveInput = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);

        if (isGrounded && jumpAction.IsPressed())
        {
            chargeTimer = Mathf.Min(chargeTimer + Time.deltaTime, chargeTime);
        }
        else if (isGrounded && chargeTimer > 0f && jumpAction.WasReleasedThisFrame())
        {
            launchPending = true;
        }

        UpdateCompression();
    }

    private void FixedUpdate()
    {
        if (launchPending)
        {
            Launch();
            return;
        }

        if (isGrounded || moveInput.sqrMagnitude == 0f || airControl == 0f)
        {
            return;
        }

        Vector3 horizontalVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
        if (horizontalVelocity.magnitude < maxAirSpeed)
        {
            Vector3 direction = GetCameraRelativeDirection();
            body.AddForce(direction * airControl, ForceMode.Acceleration);
        }
    }

    private Vector3 GetCameraRelativeDirection()
    {
        if (movementCamera == null)
        {
            return new Vector3(moveInput.x, 0f, moveInput.y);
        }

        Vector3 forward = Vector3.ProjectOnPlane(movementCamera.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(movementCamera.right, Vector3.up).normalized;
        return right * moveInput.x + forward * moveInput.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrounded || !HasGroundContact(collision))
        {
            return;
        }

        isGrounded = true;
        chargeTimer = 0f;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.constraints = originalConstraints | RigidbodyConstraints.FreezeAll;
    }

    private void Launch()
    {
        float chargeRatio = chargeTimer / chargeTime;
        chargeTimer = 0f;
        launchPending = false;
        isGrounded = false;
        body.constraints = originalConstraints;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.AddForce(Vector3.up * (jumpForce + maxChargeBonus * chargeRatio), ForceMode.Impulse);
    }

    private bool HasGroundContact(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateCompression()
    {
        if (visual == null)
        {
            return;
        }

        float chargeRatio = Mathf.Clamp01(chargeTimer / chargeTime);
        Vector3 compressedScale = originalScale;
        compressedScale.y *= Mathf.Lerp(1f, squashScaleY, chargeRatio);
        visual.localScale = compressedScale;
    }
}
