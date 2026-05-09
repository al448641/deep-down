using System;
using UnityEngine;

public class CleanGarbage : MonoBehaviour
{
    public bool cleaned = false;
    [SerializeField] private Animator animator;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") && (cleaned == false))
        {
            cleaned = true;
            animator.SetBool("Limpiando", true);

        }
    }

    private void DestroyGarbage()
    {
        Destroy(gameObject);
    }
}
