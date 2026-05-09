using UnityEngine;

public class EnemieController : MonoBehaviour
{
    private bool playerDetected = false;
    [SerializeField] GameObject inkBullet;
    [SerializeField] private Transform enemiePosition;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spawnBulletPoint;

    private void Update()
    {
        playerDetected = Physics2D.OverlapCircle(enemiePosition.position, detectionRadius, playerLayer);
        animator.SetBool("playerDetected", playerDetected);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemiePosition.position,detectionRadius);
    }

    private void CreateInkBullet()
    {
        Instantiate(inkBullet, spawnBulletPoint.position, spawnBulletPoint.rotation);
    }


}
