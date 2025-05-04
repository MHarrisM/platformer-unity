using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float playerStepsCD = 0.3f;
    [SerializeField] private float moveInput;

    private float stepVFXTimer = 0f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float jumpCooldown;
    private float jumpCooldownCounter;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;
    [SerializeField] private float upJumpMultiplier = 2.0f;
    bool isJumping;
    [SerializeField] private float maxJumpHeight;

    [Header("VFX")]
    [SerializeField] private GameObject dirtPuffVFX;

    [Header("Animator")]
    private Animator animator;

    public bool isMovementDisabled = false;
    public bool isFlippingDisabled = false;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private int facingDirection = 1;
    public int FacingDirection => facingDirection;
    private PlayerCombat playerCombat;
    Vector2 vecGravity;
    private Vector2 velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        animator = GetComponent<Animator>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;
        
        
        moveInput = Input.GetAxisRaw("Horizontal");
        
      
        
        
        animator.SetBool("isJump", !IsGrounded());
        animator.SetBool("isRun", Mathf.Abs(moveInput) > 0.1f);

        // Flip sprite based on direction
        if (!isFlippingDisabled)
        {
            if (moveInput > 0)
            {

                facingDirection = 1;
                if (!isFacingRight) Flip();
                if (IsGrounded())
                {
                    stepVFXTimer -= Time.deltaTime;
                    if (stepVFXTimer <= 0f)
                    {
                        SpawnDirtPuff();
                        stepVFXTimer = playerStepsCD;
                    }

                }
            }
            else if (moveInput < 0)
            {

                facingDirection = -1;
                if (isFacingRight) Flip();
                if (IsGrounded())
                {
                    stepVFXTimer -= Time.deltaTime;
                    if (stepVFXTimer <= 0f)
                    {
                        SpawnDirtPuff();
                        stepVFXTimer = playerStepsCD;
                    }

                }
            }
        }
       

        //jump
        if (IsGrounded())
        {
            jumpCooldownCounter = jumpCooldown;
        }
        else
        {
            jumpCooldownCounter = Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpCooldownCounter > 0f && IsGrounded())
        {
            isJumping = true;
            

            jumpCooldownCounter = 0f;
        }


        if (Input.GetKeyDown(KeyCode.F) && !isDashing)
        {
            TryDash();
        }
    }
    void FixedUpdate()
    {
        if (isDashing) return;

        // Horizontal movement
        float newX = moveInput * moveSpeed;
        float newY = rb.velocity.y;
        velocity = new Vector2(newX, newY);

        if (isJumping)
        {
            isJumping = false;
            
            Jump();
            
        }


        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0f)
        {
            rb.gravityScale = upJumpMultiplier;
        }
        else if (rb.velocity.y < 0f)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        

        rb.velocity = velocity;
    }

    void Jump()
    {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * maxJumpHeight);
        
        if (velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
        }
        velocity.y += jumpSpeed;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }


    bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheckPoint.position, new Vector2(0.5f, 1.0f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    void TryDash()
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }
    
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(facingDirection * dashSpeed, 0f);
        SpawnDirtPuff();
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        if (dashCooldown > 0f)
        {
            yield return new WaitForSeconds(dashCooldown);
        }

        canDash = true;
    }


    //VFX
    void SpawnDirtPuff()
    {
        Quaternion rotation = Quaternion.Euler(-90.0f, 0f, 0f); //VFX spawn with wrong rotation for some reason, fixes it
        GameObject puff = Instantiate(dirtPuffVFX, transform.position + new Vector3(0f, -0.5f, 0f), rotation);
        Destroy(puff, 0.5f);
    }
}
