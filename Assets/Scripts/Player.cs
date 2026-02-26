using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    private bool onGround;
    private bool onRightWall;
    private bool onLeftWall;
    private Vector3 mousePosition;
    [SerializeField] private Vector2 sizeGroundCheck;
    [SerializeField] private Vector2 sizeWallCheck;
    [SerializeField] private float offsetY = 0.2f;
    [SerializeField] private float offsetX = 0.2f;
    [SerializeField] private float weightHead = 0.2f;
    [SerializeField] private float torsoWidh = 0.2f;
    [SerializeField] private LayerMask groundAndWalls;
    



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        DetectGroundOrWalls();
        JumpToMouseDirection();
        

    }

    private Vector2 VectorToMouse()
    {

        Vector2 mouseVector2D = mousePosition;
        Vector2 direction = (mouseVector2D - (Vector2)transform.position).normalized;

        return direction;
    }

    private void JumpToMouseDirection()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && CanJump() == true)
        {
            rb.AddForce(VectorToMouse() * 10f, ForceMode2D.Impulse);
        }
    }

    private bool CanJump()
    {
        //check if the player is touching the ground to allow them to jump 

        if ((onGround == true) && (mousePosition.y > transform.position.y + weightHead))
        {
            return true;
        }
        else if ((onRightWall == true) && (mousePosition.x < transform.position.x - torsoWidh))
        {
            return true; 
        }
        else if ((onLeftWall == true) && (mousePosition.x > transform.position.x + torsoWidh))
        {
            return true;
        }
        else { return false; }
            
    }

    /*private bool CanJumpRightWall()
    {
        //Check if the player is on the right wall and the mouse placed in the right place

        if ((onRightWall == true) && (mousePosition.x > transform.position.x + torsoWidh))
        {
            return true;
        }
        else
        {
            return false;
        }

    }*/

    private void DetectGroundOrWalls()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * offsetY, sizeGroundCheck,0f, groundAndWalls);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + Vector2.left * offsetX, sizeWallCheck, 0f,groundAndWalls);
        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + Vector2.right * offsetX, sizeWallCheck, 0f, groundAndWalls);
    }

    private void OnDrawGizmos()
    {
        //draw the squares that show where the character is touching
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.down * offsetY, sizeGroundCheck);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.right * offsetX, sizeWallCheck);
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.left * offsetX, sizeWallCheck);

        Gizmos.color = Color.green;
        Vector3 headLevel = new Vector3(transform.position.x, transform.position.y + weightHead, 0);
        Vector3 torsoLevelRight = new Vector3(transform.position.x + torsoWidh, transform.position.y, 0);
        Vector3 torsoLevelLeft = new Vector3(transform.position.x - torsoWidh, transform.position.y, 0);
        Gizmos.DrawLine(transform.position, headLevel);
        Gizmos.DrawLine(transform.position, torsoLevelRight);
        Gizmos.DrawLine(transform.position, torsoLevelLeft);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground&Walls"))
        {
            rb.linearVelocity = Vector2.zero;

        }
    }
}


