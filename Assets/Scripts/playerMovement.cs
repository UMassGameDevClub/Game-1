using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed = 0.02f;
    public float moveVelocity = 12.0f;
    public float jumpForce = 10.0f;
    public float linearDrag = 1.0f;
    public float groundedRaycastLength = 1.8f;

    public LayerMask groundLayer;
    private bool grounded = true;

    public bool OnWarp { get; set; } = false;

    public Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        rb.drag = linearDrag;
    }

    void Update()
    {
        // This is a combo of a/d inputs and left/right inputs
        // Negative = Left
        // Positive = Right
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("horizontalInput", Mathf.Abs(horizontalInput));

        // character faces direction of movement
        // if (horizontalInput >= 0) {
        //     //transform.Rotate(0.0f, 0.0f, 0.0f, Space.Self);
        // } else {
        //     transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
        // }

        // Likewise, this is w/s and up/down
        // Negative = Down
        // Positive = Up
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (!WarpInfo.CurrentlyWarping)
            rb.velocity = new Vector2(moveVelocity * horizontalInput, rb.velocity.y);
        
        // the box sprite is about 1.0f high, so I set the length of the ray to 0.8f since it starts from the center
        grounded = Physics2D.Raycast(transform.position, Vector2.down, groundedRaycastLength, groundLayer).collider != null;

        if ((verticalInput > 0 || Input.GetKey("space")) && OnWarp == false )
        {
            if (grounded)
            {
                rb.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
            }
        }
    }
}
