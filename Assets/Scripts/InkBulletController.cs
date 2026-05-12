using UnityEditor.Tilemaps;
using UnityEngine;

public class InkBulletController : MonoBehaviour
{
    private Transform playerPosition;
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    Rigidbody2D rb;
    private bool crashed = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.transform;
    }
    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground&Walls") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerLayer") || collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            animator.SetBool("Crashed", true);
            crashed = true;
            rb.linearVelocity = Vector3.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void FollowPlayer()
    {
        if (!crashed)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, playerPosition.position, speed * Time.deltaTime);
        }
    }

    private void DestroyInkBullet()
    {
        Destroy(gameObject);
    }
}
