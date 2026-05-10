using UnityEngine;

public class EchinusController : MonoBehaviour
{
    private bool playerDetected = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform enemiePosition;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject collisionAttack;


    private void Update()
    {
        playerDetected = Physics2D.OverlapCircle(enemiePosition.position, detectionRadius, playerLayer);
        animator.SetBool("PlayerDetected", playerDetected);
    }

    private void AttackCollider()
    {
        collisionAttack.SetActive(true);
    }

    private void IdleCollider()
    {
        collisionAttack.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemiePosition.position, detectionRadius);
    }

}
