using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private float verticalVelocity = 0f;
    private float gravityMult = 5f;
    private float jumpForce = 1f;
    PlayerInputActions playerInputActions;
    CharacterController characterController;
    float cameraForward;
    Quaternion controlRotation;
    Vector2 playerScreenPos;
    Stopwatch stopwatch;

    // Start is called before the first frame update
    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        characterController = GetComponent<CharacterController>();
        cameraForward = Camera.main.transform.eulerAngles.y;
        controlRotation = Quaternion.Euler(0, cameraForward, 0);
    }

    private void Start()
    {
        stopwatch = new Stopwatch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
        HandleLooking();
        HandleJumpAndGravity();
    }

    private void HandleJumpAndGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity += gravityMult * Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = 0f;
            }
            if (playerInputActions.Player.Jump.IsPressed())
            {
                verticalVelocity += jumpForce * -Physics.gravity.y;
            }
        }
    }

    private void HandleLooking()
    {
        Vector2 forward = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2).normalized;
        transform.forward = new Vector3(forward.x, transform.forward.y, forward.y);
        transform.Rotate(Vector3.up, -135f);
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GetMovementVectorNormalized();
        Vector3 moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        Vector3 rotatedMoveVector = controlRotation * moveVector;
        Vector3 motion = (rotatedMoveVector * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime;
        characterController.Move(motion);
    }

    private Vector2 GetMovementVectorNormalized()
    {
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

}
