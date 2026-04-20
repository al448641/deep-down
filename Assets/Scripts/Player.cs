
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System;


public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    private bool onGround;
    private bool onRightWall;
    private bool onLeftWall;
    private bool onPlatform;
    private Vector3 mousePosition;
    private bool chargingJump = false;
    private float actualForce;
    private float chargePercent;

    [SerializeField] private Animator animator;

    //respawn
    private Vector3 lastPointOnGround;

    //events
    public static event Action<float> JumpIsCharging;

    //options inside the scene

     [Header("Jump and slide")]
    [SerializeField] private float minimForce = 5f;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private float jumpchargeVel = 10f;
    [SerializeField] private float wallSlidingSpeed;

    [Header("Detection")]
    [SerializeField] private Vector2 sizeGroundCheck;
    [SerializeField] private Vector2 sizeWallCheck;
    [SerializeField] private float offsetY = 0.2f;
    [SerializeField] private float offsetX = 0.2f;
    [SerializeField] private float weightHead = 0.2f;
    [SerializeField] private float torsoWidh = 0.2f;
    [SerializeField] private LayerMask groundAndWalls;

    // here starts the code -----------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        DetectGroundOrWalls();
        animator.SetBool("OnGround", onGround);
        animator.SetBool("OnrightWall", onRightWall);
        animator.SetBool("OnleftWall", onLeftWall);

        if (rb.linearVelocity.y <= 0 && (!onRightWall || !onLeftWall))
        {
            animator.SetBool("Falling", true);
            animator.SetBool("Jumping", false);
        }
        else
        {
            animator.SetBool("Falling", false);
        }

        if (mousePosition.x < transform.position.x)
        {
            animator.SetBool("PointerOnRight", false);
        }
        else
        {
            animator.SetBool("PointerOnRight", true);
        }

        WallSlide();

        //keeps last position on ground
        if ((onGround == true) && (onPlatform == false))
        {
            lastPointOnGround = transform.position;
        }

        //Player movement
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            chargingJump = true;
            actualForce = minimForce;
            animator.SetBool("chargingJump", true);
        }

        if (chargingJump)
        {
            actualForce += jumpchargeVel * Time.deltaTime;
            actualForce = Mathf.Clamp(actualForce, minimForce, maxForce);
            chargePercent = (actualForce - minimForce) / (maxForce - minimForce);
            JumpIsCharging?.Invoke(chargePercent);

        }

        
        if (Mouse.current.leftButton.wasReleasedThisFrame && chargingJump && CanJump())
        {
            rb.linearVelocity = Vector2.zero;
            if (onPlatform)
            {
                rb.gravityScale = 1;
            }
            rb.AddForce(VectorToMouse() * actualForce, ForceMode2D.Impulse);
            chargingJump = false;
            animator.SetBool("chargingJump", false);
            animator.SetBool("Jumping", true);
            JumpIsCharging?.Invoke(0f);

        }
        
        if (Mouse.current.leftButton.wasReleasedThisFrame && chargingJump && !CanJump())
        {
            JumpIsCharging?.Invoke(0f);
            chargingJump = false;
            animator.SetBool("chargingJump", false);
        }

    }

    private Vector2 VectorToMouse()
    {
        //gives the direction of the mouse as a vector

        Vector2 mouseVector2D = mousePosition;
        Vector2 direction = (mouseVector2D - (Vector2)transform.position).normalized;

        return direction;
    }

    private bool CanJump()
    {
        //check if the player is touching the ground and the mouse is positioned correctly to allow them to jump 

        if (onGround == true) 
        {
            return mousePosition.y > transform.position.y + weightHead;
        }
        if (onRightWall == true)
        {
            return mousePosition.x < transform.position.x - torsoWidh; 
        }
        if (onLeftWall == true) 
        {
            return mousePosition.x > transform.position.x + torsoWidh;
        }

        return false;
            
    }

    private void DetectGroundOrWalls()
    {
        //use overlap box to detect if the player is touching either the wall or the floor
        onGround = Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * offsetY, sizeGroundCheck,0f, groundAndWalls);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + Vector2.left * offsetX, sizeWallCheck, 0f,groundAndWalls);
        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + Vector2.right * offsetX, sizeWallCheck, 0f, groundAndWalls);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //with this the player doesn't slide on the ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground&Walls"))
        {
            rb.linearVelocity = Vector2.zero;

        }

        //with this the player position will follow the mobile platforms
        if (collision.gameObject.tag == "mobilePlatform")
        {
            rb.gravityScale = 0;
            transform.parent = collision.transform;
            onPlatform = true;

        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            transform.position = lastPointOnGround;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "mobilePlatform")
        {
            transform.parent = null;
            onPlatform = false;
        }
    }

    private void WallSlide()
    {


        if ((onRightWall || onLeftWall) && !onGround && !onPlatform && rb.linearVelocity.y < 0)
        {
            float limitedVelY = Mathf.Max(rb.linearVelocity.y, -wallSlidingSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, limitedVelY);
        }

    }

     private void OnDrawGizmos()
    {
        //draw the squares that show where the character is touching
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.down * offsetY, sizeGroundCheck);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.right * offsetX, sizeWallCheck);
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.left * offsetX, sizeWallCheck);
        
        //draw the lines that show where the mouse needs to be placed to allow the player jump
        Gizmos.color = Color.green;
        Vector3 headLevel = new Vector3(transform.position.x, transform.position.y + weightHead, 0);
        Vector3 torsoLevelRight = new Vector3(transform.position.x + torsoWidh, transform.position.y, 0);
        Vector3 torsoLevelLeft = new Vector3(transform.position.x - torsoWidh, transform.position.y, 0);
        Gizmos.DrawLine(transform.position, headLevel);
        Gizmos.DrawLine(transform.position, torsoLevelRight);
        Gizmos.DrawLine(transform.position, torsoLevelLeft);
    }


}


