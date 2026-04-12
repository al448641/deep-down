using System.Reflection;
using UnityEngine;
using Unity.Cinemachine;
using System.Configuration.Assemblies;

public class CameraController : MonoBehaviour
{

    [Header("Zoom Configuration")]
    [SerializeField] private float maxZoom;
    [SerializeField] private float smoothTimeforZoom;
    [SerializeField] private CinemachineCamera cam;

    private float zoom;
    private float originalZoom;
    private float velocity = 0f;

    [Header("Shake Configuration")]

    [SerializeField] private float maxShakeAmplitudeGain;
    [SerializeField] private float maxShakeFrequencyGain;
    private CinemachineBasicMultiChannelPerlin shakeEffect;
    private float UpdateShakeIntensity;


    void Awake()
    {
        originalZoom = cam.Lens.OrthographicSize;
        zoom = originalZoom;
        shakeEffect = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        
    }


    void OnEnable()
    {
        Player.JumpIsCharging += UpdateZoom;
        Player.JumpIsCharging += UpdateChargeJumpIntensity;
    }

    void OnDisable()
    {
        Player.JumpIsCharging -= UpdateZoom;
        Player.JumpIsCharging -= UpdateChargeJumpIntensity;
    }

    void Update()
    {
        //Here we change the zoom when the jump is loading
        cam.Lens.OrthographicSize = Mathf.SmoothDamp(cam.Lens.OrthographicSize,zoom,ref velocity,smoothTimeforZoom);

        //Here we change the shake when the jump is loading
        shakeEffect.AmplitudeGain = UpdateShakeIntensity * maxShakeAmplitudeGain;
        shakeEffect.FrequencyGain = UpdateShakeIntensity * maxShakeFrequencyGain;

    }

    void UpdateZoom(float intensityPercent)
    {
        //Here we take how much is left for the maximum zoom, or the maximum jump load, using a function that uses linear interpolation
        zoom = Mathf.Lerp(originalZoom,maxZoom,intensityPercent);

    }

    void UpdateChargeJumpIntensity(float intensityPercent)
    {
        UpdateShakeIntensity = intensityPercent;
    }

}
