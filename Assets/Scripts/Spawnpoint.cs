using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] private Light2D spawnLight;
    [SerializeField] private Animator animator;
    [SerializeField] private int lightIntensity;
    private bool activate;
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player") && activate == false )
        {
            activate = true;
            Player scriptPlayer = collision.GetComponent<Player>();
            scriptPlayer.ResetTries();
            animator.SetBool("Activate", true);
            spawnLight.intensity = lightIntensity;
            
            
        }
    }
}
