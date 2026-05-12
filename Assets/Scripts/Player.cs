using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System;



public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    private InputAction jumpAction;
    private bool onGround;
    private bool onRightWall;
    private bool onLeftWall;
    private bool lookingRight;
    private Vector3 mousePosition;
    private bool chargingJump = false;
    private float actualForce;
    private float chargePercent;

    [SerializeField] private Animator animator;

    //respawn
    private int lives = 3;
    private int tries = 5;
    private Vector3 lastPointOnGround;
    private Vector3 lastSpawnPoint;

    //mobile platforms
    private Transform currentPlatform;
    Collider2D ground;
    Collider2D leftWall;
    Collider2D rightWall;

    //collectables
    private int garbageRecollected = 0;

    //camera stuff
    public static event Action<float> JumpIsCharging;
    [SerializeField] private CameraController cam;

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
    [SerializeField] private LayerMask platforms;

    //Code -------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction = new InputAction(binding: "<Mouse>/leftButton");
        jumpAction.Enable();

    }

    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        DetectGroundOrWalls();

        //configurate animator booleans
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

        //to the cleaning animations 

        if (rb.linearVelocity.x > 0)
        {
            lookingRight = true;
        }
        else { lookingRight = false; }

        //to follow the platform movement
        if (currentPlatform != null && (onGround || onLeftWall || onRightWall))
        {
            rb.gravityScale = 0;
            transform.parent = currentPlatform;
        }
        else if (currentPlatform == null)
        {
            transform.parent = null;
            rb.gravityScale = 1;
        }

        //keeps last position on ground
        if (onGround && ground != null && !ground.CompareTag("mobilePlatform"))
        {
            lastPointOnGround = transform.position;
        }

        //to slide on the walls
        WallSlide();

        //Player movement
        if (jumpAction.WasPressedThisFrame())
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

        if (jumpAction.WasReleasedThisFrame() && chargingJump)
        {
            JumpIsCharging?.Invoke(0f);
            chargingJump = false;
            animator.SetBool("chargingJump", false);

            if (CanJump())
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(VectorToMouse() * actualForce, ForceMode2D.Impulse);
                animator.SetBool("Jumping", true);
            }
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
        if (onGround)
            return mousePosition.y > transform.position.y + weightHead;

        if (onRightWall)
            return mousePosition.x < transform.position.x - torsoWidh;

        if (onLeftWall)
            return mousePosition.x > transform.position.x + torsoWidh;

        return false;
    }

    private void DetectGroundOrWalls()
    {
        ground = Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * offsetY, sizeGroundCheck, 0f, groundAndWalls);
        leftWall = Physics2D.OverlapBox((Vector2)transform.position + Vector2.left * offsetX, sizeWallCheck, 0f, groundAndWalls);
        rightWall = Physics2D.OverlapBox((Vector2)transform.position + Vector2.right * offsetX, sizeWallCheck, 0f, groundAndWalls);

        onGround = ground;
        onLeftWall = leftWall;
        onRightWall = rightWall;

        if (ground != null && ground.CompareTag("mobilePlatform"))
            currentPlatform = ground.transform;
        else if (leftWall != null && leftWall.CompareTag("mobilePlatform"))
            currentPlatform = leftWall.transform;
        else if (rightWall != null && rightWall.CompareTag("mobilePlatform"))
            currentPlatform = rightWall.transform;
        else
            currentPlatform = null;
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        //with this the player doesn't slide on the ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground&Walls"))
        {
            rb.linearVelocity = Vector2.zero;

        }

        //damage management
        if (collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {

            tries -= 1;
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("bullet");
            foreach (GameObject bullet in bullets)
            {
                 Animator animatorBullet = bullet.GetComponent<Animator>();
                 animatorBullet.Play("bala choque");
            }
            rb.bodyType = RigidbodyType2D.Static;
            cam.StartDamageTimer();
            DamageAnimation();
        }


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            tries = 5;
            lastSpawnPoint = collision.transform.position;
        }

        if (collision.CompareTag("Collectable"))
        {
            if (lookingRight)
            {
                animator.Play("cleaning right");
            }
            else { animator.Play("cleaning left"); }

            garbageRecollected += 1;
            Debug.Log("Basura recogida | Total en el inventario: " + garbageRecollected);
        }
    }

    private void DamageAnimation()
    {
        if (lookingRight)
        {
           
            animator.Play("damage right");
        }
        else
        {
            
            animator.Play("damage left");
        }

    }

    private void Respawn()
    {
        if (tries > 0)
        {
            transform.position = lastPointOnGround;
            Debug.Log("Impacto detectado. Vidas restantes: " + lives + " | Intentos restantes: " + tries);

        }
        else
        {
            lives -= 1;

            if (lives > 0)
            {
                tries = 5;
                transform.position = lastSpawnPoint;
                Debug.Log("Impacto detectado. Vidas restantes: " + lives + " | Intentos restantes: " + tries);
            }
            else
            {
                Debug.Log("Game Over");
            }

        }
        rb.bodyType = RigidbodyType2D.Dynamic;

    }

    public void ResetTries()
    {
        tries = 5;
    }
    private void WallSlide()
    {


        if ((onRightWall || onLeftWall)  && !onGround && rb.linearVelocity.y < 0)
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


