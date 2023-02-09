using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform overHeadCheckCollider;
    [SerializeField] LayerMask groundLayer;

    const float groundCheckRadius = 0.2f;
    const float overHeadCheckRadius = 0.2f;

    [SerializeField] float speed = 2;
    [SerializeField] float jumpPower = 10;
    float horizontalValue;
    float runSpeedModifier = 2f;
    float crouchSpeedModifier = 0.1f;

    bool facingRight = true;
    bool isRunnin;
    bool jump;
    bool isGrounded;
    [SerializeField] bool crouchPressed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {

    }
    void Update()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.LeftShift)) { isRunnin = true; } else isRunnin = false;
        if (Input.GetButtonDown("Jump")) { jump = true; animator.SetBool("Jump", true); }
        else if (Input.GetButtonUp("Jump")) jump = false;
        if (Input.GetButtonDown("Crouch"))
        {
            crouchPressed = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouchPressed = false;
        }
        animator.SetFloat("yVelocity", rb.velocity.y);
    }
    void FixedUpdate()
    {
        Move(horizontalValue, jump, crouchPressed);
        GroundCheck();
    }
    void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;

        }
        animator.SetBool("Jump", !isGrounded);
    }
    void Move(float dir, bool jumpFlag, bool crouchFlag)
    {
        #region Jump & Crouch
        // if (!crouchFlag)
        // {
        //     if (Physics2D.OverlapCircle(overHeadCheckCollider.position, overHeadCheckRadius, groundLayer))
        //         crouchFlag = true;
        // }

        if (isGrounded)
        {
            standingCollider.enabled = !crouchFlag;
            if (jumpFlag)
            {
                isGrounded = false;
                jumpFlag = false;
                rb.AddForce(new Vector2(0f, jumpPower));
            }
        }
        animator.SetBool("Crouch", crouchFlag);
        #endregion
        #region Move & Run
        float xVal = dir * speed * Time.fixedDeltaTime * 100;
        if (isRunnin) xVal *= runSpeedModifier;
        if (crouchFlag) xVal *= crouchSpeedModifier;

        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        //Vector3 currentScale = transfor.localScale;
        if (facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }
}
