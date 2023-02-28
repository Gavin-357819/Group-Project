using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementTutorial : MonoBehaviour
{
    [Header("Movement")]
    public Transform orientation;

    public float moveSpeed;
    public float groundDrag;

    public float gravity;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Physics")]
    public PhysicMaterial airMaterial;
    public PhysicMaterial groundMaterial;

    public Collider coll;

    [Header("Animations")]
    public Animator anim;
    public float movementAnimSensitivity;



    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            coll.material = groundMaterial;

            //end jumping animation
            if (anim.GetBool("onAir"))
                anim.SetBool("onAir", false);
        }
        else
        {
            coll.material = airMaterial;
            rb.drag = 0;

            //start jumping animation
            if (!anim.GetBool("onAir"))
                anim.SetBool("onAir", true);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        //gravity
        rb.AddForce(-transform.up * gravity * Time.deltaTime, ForceMode.Force);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        //moving animations
        if (flatVel.magnitude > movementAnimSensitivity)
        {
            if (!anim.GetBool("jogging"))
                anim.SetBool("jogging", true);
        }
        else
        {
            if (anim.GetBool("jogging"))
                anim.SetBool("jogging", false);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}