using Unity.Cinemachine;
using UnityEngine;


public class UIController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vCam;
    private float initialSize;
    private Vector3 initialScale;
    void Start()
    {
        initialSize = vCam.Lens.OrthographicSize;
        initialScale = transform.localScale;

    }

    void LateUpdate()
    {
        float scaleFactor = vCam.Lens.OrthographicSize / initialSize;
        transform.localScale = initialScale * scaleFactor;
    }
}
