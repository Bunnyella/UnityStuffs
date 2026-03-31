using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Windows;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 1.5f;
    public float gravityValue = -9.81f;
    public float acceleration = 2f;
    public float maxSpeed = 10.0f;
    public float rotationSpeed = 2.0F;
    public float rotationAngle = 0;

    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference lookAction;

    [SerializeField] Transform cam;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        lookAction.action.Disable();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;
        }

        // Input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float inputX = input.x;
        float inputY = input.y;

        // Camera relative direction
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Movement relative to camera
        Vector3 moveDirection = camForward * inputY + camRight * inputX;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Apply movement
        playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, moveDirection.x * maxSpeed, acceleration * Time.deltaTime);
        playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, moveDirection.z * maxSpeed, acceleration * Time.deltaTime);

        // Jump
        if (groundedPlayer && jumpAction.action.WasPressedThisFrame())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Move
        controller.Move(playerVelocity * Time.deltaTime);

        Vector2 moveVector = lookAction.action.ReadValue<Vector2>();

        rotationAngle = (rotationAngle + (moveVector.x * Time.deltaTime)) % 360;
    }
}