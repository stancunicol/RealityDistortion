using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerWalkCamera : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 3f;
    public float mouseSensitivity = 400f;

    private Animator animator;
    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private int ignoreMouseLookFrames;

    private static float NormalizeSignedAngle(float angleDegrees)
    {
        // Convert 0..360 into -180..180
        if (angleDegrees > 180f)
            angleDegrees -= 360f;
        return angleDegrees;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Cursor locking can produce a one-frame mouse delta spike, which can clamp pitch to +/-90.
        // Ignore a couple of frames to prevent spawning while looking straight up/down.
        ignoreMouseLookFrames = 2;

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>(true);

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("PlayerWalkCamera: No Camera found (assign 'playerCamera' or tag a camera as MainCamera). Disabling.");
            enabled = false;
            return;
        }

        xRotation = NormalizeSignedAngle(playerCamera.transform.localEulerAngles.x);
    }

    void Update()
    {
        // --- Mouse Look ---
        if (ignoreMouseLookFrames > 0)
        {
            ignoreMouseLookFrames--;
        }
        else
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        // --- Movement ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // --- Walking & Running ---
        bool isWalking = move.magnitude > 0.1f;
        bool isRunning = isWalking && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);

        // RunSpeed este dublu față de WalkSpeed
        float currentSpeed = isRunning ? walkSpeed * 2f : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Gravitație ---
        if (!controller.isGrounded)
            velocity.y += -9.81f * Time.deltaTime;
        else if (velocity.y < 0)
            velocity.y = -2f;

        controller.Move(velocity * Time.deltaTime);
    }
}
